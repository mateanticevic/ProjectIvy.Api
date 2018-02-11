namespace ProjectIvy.Model.View
{
    public class SumBy<T>
    {
        public SumBy(T by, long sum)
        {
            By = by;
            Sum = sum;
        }

        public T By { get; set; }

        public long Sum { get; set; }
    }
}
