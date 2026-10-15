using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
namespace RaceDay.Api.Data;
public class RaceDayContext(DbContextOptions<RaceDayContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RaceEvent> Events => Set<RaceEvent>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EventEnrollment> EventEnrollments => Set<EventEnrollment>();
    public DbSet<RaceResult> Results => Set<RaceResult>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Role>().ToTable("Roles").HasKey(x => x.RoleID);
        b.Entity<Role>().HasIndex(x => x.RoleName).IsUnique();
        b.Entity<User>().ToTable("Users").HasKey(x => x.UserID);
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<User>().HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleID);
        b.Entity<RaceEvent>().ToTable("Events").HasKey(x => x.EventID);
        b.Entity<RaceEvent>().HasOne(x => x.Organizer).WithMany().HasForeignKey(x => x.OrganizerID).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Category>().ToTable("Categories").HasKey(x => x.CategoryID);
        b.Entity<Category>().HasOne(x => x.Event).WithMany().HasForeignKey(x => x.EventID).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Category>().HasIndex(x => new { x.EventID, x.CategoryName }).IsUnique();
        b.Entity<EventEnrollment>().ToTable("EventEnrollments").HasKey(x => x.EnrollmentID);
        b.Entity<EventEnrollment>().HasOne(x => x.Participant).WithMany().HasForeignKey(x => x.ParticipantID).OnDelete(DeleteBehavior.Restrict);
        b.Entity<EventEnrollment>().HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryID).OnDelete(DeleteBehavior.Restrict);
        b.Entity<EventEnrollment>().HasIndex(x => new { x.ParticipantID, x.CategoryID }).IsUnique();
        b.Entity<EventEnrollment>().HasIndex(x => x.BibNumber).IsUnique().HasFilter("[BibNumber] IS NOT NULL");
        b.Entity<RaceResult>().ToTable("Results").HasKey(x => x.ResultID);
        b.Entity<RaceResult>().HasOne(x => x.Enrollment).WithOne().HasForeignKey<RaceResult>(x => x.EnrollmentID).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Category>().Property(x => x.DistanceKM).HasPrecision(6,2);
        b.Entity<Category>().Property(x => x.EntryFee).HasPrecision(10,2);
        b.Entity<User>().Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
        b.Entity<RaceEvent>().Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
        b.Entity<EventEnrollment>().Property(x => x.EnrollmentDate).HasDefaultValueSql("GETDATE()");
        b.Entity<RaceResult>().Property(x => x.RecordedAt).HasDefaultValueSql("GETDATE()");
        b.Entity<Role>().HasData(
            new Role { RoleID=1, RoleName="Administrator", Description="Manages the RaceDay system" },
            new Role { RoleID=2, RoleName="Organizer", Description="Creates and manages race events" },
            new Role { RoleID=3, RoleName="Participant", Description="Registers and participates in events" });
    }
}
