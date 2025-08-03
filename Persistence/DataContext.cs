using System.Security.Cryptography.X509Certificates;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public required DbSet<Activity> Activities { get; set; }
        public required DbSet<UserData> Users { get; set; }
        public required DbSet<ActivityAttendee> ActivityAttendees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActivityAttendee>(x => x.HasKey(a => new { a.ActivityId, a.UserId }));

            modelBuilder.Entity<ActivityAttendee>()
                .HasOne(x => x.UserData)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<ActivityAttendee>()
                .HasOne(x => x.Activity)
                .WithMany(x => x.Attendees)
                .HasForeignKey(x => x.ActivityId);
        }
    }
}