--DECLARE @From DATE = '2018-04-01'
--DECLARE @To DATE = '2018-04-30'
--DECLARE @UserId INT = 1

WITH LocationChanges AS (
            SELECT 
                LocationId,
                Timestamp,
                ROW_NUMBER() OVER (ORDER BY Timestamp) as RowNum
            FROM Tracking.Tracking
            WHERE UserId = 1
                AND Timestamp BETWEEN '2025-02-24' AND '2025-02-28'
                AND LocationId IS NOT NULL
        ),
        GroupBoundaries AS (
            SELECT 
                lc1.LocationId,
                lc1.Timestamp as EntryTimestamp,
                (
                    SELECT MAX(lc2.Timestamp)
                    FROM LocationChanges lc2
                    WHERE lc2.LocationId = lc1.LocationId
                    AND NOT EXISTS (
                        SELECT 1 
                        FROM LocationChanges lc3
                        WHERE lc3.RowNum > lc2.RowNum
                        AND lc3.RowNum < (
                            SELECT MIN(RowNum)
                            FROM LocationChanges lc4
                            WHERE lc4.RowNum > lc1.RowNum
                            AND lc4.LocationId != lc1.LocationId
                        )
                    )
                ) as ExitTimestamp
            FROM LocationChanges lc1
            WHERE NOT EXISTS (
                SELECT 1 
                FROM LocationChanges lc3
                WHERE lc3.RowNum = lc1.RowNum - 1 
                AND lc3.LocationId = lc1.LocationId
            )
        )
        SELECT 
            LocationId,
            EntryTimestamp,
            ExitTimestamp
        FROM GroupBoundaries
        ORDER BY EntryTimestamp