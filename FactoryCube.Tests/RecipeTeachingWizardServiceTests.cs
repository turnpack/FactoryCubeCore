using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FactoryCube.Interfaces.Wizard;
using FactoryCube.Core.Models.Wizard;
using FactoryCube.Services.Wizard;

namespace FactoryCube.Tests
{
    public class RecipeTeachingWizardServiceTests
    {
        private readonly TeachingContext _context;

        public RecipeTeachingWizardServiceTests()
        {
            _context = new TeachingContext();
        }

        [Fact]
        public async Task RunWizardAsync_ExecutesStepsInOrder()
        {
            // Arrange
            var callLog = new List<string>();

            var step1 = new Mock<IRecipeTeachingStep>();
            step1.SetupGet(s => s.StepOrder).Returns(2);
            step1.SetupGet(s => s.StepName).Returns("SecondStep");
            step1.Setup(s => s.ExecuteAsync(_context))
                 .Returns(() =>
                 {
                     callLog.Add("SecondStep");
                     return Task.CompletedTask;
                 });

            var step2 = new Mock<IRecipeTeachingStep>();
            step2.SetupGet(s => s.StepOrder).Returns(1);
            step2.SetupGet(s => s.StepName).Returns("FirstStep");
            step2.Setup(s => s.ExecuteAsync(_context))
                 .Returns(() =>
                 {
                     callLog.Add("FirstStep");
                     return Task.CompletedTask;
                 });

            var wizard = new RecipeTeachingWizardService(new[] { step1.Object, step2.Object });

            // Act
            await wizard.RunWizardAsync(_context);

            // Assert
            Assert.Equal(2, callLog.Count);
            Assert.Equal("FirstStep", callLog[0]);
            Assert.Equal("SecondStep", callLog[1]);
        }

        [Fact]
        public async Task RunWizardAsync_ThrowsWithStepNameOnFailure()
        {
            // Arrange
            var faultyStep = new Mock<IRecipeTeachingStep>();
            faultyStep.SetupGet(s => s.StepOrder).Returns(1);
            faultyStep.SetupGet(s => s.StepName).Returns("FaultyStep");
            faultyStep.Setup(s => s.ExecuteAsync(_context))
                      .ThrowsAsync(new InvalidOperationException("Something went wrong"));

            var wizard = new RecipeTeachingWizardService(new[] { faultyStep.Object });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RecipeTeachingException>(
                () => wizard.RunWizardAsync(_context));

            Assert.Contains("FaultyStep", ex.Message);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }
    }
}
