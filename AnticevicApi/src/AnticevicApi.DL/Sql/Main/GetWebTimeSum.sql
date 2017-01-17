--DECLARE @DeviceValueId nvarchar(max) = NULL
--DECLARE @From DATETIME = '2017-01-16'
--DECLARE @To DATETIME = '2017-01-17'
--DECLARE @UserId INT = 1
--DECLARE @Page INT = 0
--DECLARE @PageSize INT = 10

SELECT
    w.ValueId AS WebValueId,
    SUM(DATEDIFF(s, TimestampStart, TimestampEnd)) AS Seconds
FROM Log.BrowserLog bl
JOIN Net.Domain d ON d.Id = bl.DomainId
JOIN Net.Web w ON w.Id = d.WebId
JOIN Inv.Device device ON device.Id = bl.DeviceId AND device.UserId = @UserId
WHERE (@DeviceValueId IS NULL OR device.ValueId = @DeviceValueId)
	AND (@From IS NULL OR @From < bl.TimestampStart)
	AND (@To IS NULL OR @To > bl.TimestampEnd)
GROUP BY w.ValueId
ORDER BY Seconds DESC
OFFSET (@Page * @PageSize) ROWS
FETCH NEXT @PageSize ROWS ONLY;