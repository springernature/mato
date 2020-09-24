using MATO.Classes;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace MATO.DataModel
{
    public class MATOContext : IdentityDbContext<AppUser>
    {
        public DbSet<Requests> Requests { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<AuthorDetails> AuthorDetails { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<TalkType> TalkType { get; set; }
        public DbSet<TargetSector> TargetSector { get; set; }
        public DbSet<PromotedTitle> PromotedTitle { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<TitleForecast> TitleForecast { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<TitleAuthorAssociation> TitleAuthorAssociation { get; set; }
        public DbSet<Calendar> Calendar { get; set; }
        public DbSet<EmailTemplates> EmailTemplates { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<WebinarCalendar> WebinarCalendars { get; set; }
        public DbSet<AuthorFiles> AuthorFiles { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }

    }
}
