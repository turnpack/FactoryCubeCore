using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FactoryCube.Core.Models.Recipes;
using FactoryCube.Interfaces.Wizard;
using FactoryCube.Services.Wizard;
using FactoryCube.Core.Models.Wizard;

namespace FactoryCube.Tests.Wizards
{
    public class RecipeTeachingWizardService_RecipeExecution_Tests
    {
        [Fact]
        public async Task RunWizardAsync_ExecutesOnlyStepsDeclaredInRecipe()
        {
            // Arrange
            var context = new TeachingContext();
            var recipe = new Recipe
            {
                RecipeId = "r001",
                Name = "Test Recipe",
                StepSequence = new List<string> { "pickup", "eject" }
            };

            var callLog = new List<string>();

            var pickupStep = new Mock<IRecipeTeachingStep>();
            pickupStep.SetupGet(s => s.StepId).Returns("pickup");
            pickupStep.SetupGet(s => s.StepName).Returns("Pickup Nozzle");
            pickupStep.Setup(s => s.ExecuteAsync(context))
                      .Returns(() => {
                          callLog.Add("pickup");
                          return Task.CompletedTask;
                      });

            var ejectStep = new Mock<IRecipeTeachingStep>();
            ejectStep.SetupGet(s => s.StepId).Returns("eject");
            ejectStep.SetupGet(s => s.StepName).Returns("Ejection System");
            ejectStep.Setup(s => s.ExecuteAsync(context))
                     .Returns(() => {
                         callLog.Add("eject");
                         return Task.CompletedTask;
                     });

            var unusedStep = new Mock<IRecipeTeachingStep>();
            unusedStep.SetupGet(s => s.StepId).Returns("visionCal");
            unusedStep.SetupGet(s => s.StepName).Returns("Vision Calibration");
            // Unused step shouldn't be called

            var wizard = new RecipeTeachingWizardService(new[]
            {
                pickupStep.Object, ejectStep.Object, unusedStep.Object
            });

            // Act
            await wizard.RunWizardAsync(recipe, context);

            // Assert
            Assert.Equal(2, callLog.Count);
            Assert.Equal("pickup", callLog[0]);
            Assert.Equal("eject", callLog[1]);

            // Confirm unused step was NOT called
            unusedStep.Verify(s => s.ExecuteAsync(It.IsAny<TeachingContext>()), Times.Never);
        }
    }
}
