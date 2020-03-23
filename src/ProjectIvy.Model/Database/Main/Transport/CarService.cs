using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport
{
    [Table(nameof(CarService), Schema = nameof(Transport))]
    public class CarService
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public int CarId { get; set; }

        public int CarServiceTypeId { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public Car Car { get; set; }

        public CarServiceType CarServiceType { get; set; }
    }
}
