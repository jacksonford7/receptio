using System;
using System.Threading;
using System.Windows;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Services;
using ControlesAccesoQR.ViewModels;
using ControlesAccesoQR.Views.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ControlesAccesoQR.Tests
{
    [TestClass]
    public class FingerprintPanelTests
    {
        private class StubWorkflow : IFingerprintWorkflow
        {
            public event Action<FingerprintStatus> StatusChanged;
            public Task<FingerprintDecision> ValidateAsync(Guid choferId, CancellationToken ct)
                => Task.FromResult(new FingerprintDecision());
        }

        [TestMethod]
        public void PanelStartsCaptureOnLoad()
        {
            var wf = new StubWorkflow();
            var cts = new CancellationTokenSource();
            var vm = new FingerprintPanelViewModel(wf, cts);
            var panel = new FingerprintPanel { DataContext = vm };
            panel.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            Assert.AreEqual("Coloque el dedo", vm.Status);
        }

        [TestMethod]
        public void CancelCommandCancelsToken()
        {
            var wf = new StubWorkflow();
            var cts = new CancellationTokenSource();
            var vm = new FingerprintPanelViewModel(wf, cts);
            vm.CancelCommand.Execute(null);
            Assert.IsTrue(cts.IsCancellationRequested);
        }
    }
}
