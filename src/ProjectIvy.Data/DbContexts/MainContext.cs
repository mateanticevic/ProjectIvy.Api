using Microsoft.EntityFrameworkCore;
using ProjectIvy.Model.Database.Main.Beer;
using ProjectIvy.Model.Database.Main.Common;
using ProjectIvy.Model.Database.Main.Contacts;
using ProjectIvy.Model.Database.Main.Finance;
using ProjectIvy.Model.Database.Main.Net;
using ProjectIvy.Model.Database.Main.Storage;
using ProjectIvy.Model.Database.Main.Tracking;
using ProjectIvy.Model.Database.Main.Transport;
using ProjectIvy.Model.Database.Main.Travel;
using ProjectIvy.Model.Database.Main.User;

namespace ProjectIvy.Data.DbContexts
{
    public class MainContext : DbContext
    {
        public MainContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Airline> Airlines { get; set; }

        public DbSet<Airport> Airports { get; set; }

        public DbSet<Beer> Beers { get; set; }

        public DbSet<BeerBrand> BeerBrands { get; set; }

        public DbSet<BeerServing> BeerServings { get; set; }

        public DbSet<BeerStyle> BeerStyles { get; set; }

        public DbSet<Call> Calls { get; set; }

        public DbSet<CallBlacklist> CallBlacklist { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<CarModel> CarModels { get; set; }

        public DbSet<CarService> CarServices { get; set; }

        public DbSet<CarServiceInterval> CarServiceIntervals { get; set; }

        public DbSet<CarServiceType> CarServiceTypes { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<CarFuel> CarFuelings { get; set; }

        public DbSet<CarLog> CarLogs { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<CityAccessGeohash> CityAccessGeohashes { get; set; }

        public DbSet<CityVisited> CitiesVisited { get; set; }

        public DbSet<Consumation> Consumations { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<ContactType> ContactTypes { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<CountryIpRange> CountryIpRanges { get; set; }

        public DbSet<CountryList> CountryLists { get; set; }

        public DbSet<CountryPolygon> CountryPolygons { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<ExpenseFile> ExpenseFiles { get; set; }

        public DbSet<ExpenseFileType> ExpenseFileTypes { get; set; }

        public DbSet<ExpenseType> ExpenseTypes { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<FileType> FileTypes { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<GeohashCity> GeohashCities { get; set; }

        public DbSet<GeohashCountry> GeohashCountries { get; set; }

        public DbSet<Holiday> Holidays { get; set; }

        public DbSet<Income> Incomes { get; set; }

        public DbSet<IncomeSource> IncomeSources { get; set; }

        public DbSet<IncomeType> IncomeTypes { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<LocationType> LocationTypes { get; set; }

        public DbSet<LocationGeohash> LocationGeohashes { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<PaymentProvider> PaymentProviders { get; set; }

        public DbSet<PaymentProviderAccount> PaymentProviderAccounts { get; set; }

        public DbSet<PaymentType> PaymentTypes { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<Poi> Pois { get; set; }

        public DbSet<PoiCategory> PoiCategories { get; set; }

        public DbSet<Ride> Rides { get; set; }

        public DbSet<Route> Routes { get; set; }

        public DbSet<RoutePoint> RoutePoints { get; set; }

        public DbSet<RideType> RideTypes { get; set; }

        public DbSet<Tracking> Trackings { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<TripPoi> TripPois { get; set; }

        public DbSet<TripExpenseExclude> TripExpensesExcluded { get; set; }

        public DbSet<TripExpenseInclude> TripExpensesIncluded { get; set; }

        public DbSet<TrackingDistance> TrackingDistances { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<WorkDay> WorkDays { get; set; }

        public DbSet<WorkDayType> WorkDayTypes { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<VendorPoi> VendorPois { get; set; }

        public DbSet<Weight> Weights { get; set; }

        public string ConnectionString { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trip>()
                        .HasMany(p => p.Cities)
                        .WithMany(p => p.Trips)
                        .UsingEntity<CityVisited>(
                            j => j
                                .HasOne(cv => cv.City)
                                .WithMany()
                                .HasForeignKey(cv => cv.CityId),
                            j => j
                                .HasOne(cv => cv.Trip)
                                .WithMany()
                                .HasForeignKey(cv => cv.TripId),
                            j =>
                            {
                                j.Property(cv => cv.Timestamp);
                                j.HasKey(cv => cv.Id);
                            });

            modelBuilder.Entity<Trip>()
                        .HasMany(p => p.Files)
                        .WithMany(p => p.Trips)
                        .UsingEntity<TripFile>(
                            j => j
                                .HasOne(tf => tf.File)
                                .WithMany()
                                .HasForeignKey(tf => tf.FileId),
                            j => j
                                .HasOne(tf => tf.Trip)
                                .WithMany()
                                .HasForeignKey(tf => tf.TripId),
                            j =>
                            {
                                j.Property(tf => tf.Name);
                                j.HasKey(tf => new { tf.TripId, tf.FileId });
                            });

            modelBuilder.Entity<Airport>()
                        .HasOne(x => x.City)
                        .WithMany(x => x.Airports);

            modelBuilder.Entity<Airport>()
                        .HasOne(x => x.Poi);

            modelBuilder.Entity<Beer>()
                        .HasOne(x => x.BeerBrand)
                        .WithMany(x => x.Beers);

            modelBuilder.Entity<Consumation>()
                        .HasOne(x => x.Beer);

            modelBuilder.Entity<Consumation>()
                        .HasOne(x => x.BeerServing);

            modelBuilder.Entity<Car>()
                        .HasOne(x => x.User);

            modelBuilder.Entity<CarServiceInterval>()
                        .HasKey(x => new { x.CarModelId, x.CarServiceTypeId });

            modelBuilder.Entity<City>()
                        .HasOne(x => x.Country)
                        .WithMany(x => x.Cities);

            modelBuilder.Entity<CarLog>()
                        .HasOne(x => x.Car)
                        .WithMany(x => x.CarLogs);

            modelBuilder.Entity<CarService>()
                        .HasOne(x => x.Car)
                        .WithMany(x => x.CarServices);

            modelBuilder.Entity<CarService>()
                        .HasOne(x => x.CarServiceType)
                        .WithMany(x => x.CarServices);

            modelBuilder.Entity<CountryPolygon>()
                        .HasKey(x => new { x.CountryId, x.GroupId, x.Index });

            modelBuilder.Entity<CountryList>()
                        .HasMany(x => x.Countries);

            modelBuilder.Entity<CountryListCountry>()
                        .HasKey(x => new { x.CountryId, x.CountryListId });

            modelBuilder.Entity<Tracking>()
                        .HasOne(x => x.User)
                        .WithMany(x => x.Trackings);

            modelBuilder.Entity<Tracking>()
                        .Property(x => x.Latitude)
                        .HasColumnType("numeric(9, 6)");

            modelBuilder.Entity<Tracking>()
                        .Property(x => x.Longitude)
                        .HasColumnType("numeric(9, 6)");

            modelBuilder.Entity<Tracking>()
                        .Property(x => x.Timestamp)
                        .HasColumnType("datetime2(3)");

            modelBuilder.Entity<Expense>()
                        .Property(x => x.ParentCurrencyExchangeRate)
                        .HasPrecision(22, 10);

            modelBuilder.Entity<ExpenseType>()
                        .HasOne(x => x.ParentType)
                        .WithMany(x => x.Children);

            modelBuilder.Entity<ExpenseFile>()
                        .HasOne(x => x.ExpenseFileType);

            modelBuilder.Entity<ExpenseFile>()
                        .HasKey(x => new { x.ExpenseId, x.FileId });

            modelBuilder.Entity<ExpenseFile>()
                        .HasOne(x => x.Expense)
                        .WithMany(x => x.ExpenseFiles);

            modelBuilder.Entity<ExpenseFile>()
                        .HasOne(x => x.File)
                        .WithMany(x => x.ExpenseFiles);

            modelBuilder.Entity<File>()
                        .HasOne(x => x.FileType);

            modelBuilder.Entity<Vendor>()
                        .HasOne(x => x.City)
                        .WithMany(x => x.Vendors);

            modelBuilder.Entity<VendorPoi>()
                        .HasKey(x => new { x.PoiId, x.VendorId });

            modelBuilder.Entity<TrackingDistance>()
                        .HasKey(x => new { x.Timestamp, x.UserId });

            modelBuilder.Entity<User>()
                        .HasOne(x => x.BirthCity);

            modelBuilder.Entity<User>()
                        .HasOne(x => x.DefaultCar)
                        .WithOne(x => x.User)
                        .HasForeignKey<User>(x => x.DefaultCarId);

            modelBuilder.Entity<Poi>()
                        .HasOne(x => x.PoiCategory)
                        .WithMany(x => x.Pois);

            modelBuilder.Entity<Poi>()
                        .HasOne(x => x.PoiCategory)
                        .WithMany(x => x.Pois);

            modelBuilder.Entity<Poi>()
                        .HasMany(x => x.VendorPois)
                        .WithOne(x => x.Poi);

            modelBuilder.Entity<TripPoi>()
                        .HasKey(x => new { x.PoiId, x.TripId });

            modelBuilder.Entity<TripExpenseExclude>()
                        .HasKey(x => new { x.ExpenseId, x.TripId });

            modelBuilder.Entity<TripExpenseInclude>()
                        .HasKey(x => new { x.ExpenseId, x.TripId });

            modelBuilder.Entity<CallBlacklist>()
                        .HasKey(x => new { x.UserId, x.Number });

            modelBuilder.Entity<GeohashCity>()
                        .HasKey(x => new { x.CityId, x.Geohash });

            modelBuilder.Entity<GeohashCountry>()
                        .HasKey(x => new { x.CountryId, x.Geohash });

            modelBuilder.Entity<Location>()
                        .HasMany(x => x.Geohashes)
                        .WithOne(x => x.Location);

            modelBuilder.Entity<Location>()
                        .Property(x => x.Latitude)
                        .HasColumnType("numeric(9, 6)");

            modelBuilder.Entity<Location>()
                        .Property(x => x.Longitude)
                        .HasColumnType("numeric(9, 6)");

            modelBuilder.Entity<CityAccessGeohash>()
                        .HasKey(x => new { x.CityId, x.Geohash });

            modelBuilder.Entity<CarFuel>()
                        .HasOne(x => x.Car)
                        .WithMany(x => x.CarFuelings);

            modelBuilder.Entity<RoutePoint>()
                        .HasKey(x => new { x.RouteId, x.Index });

            modelBuilder.Entity<RoutePoint>()
                        .Property(x => x.Lat)
                        .HasColumnType("numeric(9, 6)");

            modelBuilder.Entity<RoutePoint>()
                        .Property(x => x.Lng)
                        .HasColumnType("numeric(9, 6)");
        }
    }
}
