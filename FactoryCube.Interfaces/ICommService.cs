﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Interfaces
{
    public interface ICommService
    {
        Task<bool> SendCommandAsync(string command);
        string LastStatus { get; }
    }
}
