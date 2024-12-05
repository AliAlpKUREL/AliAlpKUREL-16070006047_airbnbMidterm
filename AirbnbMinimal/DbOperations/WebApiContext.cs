using AirbnbMinimal.Enums;
using AirbnbMinimal.Models;
using Microsoft.EntityFrameworkCore;

namespace AirbnbMinimal.DbOperations;

public class WebApiContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly DbContextOptions<WebApiContext> _context;
    public WebApiContext(DbContextOptions<WebApiContext> context, IConfiguration configuration) : base(context)
    {
        _context = context;
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = _configuration.GetConnectionString("mysql")
                                  ?? throw new ArgumentNullException(nameof(connectionString));

        optionsBuilder.UseMySQL(connectionString);
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);
        

        modelBuilder.Entity<User>()
            .HasMany(u => u.Listings)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId);
        

        modelBuilder.Entity<Listing>()
            .HasMany(l => l.Comments)
            .WithOne(c => c.Listing)
            .HasForeignKey(c => c.ListingId);
        

        modelBuilder.Entity<Booking>()
            .HasOne(r => r.Listing)
            .WithMany(l => l.Bookings)
            .HasForeignKey(r => r.ListingId);
        

        modelBuilder.Entity<Listing>()
            .HasOne(l => l.Location)
            .WithMany()
            .HasForeignKey(l => l.LocationId);
        

        modelBuilder.Entity<Location>()
            .HasOne(l => l.ParentLocation)
            .WithMany()
            .HasForeignKey(l => l.ParentLocationId);
        
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", CreatedDate = DateTime.Now, CreatedVersion = "1.0" },
                new Role { Id = 2, Name = "Guest", CreatedDate = DateTime.Now, CreatedVersion = "1.0" },
                new Role { Id = 3, Name = "Host", CreatedDate = DateTime.Now, CreatedVersion = "1.0" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "AdminName", Surname = "AdminSurname", Username = "Admin", Email = "admin@gmail.com", Phone = "5554443322", BirthDate = DateTime.Parse("2000-01-01"), Password = "password123", Gender = Gender.Erkek, RoleId = 1, CreatedDate = DateTime.Now, CreatedVersion = "1.0" },
                new User { Id = 2, Name = "GuestName", Surname = "GuestSurname", Username = "Guest", Email = "guest@gmail.com", Phone = "5554443322", BirthDate = DateTime.Parse("2000-01-01"), Password = "password123", Gender = Gender.Erkek, RoleId = 2, CreatedDate = DateTime.Now, CreatedVersion = "1.0" },
                new User { Id = 3, Name = "HostName", Surname = "HostSurname", Username = "Host", Email = "host@gmail.com", Phone = "5554443322", BirthDate = DateTime.Parse("2000-01-01"), Password = "password123", Gender = Gender.Erkek, RoleId = 3, CreatedDate = DateTime.Now, CreatedVersion = "1.0" },
                new User { Id = 4, Name = "Ali Alp", Surname = "Kurel", Username = "Admin", Email = "test@gmail.com", Phone = "5554443322", BirthDate = DateTime.Parse("2000-01-01"), Password = "password123", Gender = Gender.Erkek, RoleId = 3, CreatedDate = DateTime.Now, CreatedVersion = "1.0" }
            );
            
            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, Name = "Türkiye", Type = LocationType.Ulke, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },

                new Location { Id = 2, Name = "İstanbul", Type = LocationType.Il, ParentLocationId = 1, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 9, Name = "Ankara", Type = LocationType.Il, ParentLocationId = 1, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 14, Name = "İzmir", Type = LocationType.Il, ParentLocationId = 1, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },

                new Location { Id = 3, Name = "Kadıköy", Type = LocationType.Ilce, ParentLocationId = 2, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 5, Name = "Beşiktaş", Type = LocationType.Ilce, ParentLocationId = 2, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 7, Name = "Üsküdar", Type = LocationType.Ilce, ParentLocationId = 2, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 10, Name = "Çankaya", Type = LocationType.Ilce, ParentLocationId = 9, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 12, Name = "Keçiören", Type = LocationType.Ilce, ParentLocationId = 9, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 15, Name = "Bornova", Type = LocationType.Ilce, ParentLocationId = 14, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 17, Name = "Konak", Type = LocationType.Ilce, ParentLocationId = 14, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                
                new Location { Id = 4, Name = "Caferağa", Type = LocationType.Mahalle, ParentLocationId = 3, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 6, Name = "Etiler", Type = LocationType.Mahalle, ParentLocationId = 5, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 8, Name = "Beylerbeyi", Type = LocationType.Mahalle, ParentLocationId = 7, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 11, Name = "Bilkent", Type = LocationType.Mahalle, ParentLocationId = 10, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 13, Name = "Gazino", Type = LocationType.Mahalle, ParentLocationId = 12, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 16, Name = "Erzene", Type = LocationType.Mahalle, ParentLocationId = 15, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" },
                new Location { Id = 18, Name = "Alsancak", Type = LocationType.Mahalle, ParentLocationId = 17, CreatedDate = DateTime.UtcNow, CreatedVersion = "1.0" }
            );

           
    }
}