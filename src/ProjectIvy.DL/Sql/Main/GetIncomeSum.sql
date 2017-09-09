--DECLARE @From DATE = '2014-04-01'
--DECLARE @To DATE = '2018-04-30'
--DECLARE @UserId INT = 1
--DECLARE @TargetCurrencyId INT = 45

DECLARE @EURId INT = (SELECT Id FROM Common.Currency WHERE Code = 'EUR')

SELECT
	SUM
	(
		CASE WHEN i.CurrencyId = @TargetCurrencyId THEN i.Ammount
			 WHEN i.CurrencyId = @EURId THEN i.Ammount * crEurToTarget.Rate
			 WHEN @TargetCurrencyId = @EURId THEN i.Ammount * (1/crEurToOrigin.Rate)
			 ELSE i.Ammount * (1/crEurToOrigin.Rate) * crEurToTarget.Rate
		END
	)
FROM Finance.Income i
LEFT JOIN Common.CurrencyRate crEurToTarget ON crEurToTarget.FromCurrencyId = @EURId
									AND crEurToTarget.ToCurrencyId = @TargetCurrencyId
									AND crEurToTarget.Timestamp = (SELECT TOP 1 x.Timestamp FROM Common.CurrencyRate x WHERE x.FromCurrencyId = @EURId AND x.ToCurrencyId = @TargetCurrencyId AND x.Timestamp <= i.Timestamp ORDER BY x.Timestamp DESC)
LEFT JOIN Common.CurrencyRate crEurToOrigin ON crEurToOrigin.FromCurrencyId = @EURId
									AND crEurToOrigin.ToCurrencyId = i.CurrencyId
									AND crEurToOrigin.Timestamp = (SELECT TOP 1 x.Timestamp FROM Common.CurrencyRate x WHERE x.FromCurrencyId = @EURId AND x.ToCurrencyId = i.CurrencyId AND x.Timestamp <= i.Timestamp ORDER BY x.Timestamp DESC)
WHERE i.UserId = @UserId
AND (@From IS NULL OR @From <= i.Timestamp)
AND (@To IS NULL OR @To >= i.Timestamp)