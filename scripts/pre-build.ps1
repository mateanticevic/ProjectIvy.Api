$version = git describe --tags;

$projects = Get-ChildItem ".." -Recurse *.csproj

Foreach($project in $projects)
{
    $file = Get-Content $project.FullName
    $file = $file -replace '<Version>[0-9]+.[0-9]+.[0-9]+</Version>', ('<Version>' + $version + '</Version>')
    $file = $file -replace '<VersionPrefix>[0-9]+.[0-9]+.[0-9]+</VersionPrefix>', ('<VersionPrefix>' + $version + '</VersionPrefix>')

    $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
    [System.IO.File]::WriteAllLines($project.FullName, $file, $Utf8NoBomEncoding)
}