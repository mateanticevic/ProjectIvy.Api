using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnticevicApi.Model.Binding.Task
{
    public class TaskBinding
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string PriorityId { get; set; }
        public string ProjectId { get; set; }
        public string TypeId { get; set; }
    }
}
