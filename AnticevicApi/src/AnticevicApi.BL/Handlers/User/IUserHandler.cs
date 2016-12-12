using View = AnticevicApi.Model.View.User;

namespace AnticevicApi.BL.Handlers.User
{
    public interface IUserHandler : IHandler
    {
        View.User Get(string username);

        View.User Get(int? id = null);
    }
}
