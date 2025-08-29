using System;
using System.Threading;
using System.Threading.Tasks;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ControlesAccesoQR.Tests
{
    [TestClass]
    public class FingerprintWorkflowTests
    {
        private class FakeSdk : IFingerprintSdk
        {
            public Func<CancellationToken, Task<byte[]>> CaptureFunc { get; set; }
            public Func<byte[], Guid, CancellationToken, Task<int>> MatchFunc { get; set; }
            public Task<byte[]> CaptureAsync(CancellationToken ct) => CaptureFunc(ct);
            public Task<int> MatchAsync(byte[] template, Guid choferId, CancellationToken ct) => MatchFunc(template, choferId, ct);
        }

        [TestMethod]
        public async Task ValidateAsync_ValidFingerprint()
        {
            var sdk = new FakeSdk
            {
                CaptureFunc = ct => Task.FromResult(new byte[] {1}),
                MatchFunc = (t, g, ct) => Task.FromResult(50)
            };
            var wf = new FingerprintWorkflow(sdk);
            var decision = await wf.ValidateAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.IsTrue(decision.IsValid);
            Assert.AreEqual(50, decision.Score);
        }

        [TestMethod]
        public async Task ValidateAsync_NotRegistered()
        {
            var sdk = new FakeSdk
            {
                CaptureFunc = ct => Task.FromResult(new byte[] {1}),
                MatchFunc = (t, g, ct) => Task.FromResult(20)
            };
            var wf = new FingerprintWorkflow(sdk);
            var decision = await wf.ValidateAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.IsFalse(decision.IsValid);
            Assert.AreEqual("MATCH_FAIL", decision.ErrorCode);
        }

        [TestMethod]
        public async Task ValidateAsync_LowQuality()
        {
            var sdk = new FakeSdk
            {
                CaptureFunc = ct => throw new Exception("LOW_QUALITY"),
                MatchFunc = (t, g, ct) => Task.FromResult(0)
            };
            var wf = new FingerprintWorkflow(sdk);
            var decision = await wf.ValidateAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.AreEqual("LOW_QUALITY", decision.ErrorCode);
        }

        [TestMethod]
        public async Task ValidateAsync_DeviceNotReady()
        {
            var sdk = new FakeSdk
            {
                CaptureFunc = ct => throw new Exception("DEVICE_NOT_READY"),
                MatchFunc = (t, g, ct) => Task.FromResult(0)
            };
            var wf = new FingerprintWorkflow(sdk);
            var decision = await wf.ValidateAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.AreEqual("DEVICE_NOT_READY", decision.ErrorCode);
        }

        [TestMethod]
        public async Task ValidateAsync_Timeout()
        {
            var sdk = new FakeSdk
            {
                CaptureFunc = async ct => { await Task.Delay(6000, ct); return new byte[] {1}; },
                MatchFunc = (t, g, ct) => Task.FromResult(0)
            };
            var wf = new FingerprintWorkflow(sdk);
            var decision = await wf.ValidateAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.AreEqual("CAPTURE_TIMEOUT", decision.ErrorCode);
        }

        [TestMethod]
        public async Task ValidateAsync_TemplateCorrupt()
        {
            var sdk = new FakeSdk
            {
                CaptureFunc = ct => Task.FromResult(new byte[] {1}),
                MatchFunc = (t, g, ct) => throw new Exception("TEMPLATE_CORRUPT")
            };
            var wf = new FingerprintWorkflow(sdk);
            var decision = await wf.ValidateAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.AreEqual("TEMPLATE_CORRUPT", decision.ErrorCode);
        }

        [TestMethod]
        public async Task ValidateAsync_GreyZoneRetry()
        {
            int call = 0;
            var sdk = new FakeSdk
            {
                CaptureFunc = ct => Task.FromResult(new byte[] {1}),
                MatchFunc = (t, g, ct) =>
                {
                    call++;
                    return Task.FromResult(call == 1 ? 35 : 35);
                }
            };
            var wf = new FingerprintWorkflow(sdk);
            var decision = await wf.ValidateAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.IsFalse(decision.IsValid);
            Assert.AreEqual(35, decision.Score);
        }
    }
}
