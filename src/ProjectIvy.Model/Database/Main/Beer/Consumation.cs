using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Beer;

[Table(nameof(Consumation), Schema = nameof(Beer))]
public class Consumation : UserEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int BeerId { get; set; }

    public int BeerServingId { get; set; }

    public int Volume { get; set; }

    public Beer Beer { get; set; }

    public BeerServing BeerServing { get; set; }
}
