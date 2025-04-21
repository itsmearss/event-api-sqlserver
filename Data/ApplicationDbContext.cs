using Microsoft.EntityFrameworkCore;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<RegisterEvent> RegisterEvents { get; set; }
        public DbSet<Z_Production_Schedule> Z_Production_Schedule { get; set; }
        public DbSet<A_T_NonWorking_Date> A_T_NonWorking_Date { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Model
            modelBuilder.Entity<User>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<User>()
                .Property(e => e.Username)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(e => e.Fullname)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(e => e.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasData(
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        Fullname = "Admin",
                        Password = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                        CreatedAt = new DateTime()
                    }
                );

            // Role
            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(e => e.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .HasData(
                    new Role
                    {
                        Id = 1,
                        Name = "User",
                        CreatedAt = new DateTime()
                    }, 
                    new Role
                    {
                        Id = 2,
                        Name = "Partner",
                        CreatedAt = new DateTime()
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "Admin",
                        CreatedAt = new DateTime()
                    }
                );

            // UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.Id);

            // one to many relations between UserRole and User
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);

            // one to many relations between UserRole and Role
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasData(
                    new UserRole
                    {
                        Id = 1,
                        UserId = 1,
                        RoleId = 3
                    }
                );

            // Category
            modelBuilder.Entity<Category>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .IsRequired();

            // one to many relations between category and event
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Events)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoryId)
                .IsRequired();

            // Event
            modelBuilder.Entity<Event>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Event>()
                .Property(e => e.Title)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Description)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Date)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Time)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Location)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.MaxAttendees)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Status)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Flyer)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Cover)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .HasMany(e => e.RegisterEvents)
                .WithOne(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(e => e.RegisterEvents)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            // RegisterEvent
            modelBuilder.Entity<RegisterEvent>()
                .HasKey(re => re.Id);

            modelBuilder.Entity<RegisterEvent>()
                .Property(re => re.EventId)
                .IsRequired();

            modelBuilder.Entity<RegisterEvent>()
                .Property(re => re.UserId)
                .IsRequired();

            modelBuilder.Entity<RegisterEvent>()
                .Property(re => re.IsAttend)
                .IsRequired();

            modelBuilder.Entity<RegisterEvent>()
                .Property(re => re.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<Z_Production_Schedule>()
                .HasKey(x => new { x.Factory_ID, x.PO_No, x.Batch_No });
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.Plan_Qty).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.Pairing).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.ST_Bal).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.ST_EOLR_Target).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.SF_Bal).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.SF_EOLR_Target).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.AS_Bal).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.AS_EOLR_Target).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.ACC_GR_QTY).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.SHIP_QTY).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.FA_sequence).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.UA_sequence).HasPrecision(20, 5);
            modelBuilder.Entity<Z_Production_Schedule>()
                .Property(x => x.BA_sequence).HasPrecision(20, 5);

            modelBuilder.Entity<A_T_NonWorking_Date>()
                .HasKey(x => x.ID);
        }
    }
}
