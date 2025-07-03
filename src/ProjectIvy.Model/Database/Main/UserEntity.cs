namespace ProjectIvy.Model.Database.Main;

public abstract class UserEntity
{
    public int UserId { get; set; }

    public User.User User { get; set; }
}
