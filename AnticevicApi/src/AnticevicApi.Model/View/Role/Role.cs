using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Role
{
    public class Role
    {
        public Role(DatabaseModel.User.Role x)
        {
            Name = x.Name;
            ValueId = x.ValueId;
        }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}
