using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Transport
{
    [Table("Airport", Schema = "Transport")]
    public class Airport
    {
        [Key]
        public int Id { get; set; }

        public string IATA { get; set; }

        public string Name { get; set; }
    }
}
