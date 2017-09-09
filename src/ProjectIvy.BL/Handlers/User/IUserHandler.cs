using ProjectIvy.Model.Binding.User;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.BL.Handlers.User
{
    public interface IUserHandler : IHandler
    {
        View.User Get(string username);

        View.User Get(int? id = null);

        void SetPassword(PasswordSetBinding binding);
    }
}
