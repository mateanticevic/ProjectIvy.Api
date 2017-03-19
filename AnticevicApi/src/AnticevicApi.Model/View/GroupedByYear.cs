namespace AnticevicApi.Model.View
{
    public class GroupedByYear<T>
    {
        public GroupedByYear()
        {

        }

        public GroupedByYear(T data, int year)
        {
            Data = data;
            Year = year;
        }

        public T Data { get; set; }
        public int Year { get; set; }
    }
}
