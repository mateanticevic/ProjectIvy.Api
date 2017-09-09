namespace ProjectIvy.Model.Binding.ExpenseType
{
    public class ExpenseTypeGetBinding
    {
        public bool? HasChildren { get; set; }

        public bool? HasParent { get; set; }

        public string ParentId { get; set; }
    }
}
