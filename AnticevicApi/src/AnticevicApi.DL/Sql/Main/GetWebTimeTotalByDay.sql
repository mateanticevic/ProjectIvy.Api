--DECLARE @DeviceValueId nvarchar(max) = NULL
--DECLARE @From DATETIME = '2017-1-17'
--DECLARE @To DATETIME = '2017-1-18'
--DECLARE @UserId INT = 1

SELECT
	CAST(bl.TimestampEnd AS DATE) AS [Day],
    SUM(DATEDIFF(s, TimestampStart, TimestampEnd)) AS Seconds,
	COUNT(*) AS [Sessions]
FROM Log.BrowserLog bl
JOIN Net.Domain d ON d.Id = bl.DomainId
JOIN Net.Web w ON w.Id = d.WebId
JOIN Inv.Device device ON device.Id = bl.DeviceId AND device.UserId = @UserId
WHERE (@DeviceValueId IS NULL OR device.ValueId = @DeviceValueId)
	AND (@From IS NULL OR CAST(@From AS DATE) <= CAST(bl.TimestampStart AS DATE))
	AND (@To IS NULL OR CAST(@To AS DATE) >= CAST(bl.TimestampEnd AS DATE))
GROUP BY CAST(bl.TimestampEnd AS DATE)
ORDER BY [Day] DESC