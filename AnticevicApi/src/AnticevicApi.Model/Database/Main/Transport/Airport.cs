using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Transport
{
    [Table("Airport", Schema = "Transport")]
    public class Airport
    {
        [Key]
        public int Id { get; set; }

        public string IATA { get; set; }

        public string Name { get; set; }

        public int CityId { get; set; }

        public Common.City City { get; set; }
    }
}
