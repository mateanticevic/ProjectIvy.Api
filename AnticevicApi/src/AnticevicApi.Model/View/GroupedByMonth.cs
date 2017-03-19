namespace AnticevicApi.Model.View
{
    public class GroupedByMonth<T>
    {
        public GroupedByMonth()
        {

        }

        public GroupedByMonth(T data, int month, int year)
        {
            Data = data;
            Month = month;
            Year = year;
        }

        public T Data { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
