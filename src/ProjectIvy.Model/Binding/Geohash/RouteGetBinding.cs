using System;

namespace ProjectIvy.Model.Binding.Geohash;

public class RouteGetBinding
{
	public IEnumerable<string> From { get; set; }

	public IEnumerable<string> To { get; set; }

	public RouteTimeSort OrderBy { get; set; }
}
