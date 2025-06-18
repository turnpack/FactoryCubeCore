using FactoryCube.Core.Models.Wizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryCube.Interfaces.Wizard
{
    public interface IRecipeTeachingStep
    {
        int StepOrder { get; }
        string StepName { get; }

        Task ExecuteAsync(TeachingContext context);
    }
}
