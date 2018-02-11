namespace ProjectIvy.Model.View
{
    public class CountBy<T>
    {
        public CountBy(T by, long count)
        {
            By = by;
            Count = count;
        }

        public T By { get; set; }

        public long Count { get; set; }
    }
}
