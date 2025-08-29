using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ControlesAccesoQR.Models;

namespace ControlesAccesoQR.Services
{
    // SDK-REAL: replace IFingerprintSdk with real SDK implementation
    public interface IFingerprintSdk
    {
        Task<byte[]> CaptureAsync(CancellationToken ct);
        Task<int> MatchAsync(byte[] template, Guid choferId, CancellationToken ct);
    }

    public class FingerprintWorkflow : IFingerprintWorkflow
    {
        private readonly IFingerprintSdk _sdk;
        public event Action<FingerprintStatus> StatusChanged;

        public FingerprintWorkflow(IFingerprintSdk sdk = null)
        {
            _sdk = sdk ?? new StubFingerprintSdk();
        }

        public async Task<FingerprintDecision> ValidateAsync(Guid choferId, CancellationToken ct)
        {
            var decision = new FingerprintDecision();
            StatusChanged?.Invoke(FingerprintStatus.Preparing);
            using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token))
            {
                var token = linked.Token;
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    try
                    {
                        StatusChanged?.Invoke(attempt == 0 ? FingerprintStatus.PlaceFinger : FingerprintStatus.Retrying);
                        var captureWatch = Stopwatch.StartNew();
                        var template = await _sdk.CaptureAsync(token).ConfigureAwait(false);
                        decision.CaptureLatencyMs = (int)captureWatch.ElapsedMilliseconds;

                        StatusChanged?.Invoke(FingerprintStatus.Capturing);
                        var matchWatch = Stopwatch.StartNew();
                        var score = await _sdk.MatchAsync(template, choferId, token).ConfigureAwait(false);
                        decision.MatchLatencyMs = (int)matchWatch.ElapsedMilliseconds;
                        decision.Score = score;

                        if (score >= 40)
                        {
                            decision.IsValid = true;
                            decision.Resultado = "OK";
                            StatusChanged?.Invoke(FingerprintStatus.Success);
                            decision.RetryCount = attempt;
                            return decision;
                        }
                        if (score >= 30 && score <= 39)
                        {
                            // single matching retry
                            StatusChanged?.Invoke(FingerprintStatus.Retrying);
                            score = await _sdk.MatchAsync(template, choferId, token).ConfigureAwait(false);
                            decision.Score = score;
                            if (score >= 40)
                            {
                                decision.IsValid = true;
                                decision.Resultado = "OK";
                                StatusChanged?.Invoke(FingerprintStatus.Success);
                                decision.RetryCount = attempt + 1;
                                return decision;
                            }
                        }
                        // score < 40 -> retry capture
                    }
                    catch (OperationCanceledException)
                    {
                        decision.ErrorCode = timeoutCts.IsCancellationRequested ? "CAPTURE_TIMEOUT" : "MATCH_FAIL";
                        break;
                    }
                    catch (Exception ex)
                    {
                        decision.ErrorCode = MapException(ex);
                        break;
                    }
                }
            }
            StatusChanged?.Invoke(FingerprintStatus.Denied);
            decision.IsValid = false;
            decision.Resultado = decision.ErrorCode ?? "DENIED";
            return decision;
        }

        private string MapException(Exception ex)
        {
            switch (ex.Message)
            {
                case "DEVICE_NOT_READY": return "DEVICE_NOT_READY";
                case "LOW_QUALITY": return "LOW_QUALITY";
                case "NORMALIZATION_FAILED": return "NORMALIZATION_FAILED";
                case "TEMPLATE_ERROR": return "TEMPLATE_ERROR";
                case "TEMPLATE_SIZE_EXCEEDED": return "TEMPLATE_SIZE_EXCEEDED";
                case "TEMPLATE_CORRUPT": return "TEMPLATE_CORRUPT";
                default: return "MATCH_FAIL";
            }
        }

        private class StubFingerprintSdk : IFingerprintSdk
        {
            public Task<byte[]> CaptureAsync(CancellationToken ct)
            {
                // SDK-REAL: implement real capture
                return Task.FromResult(new byte[] { 1, 2, 3 });
            }

            public Task<int> MatchAsync(byte[] template, Guid choferId, CancellationToken ct)
            {
                // SDK-REAL: implement real matching
                return Task.FromResult(50);
            }
        }
    }
}
