﻿using ProjectIvy.Model.Binding.Common;

namespace ProjectIvy.Model.Binding.Poi;

public class PoiGetBinding : PagedBinding
{
    public string CategoryId { get; set; }

    public string Name { get; set; }

    public string VendorId { get; set; }

    public LocationBinding X { get; set; }

    public LocationBinding Y { get; set; }

    public string Search { get; set; }
}
