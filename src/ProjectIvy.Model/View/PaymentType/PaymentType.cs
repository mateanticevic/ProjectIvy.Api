using DatabaseModel = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.View.PaymentType
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
