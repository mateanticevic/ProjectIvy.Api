namespace ProjectIvy.Data.Sql.Main.Scripts
{
    public static class Constants
    {
        private const string Prefix = "ProjectIvy.Data.Sql.Main.Scripts.";
        private const string Sufix = ".sql";

        public static string GetConsumationSumByDayOfWeek { get; } = Build(nameof(GetConsumationSumByDayOfWeek));

        public static string GetExpenseSumInDefaultCurrency { get; } = Build(nameof(GetExpenseSumInDefaultCurrency));

        public static string GetIncomeSum { get; } = Build(nameof(GetIncomeSum));

        public static string GetWebTimeSum { get; } = Build(nameof(GetWebTimeSum));

        public static string GetWebTimeTotal { get; } = Build(nameof(GetWebTimeTotal));

        public static string GetWebTimeTotalByDay { get; } = Build(nameof(GetWebTimeTotalByDay));

        public static string GetWebTimeTotalByMonth { get; } = Build(nameof(GetWebTimeTotalByMonth));

        public static string GetWebTimeTotalByYear { get; } = Build(nameof(GetWebTimeTotalByYear));

        private static string Build(string name)
        {
            return Prefix + name + Sufix;
        }
    }
}
