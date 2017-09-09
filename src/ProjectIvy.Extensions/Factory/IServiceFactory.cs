namespace ProjectIvy.Extensions.Factory
{
    public interface IServiceFactory<T> where T : class
    {
        T Build();
    }
}
