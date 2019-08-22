using ProjectIvy.Model.Database.Main.App;
using ProjectIvy.Model.Database.Main.Common;
using ProjectIvy.Model.Database.Main.Finance;
using ProjectIvy.Model.Database.Main.Inv;
using ProjectIvy.Model.Database.Main.Log;
using ProjectIvy.Model.Database.Main.Net;
using ProjectIvy.Model.Database.Main.Org;
using ProjectIvy.Model.Database.Main.Security;
using ProjectIvy.Model.Database.Main.Tracking;
using ProjectIvy.Model.Database.Main.Transport;
using ProjectIvy.Model.Database.Main.Travel;
using ProjectIvy.Model.Database.Main.User;
using ProjectIvy.Model.Database.Main.Storage;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Model.Database.Main.Beer;
using ProjectIvy.Model.Database.Main.Contacts;

namespace ProjectIvy.Data.DbContexts
{
    public class MainContext : DbContext
    {
        public MainContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DbSet<AccessToken> AccessTokens { get; set; }

        public DbSet<Airport> Airports { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }

        public DbSet<Beer> Beers { get; set; }

        public DbSet<BeerBrand> BeerBrands { get; set; }

        public DbSet<BeerServing> BeerServings { get; set; }

        public DbSet<BrowserLog> BrowserLogs { get; set; }

        public DbSet<Call> Calls { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<CarLog> CarLogs { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Consumation> Consumations { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<ContactType> ContactTypes { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<CountryPolygon> CountryPolygons { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Domain> Domains { get; set; }

        public DbSet<Device> Devices { get; set; }

        public DbSet<DeviceType> DeviceTypes { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<ExpenseFile> ExpenseFiles { get; set; }

        public DbSet<ExpenseFileType> ExpenseFileTypes { get; set; }

        public DbSet<ExpenseType> ExpenseTypes { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<FileType> FileTypes { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Income> Incomes { get; set; }

        public DbSet<IncomeSource> IncomeSources { get; set; }

        public DbSet<IncomeType> IncomeTypes { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<PaymentProvider> PaymentProviders { get; set; }

        public DbSet<PaymentProviderAccount> PaymentProviderAccounts { get; set; }

        public DbSet<PaymentType> PaymentTypes { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<Poi> Pois { get; set; }

        public DbSet<PoiCategory> PoiCategories { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<TaskChange> TaskChanges { get; set; }

        public DbSet<TaskPriority> TaskPriorities { get; set; }

        public DbSet<TaskStatus> TaskStatuses { get; set; }

        public DbSet<TaskType> TaskTypes { get; set; }

        public DbSet<ToDo> ToDos { get; set; }

        public DbSet<Tracking> Trackings { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public DbSet<TripCity> TripCities { get; set; }

        public DbSet<TripPoi> TripPois { get; set; }

        public DbSet<TripExpenseExclude> TripExpensesExcluded { get; set; }

        public DbSet<TripExpenseInclude> TripExpensesIncluded { get; set; }

        public DbSet<TrackingDistance> TrackingDistances { get; set; }

        public DbSet<UniqueLocation> UniqueLocations { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<VendorPoi> VendorPois { get; set; }

        public DbSet<Web> Webs { get; set; }

        public string ConnectionString { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationSetting>()
                        .HasOne(x => x.Application)
                        .WithMany(x => x.Settings);

            modelBuilder.Entity<Airport>()
                        .HasOne(x => x.City)
                        .WithMany(x => x.Airports);

            modelBuilder.Entity<Airport>()
                        .HasOne(x => x.Poi);

            modelBuilder.Entity<AccessToken>()
                        .HasOne(x => x.User);

            modelBuilder.Entity<Beer>()
                        .HasOne(x => x.BeerBrand)
                        .WithMany(x => x.Beers);

            modelBuilder.Entity<Consumation>()
                        .HasOne(x => x.Beer);

            modelBuilder.Entity<Consumation>()
                        .HasOne(x => x.BeerServing);

            modelBuilder.Entity<Car>()
                        .HasOne(x => x.User);

            modelBuilder.Entity<City>()
                        .HasOne(x => x.Country)
                        .WithMany(x => x.Cities);

            modelBuilder.Entity<CarLog>()
                        .HasOne(x => x.Car)
                        .WithMany(x => x.CarLogs);

            modelBuilder.Entity<CountryPolygon>()
                        .HasKey(x => new { x.CountryId, x.GroupId, x.Index });

            modelBuilder.Entity<Tracking>()
                        .HasOne(x => x.User)
                        .WithMany(x => x.Trackings);

            modelBuilder.Entity<Tracking>()
                        .Property(x => x.Latitude)
                        .HasColumnType("numeric(9, 6)");

            modelBuilder.Entity<Tracking>()
                        .Property(x => x.Longitude)
                        .HasColumnType("numeric(9, 6)");

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.ExpenseType)
                        .WithMany(x => x.Expenses);

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

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.Vendor)
                        .WithMany(x => x.Expenses);

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.Currency);

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.ParentCurrency);

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.Poi);

            modelBuilder.Entity<Vendor>()
                        .HasOne(x => x.City)
                        .WithMany(x => x.Vendors);

            modelBuilder.Entity<VendorPoi>()
                        .HasKey(x => new { x.PoiId, x.VendorId });

            modelBuilder.Entity<Flight>()
                        .HasOne(x => x.DestinationAirport);

            modelBuilder.Entity<Flight>()
                        .HasOne(x => x.OriginAirport);

            modelBuilder.Entity<Income>()
                        .HasOne(x => x.User);

            modelBuilder.Entity<Income>()
                        .HasOne(x => x.Currency);

            modelBuilder.Entity<Income>()
                        .HasOne(x => x.IncomeSource)
                        .WithMany(x => x.Incomes);

            modelBuilder.Entity<Task>()
                        .HasOne(x => x.Project)
                        .WithMany(x => x.Tasks);

            modelBuilder.Entity<TaskChange>()
                        .HasOne(x => x.Task)
                        .WithMany(x => x.Changes);

            modelBuilder.Entity<TaskChange>()
                        .HasOne(x => x.Priority);

            modelBuilder.Entity<TaskChange>()
                        .HasOne(x => x.Status);

            modelBuilder.Entity<Task>()
                        .HasOne(x => x.Type);

            modelBuilder.Entity<TrackingDistance>()
                        .HasKey(x => new { Timestamp = x.Timestamp, x.UserId });

            modelBuilder.Entity<User>()
                        .HasOne(x => x.BirthCity);

            modelBuilder.Entity<UserRole>()
                        .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder.Entity<UserRole>()
                        .HasOne(x => x.User)
                        .WithMany(x => x.UserRoles);

            modelBuilder.Entity<UserRole>()
                        .HasOne(x => x.Role)
                        .WithMany(x => x.UserRoles);

            modelBuilder.Entity<RelatedTask>()
                        .HasOne(x => x.Task);

            modelBuilder.Entity<RelatedTask>()
                        .HasOne(x => x.Related);

            modelBuilder.Entity<RelatedTask>()
                        .HasKey(x => new { x.RelatedId, x.TaskId });

            modelBuilder.Entity<RelatedTask>()
                        .HasOne(x => x.Task)
                        .WithMany(x => x.WhichRelate);

            modelBuilder.Entity<RelatedTask>()
                        .HasOne(x => x.Related)
                        .WithMany(x => x.Related);

            modelBuilder.Entity<Device>()
                        .HasOne(x => x.DeviceType)
                        .WithMany(x => x.Devices);

            modelBuilder.Entity<Domain>()
                        .HasOne(x => x.Web)
                        .WithMany(x => x.Domains);

            modelBuilder.Entity<BrowserLog>()
                        .HasOne(x => x.Device)
                        .WithMany(x => x.BrowserLogs);

            modelBuilder.Entity<BrowserLog>()
                        .HasOne(x => x.Domain)
                        .WithMany(x => x.BrowserLogs);

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

            modelBuilder.Entity<TripCity>()
                        .HasKey(x => new { x.CityId, x.TripId });

            modelBuilder.Entity<Trip>()
                        .HasMany(x => x.Cities)
                        .WithOne(x => x.Trip);

            modelBuilder.Entity<Trip>()
                        .HasMany(x => x.Pois)
                        .WithOne(x => x.Trip);
        }
    }
}
