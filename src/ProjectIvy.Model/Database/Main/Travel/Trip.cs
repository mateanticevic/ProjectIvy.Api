﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(Trip), Schema = nameof(Travel))]
public class Trip : UserEntity, IHasName, IHasValueId, IHasCreatedModified
{
    public Trip()
    {
        Created = DateTime.Now;
        Modified = DateTime.Now;
        Cities = new List<Common.City>();
    }

    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public DateTime TimestampEnd { get; set; }

    public DateTime TimestampStart { get; set; }

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }

    public bool IsDomestic { get; set; }

    public ICollection<Common.City> Cities { get; set; }

    public ICollection<Storage.File> Files { get; set; }

    public ICollection<TripPoi> Pois { get; set; }
}
