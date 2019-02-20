namespace ProjectIvy.Business.Handlers.Security
{
    public interface ISecurityHandler
    {
        Model.Database.Main.User.User GetUser(string token);

        string CreateToken(string username, string password);
    }
}
