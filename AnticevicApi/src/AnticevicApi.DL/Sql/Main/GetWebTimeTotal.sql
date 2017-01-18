--DECLARE @DeviceValueId nvarchar(max) = NULL
--DECLARE @From DATETIME = NULL
--DECLARE @To DATETIME = NULL
--DECLARE @UserId INT = 1

SELECT
    SUM(DATEDIFF(s, TimestampStart, TimestampEnd)) AS Seconds
FROM Log.BrowserLog bl
JOIN Net.Domain d ON d.Id = bl.DomainId
JOIN Net.Web w ON w.Id = d.WebId
JOIN Inv.Device device ON device.Id = bl.DeviceId AND device.UserId = @UserId
WHERE (@DeviceValueId IS NULL OR device.ValueId = @DeviceValueId)
	AND (@From IS NULL OR @From < bl.TimestampStart)
	AND (@To IS NULL OR @To > bl.TimestampEnd)
ORDER BY Seconds DESC