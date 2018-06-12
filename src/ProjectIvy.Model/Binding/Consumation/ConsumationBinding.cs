using System;

namespace ProjectIvy.Model.Binding.Consumation
{
    public class ConsumationBinding
    {
        public string BeerId { get; set; }

        public string ServingId { get; set; }

        public DateTime Date { get; set; }

        public int Volume { get; set; }
    }
}
