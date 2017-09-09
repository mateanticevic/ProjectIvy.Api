namespace ProjectIvy.DL.Sql
{
    public static class MainSnippets
    {
        private const string Prefix = "ProjectIvy.DL.Sql.Main.";
        private const string Sufix = ".sql";

        public static string GetExpenseSumInDefaultCurrency { get; } = Build(nameof(GetExpenseSumInDefaultCurrency));

        public static string GetIncomeSum { get; } = Build(nameof(GetIncomeSum));


        public static string GetWebTimeSum { get; } = Build(nameof(GetWebTimeSum));

        public static string GetWebTimeTotal { get; } = Build(nameof(GetWebTimeTotal));

        public static string GetWebTimeTotalByDay { get; } = Build(nameof(GetWebTimeTotalByDay));

        private static string Build(string name)
        {
            return Prefix + name + Sufix;
        }
    }
}
