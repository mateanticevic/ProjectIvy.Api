using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;
using System.Linq;
using Db = ProjectIvy.Model.Database.Main;
using View = ProjectIvy.Model.View.ToDo;

namespace ProjectIvy.Business.Handlers.ToDo
{
    public class ToDoHandler : Handler<ToDoHandler>, IToDoHandler
    {
        public ToDoHandler(IHandlerContext<ToDoHandler> context) : base(context)
        {
        }

        public string Create(string name)
        {
            using (var context = GetMainContext())
            {
                var toDoEntity = new Db.Org.ToDo()
                {
                    ValueId = context.ToDos.NextValueId(User.Id).ToString(),
                    Name = name,
                    UserId = User.Id
                };
                context.ToDos.Add(toDoEntity);
                context.SaveChanges();

                return toDoEntity.ValueId;
            }
        }

        public PagedView<View.ToDo> GetPaged(ToDoGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.ToDos.WhereUser(User)
                                    .WhereIf(binding.IsDone.HasValue, x => x.IsDone == binding.IsDone.Value)
                                    .OrderBy(x => x.Created)
                                    .Select(x => new View.ToDo(x))
                                    .ToPagedView(binding);
            }
        }
    }
}
