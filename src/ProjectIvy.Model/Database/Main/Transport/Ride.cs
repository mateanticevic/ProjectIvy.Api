using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport;

[Table(nameof(Ride), Schema = nameof(Transport))]
public class Ride : UserEntity
{
    public DateTime DateOfArrival { get; set; }

    public DateTime DateOfDeparture { get; set; }

    public Common.City DestinationCity { get; set; }

    public int? DestinationCityId { get; set; }

    public Travel.Poi DestinationPoi { get; set; }

    public int? DestinationPoiId { get; set; }

    [Key]
    public int Id { get; set; }

    public Common.City OriginCity { get; set; }

    public int? OriginCityId { get; set; }

    public Travel.Poi OriginPoi { get; set; }

    public int? OriginPoiId { get; set; }

    public RideType RideType { get; set; }

    public int RideTypeId { get; set; }
}
