using Microsoft.EntityFrameworkCore;

namespace DnDTrackerAPI.Models
{
    public class DnDTrackerContext : DbContext
    {
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<Wave> Waves { get; set; }
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<User> Users { get; set; }

        public DnDTrackerContext(DbContextOptions<DnDTrackerContext> options) :
            base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            //builder
            //    .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Encounter

            builder.Entity<Encounter>()
                .HasKey(o => o.EncounterId);

            builder.Entity<Encounter>()
                .Property(o => o.EncounterId)
                .ValueGeneratedOnAdd();

            builder.Entity<Encounter>()
                .HasOne(o => o.User)
                .WithMany(o => o.Encounters)
                .HasForeignKey(o => o.UserId);

            #endregion

            #region User

            builder.Entity<User>()
                .HasKey(o => o.UserId);

            builder.Entity<User>()
               .Property(o => o.UserId)
               .ValueGeneratedOnAdd();

            builder.Entity<User>()
                .HasIndex(o => o.UserName)
                .IsUnique();

            #endregion

            #region Wave

            builder.Entity<Wave>()
                .HasKey(o => o.WaveId);

            builder.Entity<Wave>()
                .Property(o => o.WaveId)
                .ValueGeneratedOnAdd();

            builder.Entity<Wave>()
                .HasOne(o => o.Encounter)
                .WithMany(o => o.Waves)
                .HasForeignKey(o => o.EncounterId);

            #endregion

            #region Monster

            builder.Entity<Monster>()
                .HasKey(o => new { o.Index, o.WaveId });

            builder.Entity<Monster>()
                .HasOne(o => o.Wave)
                .WithMany(o => o.Monsters)
                .HasForeignKey(o => o.WaveId);

            #endregion
        }
    }
}