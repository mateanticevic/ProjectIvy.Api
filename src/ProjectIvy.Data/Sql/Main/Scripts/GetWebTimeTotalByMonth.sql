--DECLARE @DeviceValueId NVARCHAR(MAX) = NULL
--DECLARE @DomainValueId NVARCHAR(MAX) = 'www.facebook.com'
--DECLARE @WebValueId NVARCHAR(MAX) = NULL
--DECLARE @IsSecured BIT = NULL
--DECLARE @From DATETIME = NULL
--DECLARE @To DATETIME = NULL
--DECLARE @UserId.Value INT = 1

SELECT
	YEAR(TimestampEnd) AS [Year],
	MONTH(TimestampEnd) AS [Month],
    SUM(DATEDIFF(s, TimestampStart, TimestampEnd)) AS Seconds,
	COUNT(*) AS [Sessions]
FROM Log.BrowserLog bl
JOIN Net.Domain d ON d.Id = bl.DomainId
JOIN Net.Web w ON w.Id = d.WebId
JOIN Inv.Device device ON device.Id = bl.DeviceId AND device.UserId.Value = @UserId.Value
WHERE 1=1
	AND ISNULL(@DeviceValueId, device.ValueId) = device.ValueId
	AND ISNULL(@DomainValueId, d.ValueId) = d.ValueId
	AND ISNULL(@WebValueId, w.ValueId) = w.ValueId
	AND ISNULL(@IsSecured, bl.IsSecured) = bl.IsSecured
	AND (@From IS NULL OR CAST(@From AS DATE) <= CAST(bl.TimestampStart AS DATE))
	AND (@To IS NULL OR CAST(@To AS DATE) >= CAST(bl.TimestampEnd AS DATE))
GROUP BY YEAR(TimestampEnd), MONTH(TimestampEnd)
ORDER BY [Year] DESC, [Month] DESC