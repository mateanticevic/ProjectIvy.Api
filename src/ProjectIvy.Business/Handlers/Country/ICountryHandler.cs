﻿using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Country;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Country;

namespace ProjectIvy.Business.Handlers.Country
{
    public interface ICountryHandler
    {
        long Count(CountryGetBinding binding);

        long CountVisited();

        View.Country Get(string id);

        PagedView<View.Country> Get(CountryGetBinding binding);

        IEnumerable<View.CountryBoundaries> GetBoundaries(IEnumerable<View.Country> countries);

        Task<PagedView<Model.View.City.City>> GetCities(string countryValueId, FilteredPagedBinding binding);

        Task<IEnumerable<KeyValuePair<View.Country, int>>> GetDaysInCountry();

        Task<IEnumerable<View.CountryList>> GetLists();

        Task<IEnumerable<View.CountryListVisited>> GetListsVisited();

        Task<IEnumerable<View.Country>> GetVisited(TripGetBinding binding);

        Task<IEnumerable<KeyValuePair<int, IEnumerable<View.Country>>>> GetVisitedByYear();

        Task<IEnumerable<KeyValuePair<int, int>>> GetVisitedCountByYear();

        Task<IEnumerable<KeyValuePair<DateTime, IEnumerable<string>>>> GetCountriesByDay(FilteredBinding binding);
    }
}
