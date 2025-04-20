
using Xunit;
using System;
using System.Windows.Threading;

namespace FactoryCube.Tests
{
    public class DispatcherTests
    {
        [Fact]
        public void DispatcherRunsOnUIThread()
        {
            bool ran = false;

            var frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                ran = true;
                frame.Continue = false;
            }));

            Dispatcher.PushFrame(frame);

            Assert.True(ran);
        }
    }
}
