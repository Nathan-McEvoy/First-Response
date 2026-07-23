using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public struct TreatmentResult
{
    public bool resolved;
    public Severity severity;
    public InjuryType injuryType;

    public static TreatmentResult Failed => new TreatmentResult { resolved = false };
}
