using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Core.Models.Users
{
    public class UserChangedEventArgs : EventArgs
    {
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
