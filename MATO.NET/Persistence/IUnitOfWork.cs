using MATO.NET.Interfaces;

namespace MATO.NET.Persistence
{
    public interface IUnitOfWork
    {
        IUserService UserService { get; }
        IRequestsService RequestsService { get; }
        IAdminService AdminService { get; }
        IDecisionsService DecisionsService { get; }
        IEmailService EmailService { get; }
        void Complete();
    }
}