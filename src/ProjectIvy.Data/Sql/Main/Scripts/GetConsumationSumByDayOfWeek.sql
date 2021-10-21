--DECLARE @UserId INT = 1

SELECT
    DATEPART(DW, Date) AS [Key],
    SUM(Volume) AS [Value]
FROM
Beer.Consumation
WHERE UserId = @UserId
    AND (@From IS NULL OR @From <= Date)
    AND (@To IS NULL OR @To >= Date)
GROUP BY DATEPART(DW, Date)