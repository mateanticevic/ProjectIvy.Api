﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.Database.Main.Beer;

[Table(nameof(BeerBrand), Schema = nameof(Beer))]
public class BeerBrand : IHasName, IHasValueId
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public int? CountryId { get; set; }

    public ICollection<Beer> Beers { get; set; }

    public Country Country { get; set; }
}
