using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Vendor
{
    public class Vendor
    {
        public Vendor(DatabaseModel.Finance.Vendor x)
        {
            ValueId = x.ValueId;
            Name = x.Name;
        }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}
