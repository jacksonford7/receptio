using System;
using System.Threading;
using System.Threading.Tasks;
using ControlesAccesoQR.Models;

namespace ControlesAccesoQR.Services
{
    public interface IFingerprintWorkflow
    {
        event Action<FingerprintStatus> StatusChanged;
        Task<FingerprintDecision> ValidateAsync(Guid choferId, CancellationToken ct);
    }
}
