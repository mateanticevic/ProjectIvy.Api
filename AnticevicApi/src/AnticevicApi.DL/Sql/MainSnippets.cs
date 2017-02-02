namespace AnticevicApi.DL.Sql
{
    public static class MainSnippets
    {
        private const string _prefix = "AnticevicApi.DL.Sql.Main.";
        private const string _sufix = ".sql";

        public static string GetExpenseSumInDefaultCurrency = Build(nameof(GetExpenseSumInDefaultCurrency));
        public static string GetWebTimeSum = Build(nameof(GetWebTimeSum));
        public static string GetWebTimeTotal = Build(nameof(GetWebTimeTotal));
        public static string GetWebTimeTotalByDay = Build(nameof(GetWebTimeTotalByDay));

        private static string Build(string name)
        {
            return _prefix + name + _sufix;
        }
    }
}
