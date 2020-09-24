using MATO.DataModel;
using MATO.NET.Interfaces;
using MATO.NET.Services;

namespace MATO.NET.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MATOContext _context;

        public IUserService UserService { get; private set; }
        public IRequestsService RequestsService { get; private set; }
        public IAdminService AdminService { get; private set; }
        public IDecisionsService DecisionsService { get; private set; }
        public IEmailService EmailService { get; private set; }

        public UnitOfWork(MATOContext context)
        {
            _context = context;
            UserService = new UserService(context);
            RequestsService = new RequestsService(context);
            AdminService = new AdminService(context);
            DecisionsService = new DecisionsService(context);
            EmailService = new EmailService(context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }
    }
}