--DECLARE @From DATETIME2(3) = '2026-07-01';
--DECLARE @To DATETIME2(3) = '2026-08-01';
--DECLARE @UserId INT = 1;
DECLARE @MergeGapMinutes INT = 10;
DECLARE @MinStayMinutes INT = 3;
WITH Points AS
(
    SELECT  t.[Timestamp],
            t.LocationId,
            rn = ROW_NUMBER() OVER (ORDER BY t.[Timestamp], t.ID)
    FROM    ProjectIvy.Tracking.Tracking AS t
    WHERE   t.UserId      = @UserId
      AND   t.[Timestamp] >= @From
      AND   t.[Timestamp] <  @To
),
Flagged AS
(
    SELECT  p.*,
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
    SELECT  f.*,
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
    SELECT  LocationId,
            EnterTime,
            ExitTime = LEAD(EnterTime) OVER (ORDER BY EnterTime)
    FROM    Collapsed
),
Stays AS
(
    SELECT  LocationId,
            EnterTime,
            ExitTime
    FROM    Timeline
    WHERE   LocationId IS NOT NULL
),
Gapped AS
(
    SELECT  s.*,
            IsNewStay = CASE
                            WHEN LAG(s.LocationId) OVER (ORDER BY s.EnterTime) = s.LocationId
                             AND DATEDIFF(SECOND,
                                          LAG(s.ExitTime) OVER (ORDER BY s.EnterTime),
                                          s.EnterTime) < @MergeGapMinutes * 60
                            THEN 0
                            ELSE 1
                        END
    FROM    Stays AS s
),
Merged AS
(
    SELECT  g.*,
            StayId = SUM(g.IsNewStay) OVER (ORDER BY g.EnterTime ROWS UNBOUNDED PRECEDING)
    FROM    Gapped AS g
)
SELECT  LocationId = MIN(LocationId),
        EnterTime  = MIN(EnterTime),
        ExitTime   = CASE
                         WHEN COUNT(ExitTime) < COUNT(*) THEN NULL
                         ELSE MAX(ExitTime)
                     END
FROM    Merged
GROUP BY StayId
HAVING  COUNT(ExitTime) < COUNT(*)
   OR   DATEDIFF(SECOND, MIN(EnterTime), MAX(ExitTime)) >= @MinStayMinutes * 60
ORDER BY MIN(EnterTime);