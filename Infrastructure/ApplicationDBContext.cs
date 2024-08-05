using Domain.Entities;
using Domain.Entities.SinglePhaseEntities;
using Domain.Entities.ThreePhaseEntities;
using Domain.Entities.ThreePhaseCTEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext()
        {
        }
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=crystalPowerDb.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<Meter>().HasKey(e => e.Id);
            modelBuilder.Entity<MeterFetchDataLog>().HasKey(e => e.Id);
            modelBuilder.Entity<BillingProfileSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<BillingProfileThreePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<BillingProfileThreePhaseCT>().HasKey(e => e.Id);
            modelBuilder.Entity<BlockLoadProfileSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<BlockLoadProfileThreePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<BlockLoadProfileThreePhaseCT>().HasKey(e => e.Id);
            modelBuilder.Entity<DailyLoadProfileSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<DailyLoadProfileThreePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<DailyLoadProfileThreePhaseCT>().HasKey(e => e.Id);
            modelBuilder.Entity<EventProfileSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<EventProfileThreePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<EventProfileThreePhaseCT>().HasKey(e => e.Id);
            modelBuilder.Entity<InstantaneousProfileSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<InstantaneousProfileThreePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<InstantaneousProfileThreePhaseCT>().HasKey(e => e.Id);

            modelBuilder.Entity<ControlEvent>().HasKey(e => e.Id);
            modelBuilder.Entity<NonRolloverEvent>().HasKey(e => e.Id);

            modelBuilder.Entity<OtherEvent>().HasKey(e => e.Id);
            modelBuilder.Entity<OtherEventSinglePhase>().HasKey(e => e.Id);

            modelBuilder.Entity<TransactionEvent>().HasKey(e => e.Id);
            modelBuilder.Entity<PowerRelatedEvent>().HasKey(e => e.Id);
            modelBuilder.Entity<CurrentRelatedEvent>().HasKey(e => e.Id);

            modelBuilder.Entity<VoltageRelatedEvent>().HasKey(e => e.Id);
            modelBuilder.Entity<VoltageRelatedEventSinglePhase>().HasKey(e => e.Id);

            modelBuilder.Entity<ESWSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<ESWThreePhase>().HasKey(e => e.Id);

            modelBuilder.Entity<ControlEventSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<CurrentRelatedEventSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<NonRolloverEventSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<PowerRelatedEventSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<TransactionEventSinglePhase>().HasKey(e => e.Id);
            modelBuilder.Entity<SelfDiagnostic>().HasKey(e => e.Id);
            modelBuilder.Entity<DIEvent>().HasKey(e => e.Id);
            modelBuilder.Entity<DIEventSinglePhase>().HasKey(e => e.Id);
        }
        public DbSet<User> User { get; set; }
        public DbSet<Meter> Meter { get; set; }
        public DbSet<MeterFetchDataLog> MeterFetchDataLog { get; set; }
        public DbSet<BillingProfileSinglePhase> BillingProfileSinglePhase { get; set; }
        public DbSet<BillingProfileThreePhase> BillingProfileThreePhase { get; set; }
        public DbSet<BillingProfileThreePhaseCT> BillingProfileThreePhaseCT { get; set; }
        public DbSet<BlockLoadProfileSinglePhase> BlockLoadProfileSinglePhase { get; set; }
        public DbSet<BlockLoadProfileThreePhase> BlockLoadProfileThreePhase { get; set; }
        public DbSet<BlockLoadProfileThreePhaseCT> BlockLoadProfileThreePhaseCT { get; set; }
        public DbSet<DailyLoadProfileSinglePhase> DailyLoadProfileSinglePhase { get; set; }
        public DbSet<DailyLoadProfileThreePhase> DailyLoadProfileThreePhase { get; set; }
        public DbSet<DailyLoadProfileThreePhaseCT> DailyLoadProfileThreePhaseCT { get; set; }
        public DbSet<EventProfileSinglePhase> EventProfileSinglePhase { get; set; }
        public DbSet<EventProfileThreePhase> EventProfileThreePhase { get; set; }
        public DbSet<EventProfileThreePhaseCT> EventProfileThreePhaseCT { get; set; }
        public DbSet<InstantaneousProfileSinglePhase> InstantaneousProfileSinglePhase { get; set; }
        public DbSet<InstantaneousProfileThreePhase> InstantaneousProfileThreePhase { get; set; }
        public DbSet<InstantaneousProfileThreePhaseCT> InstantaneousProfileThreePhaseCT { get; set; }

        public DbSet<ControlEvent> ControlEvent { get; set; }
        public DbSet<NonRolloverEvent> NonRolloverEvent { get; set; }

        public DbSet<OtherEvent> OtherEvent { get; set; }
        public DbSet<OtherEventSinglePhase> OtherEventSinglePhase { get; set; }

        public DbSet<TransactionEvent> TransactionEvent { get; set; }
        public DbSet<PowerRelatedEvent> PowerRelatedEvent { get; set; }
        public DbSet<CurrentRelatedEvent> CurrentRelatedEvent { get; set; }

        public DbSet<VoltageRelatedEvent> VoltageRelatedEvent { get; set; }
        public DbSet<VoltageRelatedEventSinglePhase> VoltageRelatedEventSinglePhase { get; set; }

        public DbSet<ControlEventSinglePhase> ControlEventSinglePhase { get; set; }
        public DbSet<NonRolloverEventSinglePhase> NonRolloverEventSinglePhase { get; set; }
        public DbSet<TransactionEventSinglePhase> TransactionEventSinglePhase { get; set; }
        public DbSet<PowerRelatedEventSinglePhase> PowerRelatedEventSinglePhase { get; set; }
        public DbSet<CurrentRelatedEventSinglePhase> CurrentRelatedEventSinglePhase { get; set; }

        public DbSet<ESWSinglePhase> ESWSinglePhase { get; set; }
        public DbSet<ESWThreePhase> ESWThreePhase { get; set; }
        public DbSet<SelfDiagnostic> SelfDiagnostic { get; set; }
        public DbSet<DIEvent> DIEvent { get; set; }
        public DbSet<DIEventSinglePhase> DIEventSinglePhase { get; set; }
    }
}
