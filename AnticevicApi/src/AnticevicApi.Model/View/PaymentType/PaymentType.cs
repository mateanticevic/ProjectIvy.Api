using DatabaseModel = AnticevicApi.Model.Database.Main.Finance;

namespace AnticevicApi.Model.View.PaymentType
{
    public class PaymentType
    {
        public PaymentType(DatabaseModel.PaymentType x)
        {
            Id = x.ValueId;
            Name = x.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
