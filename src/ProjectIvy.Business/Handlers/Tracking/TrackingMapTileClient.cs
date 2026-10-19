using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;

namespace ProjectIvy.Business.Handlers.Tracking;

/// <summary>Shared tile downloads and disk cache; never stores tracking points or rendered maps.</summary>
public sealed class TrackingMapTileClient : IDisposable
{
    private readonly IHttpClientFactory _clients;
    private readonly string _url;
    private readonly string _cacheDirectory;
    private readonly SemaphoreSlim _downloads = new(4);
    // Bounded stripes avoid retaining a semaphore for every tile ever requested.
    private readonly SemaphoreSlim[] _locks = Enumerable.Range(0, 128).Select(_ => new SemaphoreSlim(1)).ToArray();

    public TrackingMapTileClient(IHttpClientFactory clients, IConfiguration configuration)
    {
        _clients = clients;
        _url = configuration["TrackingExport:TileUrl"] ?? "https://tile.openstreetmap.org/{z}/{x}/{y}.png";
        _cacheDirectory = configuration["TrackingExport:CacheDirectory"]
            ?? Path.Combine(Path.GetTempPath(), "projectivy-map-tiles");
    }

    internal async Task<byte[]> GetTile(int z, int x, int y, CancellationToken cancellationToken)
    {
        var url = _url.Replace("{z}", z.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Replace("{x}", x.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Replace("{y}", y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(url));
        var file = Path.Combine(_cacheDirectory, Convert.ToHexString(hash) + ".json");
        var gate = _locks[hash[0] % _locks.Length];
        await gate.WaitAsync(cancellationToken);
        try
        {
            var cached = await ReadCache(file, cancellationToken);
            if (cached is not null && cached.FreshUntil > DateTimeOffset.UtcNow) return cached.Data;

            await _downloads.WaitAsync(cancellationToken);
            try
            {
                using var client = _clients.CreateClient("TrackingMapTiles");
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.UserAgent.ParseAdd("ProjectIvy/1.0");
                if (cached?.ETag is not null) request.Headers.TryAddWithoutValidation("If-None-Match", cached.ETag);
                if (cached?.LastModified is not null) request.Headers.IfModifiedSince = cached.LastModified;
                using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var notModified = response.StatusCode == HttpStatusCode.NotModified && cached is not null;
                if (!notModified) response.EnsureSuccessStatusCode();
                byte[] data;
                if (notModified) data = cached.Data;
                else
                {
                    // Bound downloads and decoder input independently of upstream Content-Length.
                    await using var body = await response.Content.ReadAsStreamAsync(cancellationToken);
                    using var buffer = new MemoryStream();
                    var chunk = new byte[8192];
                    int count;
                    while ((count = await body.ReadAsync(chunk, cancellationToken)) > 0)
                    {
                        if (buffer.Length + count > 1024 * 1024)
                            throw new InvalidDataException("Map tile exceeds the download limit.");
                        buffer.Write(chunk, 0, count);
                    }
                    data = buffer.ToArray();
                    var info = Image.Identify(data);
                    if (info.Width != 256 || info.Height != 256 || info.FrameMetadataCollection.Count > 1)
                        throw new InvalidDataException("Map tile must be a single 256×256 image.");
                    using var decoded = Image.Load(data);
                }

                var now = DateTimeOffset.UtcNow;
                var control = response.Headers.CacheControl;
                var age = response.Headers.Age ?? TimeSpan.Zero;
                var lifetime = control?.MaxAge
                    ?? (response.Content.Headers.Expires.HasValue
                        ? response.Content.Headers.Expires.Value - (response.Headers.Date ?? now)
                        : notModified ? cached.Lifetime : TimeSpan.FromDays(7));
                var revalidate = control?.NoCache ?? (notModified && cached.Revalidate);
                var freshUntil = revalidate ? now
                    : now + lifetime - TimeSpan.FromTicks(Math.Max(age.Ticks, (now - (response.Headers.Date ?? now)).Ticks));
                var entry = new CacheEntry(data, freshUntil, lifetime, revalidate,
                    response.Headers.ETag?.ToString() ?? (notModified ? cached.ETag : null),
                    response.Content.Headers.LastModified ?? (notModified ? cached.LastModified : null));
                if (control?.NoStore == true)
                {
                    if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                }
                else await WriteCache(file, entry, cancellationToken);
                return data;
            }
            finally { _downloads.Release(); }
        }
        finally { gate.Release(); }
    }

    private static async Task<CacheEntry> ReadCache(string file, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = System.IO.File.OpenRead(file);
            var cached = await JsonSerializer.DeserializeAsync<CacheEntry>(stream, cancellationToken: cancellationToken);
            return cached?.Data?.Length > 0 ? cached : null;
        }
        catch (FileNotFoundException) { return null; }
        catch (DirectoryNotFoundException) { return null; }
        catch (JsonException) { return null; }
    }

    private static async Task WriteCache(string file, CacheEntry entry, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(file));
        var temporary = file + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = System.IO.File.Create(temporary))
                await JsonSerializer.SerializeAsync(stream, entry, cancellationToken: cancellationToken);
            System.IO.File.Move(temporary, file, overwrite: true);
        }
        finally { if (System.IO.File.Exists(temporary)) System.IO.File.Delete(temporary); }
    }

    public void Dispose()
    {
        _downloads.Dispose();
        foreach (var gate in _locks) gate.Dispose();
    }

    private sealed record CacheEntry(byte[] Data, DateTimeOffset FreshUntil, TimeSpan Lifetime, bool Revalidate,
        string ETag, DateTimeOffset? LastModified);
}
