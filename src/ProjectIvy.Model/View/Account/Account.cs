namespace ProjectIvy.Model.View.Account
{
    public class Account
    {
        public Account(Database.Main.Finance.Account a)
        {
            Id = a.ValueId;
            Name = a.Name;
            Bank = a.BankId.HasValue ? new Bank.Bank(a.Bank) : null;
            Currency = new Currency.Currency(a.Currency);
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Bank.Bank Bank { get; set; }

        public decimal Balance { get; set; }

        public Currency.Currency Currency { get; set; }
    }
}
