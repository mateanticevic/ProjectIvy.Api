namespace ProjectIvy.Common.Interfaces
{
    public interface IServiceFactory<T> where T : class
    {
        T Build();
    }
}
