--DECLARE @UserId INT = 1;
--DECLARE @FromLocationId INT = 1;
--DECLARE @ToLocationId INT = 2;
--DECLARE @IgnoreLocationsBelow INT = 60;
--DECLARE @OrderBy NVARCHAR(20) = 'Date'; -- Date | Duration

WITH Points AS
(
    SELECT  t.[Timestamp],
            t.LocationId,
            rn = ROW_NUMBER() OVER (ORDER BY t.[Timestamp], t.ID)
    FROM    ProjectIvy.Tracking.Tracking AS t
    WHERE   t.UserId = @UserId
),
Flagged AS
(
    SELECT  p.rn,
            p.[Timestamp],
            p.LocationId,
            IsNewSegment = CASE
                               WHEN ISNULL(LAG(p.LocationId) OVER (ORDER BY p.rn), -1)
                                    = ISNULL(p.LocationId, -1)
                               THEN 0
                               ELSE 1
                           END
    FROM    Points AS p
),
Segments AS
(
    SELECT  f.rn,
            f.[Timestamp],
            f.LocationId,
            SegmentId = SUM(f.IsNewSegment) OVER (ORDER BY f.rn ROWS UNBOUNDED PRECEDING)
    FROM    Flagged AS f
),
Collapsed AS
(
    SELECT  SegmentId,
            LocationId = MIN(LocationId),
            EnterTime  = MIN([Timestamp])
    FROM    Segments
    GROUP BY SegmentId
),
Timeline AS
(
    SELECT  SegmentId,
            LocationId,
            EnterTime,
            ExitTime = LEAD(EnterTime) OVER (ORDER BY SegmentId)
    FROM    Collapsed
),
LocationStays AS
(
    SELECT  SegmentId,
            LocationId,
            EnterTime,
            ExitTime
    FROM    Timeline
    WHERE   LocationId IS NOT NULL
),
Significant AS
(
    SELECT  LocationId,
            EnterTime,
            ExitTime,
            Seq = ROW_NUMBER() OVER (ORDER BY SegmentId)
    FROM    LocationStays
    WHERE   LocationId IN (@FromLocationId, @ToLocationId)
       OR   ExitTime IS NULL
       OR   DATEDIFF(SECOND, EnterTime, ExitTime) >= @IgnoreLocationsBelow
),
Routes AS
(
    SELECT  [Exit]   = prev.ExitTime,
            [Entry]  = curr.EnterTime,
            Duration = DATEDIFF(SECOND, prev.ExitTime, curr.EnterTime)
    FROM    Significant AS curr
    JOIN    Significant AS prev ON prev.Seq = curr.Seq - 1
    WHERE   prev.LocationId = @FromLocationId
      AND   curr.LocationId = @ToLocationId
      AND   prev.ExitTime IS NOT NULL
)
SELECT  [Exit],
        [Entry],
        Duration
FROM    Routes
ORDER BY CASE WHEN @OrderBy = 'Duration' THEN Duration END ASC,
         [Exit] DESC;
