using System.Collections.Generic;
using ProjectIvy.Model.View.Call;

namespace ProjectIvy.Business.Handlers.Call
{
    public interface ICallHandler
    {
        IEnumerable<Model.View.Call.Call> Get();
    }
}