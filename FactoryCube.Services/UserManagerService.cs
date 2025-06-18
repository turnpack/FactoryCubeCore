using FactoryCube.Interfaces;
using FactoryCube.Core.Models.Users;

namespace FactoryCube.Services
{
    public class UserManagerService : IUserManagerService
    {
        public event EventHandler<UserChangedEventArgs> UserChanged;

        public bool CanAccess(string permissionKey)
        {
            throw new NotImplementedException();
        }

        public UserRole GetCurrentUserRole()
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
