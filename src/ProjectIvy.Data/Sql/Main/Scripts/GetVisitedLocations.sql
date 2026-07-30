--DECLARE @From DATETIME2(3) = '2026-07-01';
--DECLARE @To DATETIME2(3) = '2026-08-01';
--DECLARE @UserId INT = 1;
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
             EnterTime  = MIN([Timestamp]),
             LastSeen   = MAX([Timestamp])
     FROM    Segments
     GROUP BY SegmentId
 ),
 Timeline AS
 (
     SELECT  LocationId,
             EnterTime,
             ExitTime = COALESCE(LEAD(EnterTime) OVER (ORDER BY EnterTime), LastSeen)
     FROM    Collapsed
 )
 SELECT  LocationId,
         EnterTime,
         ExitTime
 FROM    Timeline
 WHERE   LocationId IS NOT NULL
 ORDER BY EnterTime;