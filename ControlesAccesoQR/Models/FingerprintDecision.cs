using System;

namespace ControlesAccesoQR.Models
{
    public enum FingerprintStatus
    {
        Preparing,
        PlaceFinger,
        Capturing,
        Retrying,
        Success,
        Denied
    }

    public class FingerprintDecision
    {
        public bool IsValid { get; set; }
        public int Score { get; set; }
        public string ErrorCode { get; set; }
        public object Resultado { get; set; }
        public int CaptureLatencyMs { get; set; }
        public int MatchLatencyMs { get; set; }
        public int RetryCount { get; set; }
        public double AvgScore { get; set; }
        public double Frr { get; set; }
        public double Far { get; set; }
    }
}
