using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;
using System;
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
                    ValueId = context.ToDos.NextValueId(UserId.Value).ToString(),
                    Name = name,
                    UserId = UserId.Value,
                    Created = DateTime.Now
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
                return context.ToDos.WhereUser(UserId.Value)
                                    .WhereIf(binding.IsDone.HasValue, x => x.IsDone == binding.IsDone.Value)
                                    .OrderByDescending(x => x.Created)
                                    .Select(x => new View.ToDo(x))
                                    .ToPagedView(binding);
            }
        }

        public void SetDone(string valueId)
        {
            using (var context = GetMainContext())
            {
                int id = context.ToDos.WhereUser(UserId.Value).GetId(valueId).Value;

                var todo = context.ToDos.Find(id);
                todo.IsDone = true;

                context.SaveChanges();
            }
        }
    }
}
