using AnticevicApi.Model.Database.Main.App;
using AnticevicApi.Model.Database.Main.Common;
using AnticevicApi.Model.Database.Main.Finance;
using AnticevicApi.Model.Database.Main.Org;
using AnticevicApi.Model.Database.Main.Security;
using AnticevicApi.Model.Database.Main.Tracking;
using AnticevicApi.Model.Database.Main.Transport;
using AnticevicApi.Model.Database.Main.User;
using Microsoft.EntityFrameworkCore;

namespace AnticevicApi.DL.DbContexts
{
    public class MainContext : DbContext
    {
        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarLog> CarLogs { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<IncomeSource> IncomeSources { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Poi> Pois { get; set; }
        public DbSet<PoiCategory> PoiCategories { get; set; }
        public DbSet<PoiList> PoiLists { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskChange> TaskChanges { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<Tracking> Trackings { get; set; }
        public DbSet<TrackingDistance> TrackingDistances { get; set; }
        public DbSet<UniqueLocation> UniqueLocations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=sql.anticevic.net;Database=AnticevicApi;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationSetting>()
                        .HasOne(x => x.Application)
                        .WithMany(x => x.Settings);

            modelBuilder.Entity<AccessToken>()
                        .HasOne(x => x.User);

            modelBuilder.Entity<Car>()
                        .HasOne(x => x.User);

            modelBuilder.Entity<CarLog>()
                        .HasOne(x => x.Car)
                        .WithMany(x => x.CarLogs);

            modelBuilder.Entity<Tracking>()
                        .HasOne(x => x.User)
                        .WithMany(x => x.Trackings);

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.ExpenseType)
                        .WithMany(x => x.Expenses);

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.Vendor)
                        .WithMany(x => x.Expenses);

            modelBuilder.Entity<Expense>()
                        .HasOne(x => x.Currency);

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

            modelBuilder.Entity<Poi>()
                        .HasOne(x => x.PoiList)
                        .WithMany(x => x.Pois);

            modelBuilder.Entity<Poi>()
                        .HasOne(x => x.PoiCategory)
                        .WithMany(x => x.Pois);

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
                        .HasKey(x => new { x.Timestamp, x.UserId });

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
        }
    }
}
