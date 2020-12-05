using ProjectIvy.Model.Binding.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Business.Handlers.User
{
    public interface IUserHandler : IHandler
    {
        Task CloseSession(long id);

        View.User Get(string username);

        View.User Get(int? id = null);

        Task<IEnumerable<View.UserSession>> GetSessions();

        void SetPassword(PasswordSetBinding binding);

        Task SetWeight(decimal weight);
    }
}
