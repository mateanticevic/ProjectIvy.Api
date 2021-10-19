--DECLARE @From DATE = NULl
--DECLARE @To DATE = NULL
--DECLARE @Month INT = NULL
--DECLARE @UserId.Value INT = 1
--DECLARE @TargetCurrencyId INT = 45
--DECLARE @ExpenseTypeValueId NVARCHAR(MAX) = NULL
--DECLARE @VendorValueId NVARCHAR(MAX) = NULL
--DECLARE @ExpenseIds dbo.IntList
--INSERT INTO @ExpenseIds (Value) VALUES (50)
--INSERT INTO @ExpenseIds (Value) VALUES (51)

DECLARE @EURId INT = (SELECT Id FROM Common.Currency WHERE Code = 'EUR')

SELECT
	--  Original--Parent--Target
	--01   X       NULL     X   
	--02   X       NULL     Y
	--03   X       NULL    EUR
	--04  EUR      NULL    EUR
	--05  EUR      NULL     X
	--06   X       EUR      Y
	--07   X       EUR     EUR
	--08   X	    Y      EUR
	--09   X        Y       X
	--10   X        Y       Y
	--11   X        Y		Z
	--12  EUR       X      EUR
	--13  EUR       X       Y
	--14  EUR       X       X
	SUM(
		CASE WHEN e.ParentCurrencyId IS NULL THEN
			CASE WHEN e.CurrencyId = @EURId THEN
				CASE WHEN @TargetCurrencyId = @EURId THEN e.Amount --04
				ELSE e.Amount * crEurToTarget.Rate --05
				END
			ELSE
				CASE WHEN @TargetCurrencyId = @EURId THEN e.Amount * (1/crEurToOriginal.Rate) --03
				WHEN e.CurrencyId = @TargetCurrencyId THEN e.Amount --01
				ELSE e.Amount * (1/crEurToOriginal.Rate) * crEurToTarget.Rate --02
				END
			END
		WHEN e.ParentCurrencyId = @EURId THEN
			CASE WHEN @TargetCurrencyId = @EURId THEN e.Amount * e.ParentCurrencyExchangeRate --07
			ELSE e.Amount * e.ParentCurrencyExchangeRate * crEurToTarget.Rate --06
			END
		ELSE
			CASE WHEN e.CurrencyId = @EURId THEN
				CASE WHEN @TargetCurrencyId = @EURId THEN e.Amount * e.ParentCurrencyExchangeRate * (1/crEurToParent.Rate) --12
				WHEN e.ParentCurrencyId = @TargetCurrencyId THEN e.Amount * e.ParentCurrencyExchangeRate --14
				ELSE e.Amount * e.ParentCurrencyExchangeRate * (1/crEurToParent.Rate) * crEurToTarget.Rate --13
				END
			ELSE
				CASE WHEN @TargetCurrencyId = @EURId THEN e.Amount * e.ParentCurrencyExchangeRate * (1/crEurToParent.Rate) --08
				WHEN e.ParentCurrencyId = @TargetCurrencyId THEN e.Amount * e.ParentCurrencyExchangeRate --10
				ELSE e.Amount * e.ParentCurrencyExchangeRate * (1/crEurToParent.Rate) * crEurToTarget.Rate --09 11
				END
			END
		END
	)
FROM Finance.Expense e
JOIN Finance.ExpenseType et ON e.ExpenseTypeId = et.Id
LEFT JOIN Finance.Vendor v ON v.Id = e.VendorId
LEFT JOIN Common.CurrencyRate crEurToTarget ON crEurToTarget.FromCurrencyId = @EURId
							   AND crEurToTarget.ToCurrencyId = @TargetCurrencyId
							   AND crEurToTarget.Timestamp = (SELECT TOP 1 Timestamp FROM Common.CurrencyRate cr2 WHERE cr2.FromCurrencyId = @EURId AND cr2.ToCurrencyId = @TargetCurrencyId AND cr2.Timestamp <= e.Date ORDER BY Timestamp DESC)
LEFT JOIN Common.CurrencyRate crEurToOriginal ON crEurToOriginal.FromCurrencyId = @EURId
							   AND crEurToOriginal.ToCurrencyId = e.CurrencyId
							   AND crEurToOriginal.Timestamp = (SELECT TOP 1 Timestamp FROM Common.CurrencyRate cr3 WHERE cr3.FromCurrencyId = @EURId AND cr3.ToCurrencyId = e.CurrencyId AND cr3.Timestamp <= e.Date ORDER BY Timestamp DESC)
LEFT JOIN Common.CurrencyRate crEurToParent ON crEurToParent.FromCurrencyId = @EURId
							   AND crEurToParent.ToCurrencyId = e.ParentCurrencyId
							   AND crEurToParent.Timestamp = (SELECT TOP 1 Timestamp FROM Common.CurrencyRate cr4 WHERE cr4.FromCurrencyId = @EURId AND cr4.ToCurrencyId = e.ParentCurrencyId AND cr4.Timestamp <= e.Date ORDER BY Timestamp DESC)
WHERE 1=1
	AND ISNULL(@ExpenseTypeValueId, et.ValueId) = et.ValueId
	AND (ISNULL(@VendorValueId, v.ValueId) = v.ValueId OR (v.ValueId IS NULL AND @VendorValueId IS NULL))
	AND (@From IS NULL OR e.[Date] >= @From)
	AND (@To IS NULL OR e.[Date] <= @To)
    AND (@Month IS NULL OR MONTH(e.[Date]) = @Month)
	AND (NOT EXISTS (SELECT TOP 1 * FROM @ExpenseIds) OR EXISTS(SELECT TOP 1 * FROM @ExpenseIds WHERE [Value] = e.Id))
	AND ISNULL(@UserId.Value, e.UserId.Value) = e.UserId.Value