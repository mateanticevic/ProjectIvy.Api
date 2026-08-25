--DECLARE @From DATETIME2(3) = '2026-07-01';
--DECLARE @To DATETIME2(3) = '2026-08-01';
--DECLARE @UserId INT = 1;
DECLARE @MergeGapMinutes INT = 10;
DECLARE @MinStayMinutes INT = 3;
WITH Points AS
(
    SELECT  t.[Timestamp],
            t.LocationId,
            t.CityId,
            rn = ROW_NUMBER() OVER (ORDER BY t.[Timestamp], t.ID)
    FROM    ProjectIvy.Tracking.Tracking AS t
    WHERE   t.UserId      = @UserId
      AND   t.[Timestamp] >= @From
      AND   t.[Timestamp] <  @To
),
LocationFlagged AS
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
LocationSegments AS
(
    SELECT  f.*,
            SegmentId = SUM(f.IsNewSegment) OVER (ORDER BY f.rn ROWS UNBOUNDED PRECEDING)
    FROM    LocationFlagged AS f
),
LocationCollapsed AS
(
    SELECT  SegmentId,
            LocationId = MIN(LocationId),
            EnterTime  = MIN([Timestamp])
    FROM    LocationSegments
    GROUP BY SegmentId
),
LocationTimeline AS
(
    SELECT  LocationId,
            EnterTime,
            ExitTime = LEAD(EnterTime) OVER (ORDER BY EnterTime)
    FROM    LocationCollapsed
),
LocationStays AS
(
    SELECT  LocationId,
            EnterTime,
            ExitTime
    FROM    LocationTimeline
    WHERE   LocationId IS NOT NULL
),
LocationGapped AS
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
    FROM    LocationStays AS s
),
LocationMerged AS
(
    SELECT  g.*,
            StayId = SUM(g.IsNewStay) OVER (ORDER BY g.EnterTime ROWS UNBOUNDED PRECEDING)
    FROM    LocationGapped AS g
),
LocationVisits AS
(
    SELECT  LocationId = MIN(LocationId),
            EnterTime  = MIN(EnterTime),
            ExitTime   = CASE
                             WHEN COUNT(ExitTime) < COUNT(*) THEN NULL
                             ELSE MAX(ExitTime)
                         END
    FROM    LocationMerged
    GROUP BY StayId
    HAVING  COUNT(ExitTime) < COUNT(*)
       OR   DATEDIFF(SECOND, MIN(EnterTime), MAX(ExitTime)) >= @MinStayMinutes * 60
),
CityFlagged AS
(
    SELECT  p.rn,
            p.[Timestamp],
            p.CityId,
            IsNewSegment = CASE
                               WHEN ISNULL(LAG(p.CityId) OVER (ORDER BY p.rn), -1)
                                    = ISNULL(p.CityId, -1)
                               THEN 0
                               ELSE 1
                           END
    FROM    Points AS p
),
CitySegments AS
(
    SELECT  f.*,
            SegmentId = SUM(f.IsNewSegment) OVER (ORDER BY f.rn ROWS UNBOUNDED PRECEDING)
    FROM    CityFlagged AS f
),
CityCollapsed AS
(
    SELECT  SegmentId,
            CityId    = MIN(CityId),
            EnterTime = MIN([Timestamp])
    FROM    CitySegments
    GROUP BY SegmentId
),
CityTimeline AS
(
    SELECT  CityId,
            EnterTime,
            ExitTime = LEAD(EnterTime) OVER (ORDER BY EnterTime)
    FROM    CityCollapsed
),
CityStays AS
(
    SELECT  CityId,
            EnterTime,
            ExitTime
    FROM    CityTimeline
    WHERE   CityId IS NOT NULL
),
CityGapped AS
(
    SELECT  s.*,
            IsNewStay = CASE
                            WHEN LAG(s.CityId) OVER (ORDER BY s.EnterTime) = s.CityId
                             AND DATEDIFF(SECOND,
                                          LAG(s.ExitTime) OVER (ORDER BY s.EnterTime),
                                          s.EnterTime) < @MergeGapMinutes * 60
                            THEN 0
                            ELSE 1
                        END
    FROM    CityStays AS s
),
CityMerged AS
(
    SELECT  g.*,
            StayId = SUM(g.IsNewStay) OVER (ORDER BY g.EnterTime ROWS UNBOUNDED PRECEDING)
    FROM    CityGapped AS g
),
CityVisits AS
(
    SELECT  CityId    = MIN(CityId),
            EnterTime = MIN(EnterTime),
            ExitTime  = CASE
                            WHEN COUNT(ExitTime) < COUNT(*) THEN NULL
                            ELSE MAX(ExitTime)
                        END
    FROM    CityMerged
    GROUP BY StayId
    HAVING  COUNT(ExitTime) < COUNT(*)
       OR   DATEDIFF(SECOND, MIN(EnterTime), MAX(ExitTime)) >= @MinStayMinutes * 60
),
Visits AS
(
    SELECT  LocationId,
            CityId    = CAST(NULL AS INT),
            EnterTime,
            ExitTime
    FROM    LocationVisits
    UNION ALL
    SELECT  LocationId = CAST(NULL AS INT),
            CityId,
            EnterTime,
            ExitTime   = CAST(NULL AS DATETIME2(3))
    FROM    CityVisits
    UNION ALL
    SELECT  LocationId = CAST(NULL AS INT),
            CityId,
            EnterTime  = CAST(NULL AS DATETIME2(3)),
            ExitTime
    FROM    CityVisits
    WHERE   ExitTime IS NOT NULL
)
SELECT  LocationId,
        CityId,
        EnterTime,
        ExitTime
FROM    Visits
ORDER BY ISNULL(EnterTime, ExitTime);
