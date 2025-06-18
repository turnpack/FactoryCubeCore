using System;
using System.Threading.Tasks;
using FactoryCube.Core.Models.Users;

namespace FactoryCube.Interfaces
{
    public interface IUserManagerService
    {
        Task<bool> LoginAsync(string username, string password);
        void Logout();
        UserRole GetCurrentUserRole();
        bool CanAccess(string permissionKey);
        event EventHandler<UserChangedEventArgs> UserChanged;

    }
}
