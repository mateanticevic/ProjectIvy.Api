using ProjectIvy.Model.Binding.User;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Business.Handlers.User
{
    public interface IUserHandler : IHandler
    {
        View.User Get(string username);

        View.User Get(int? id = null);

        Task SetWeight(decimal weight);

        Task Update(UserUpdateBinding binding);
    }
}
