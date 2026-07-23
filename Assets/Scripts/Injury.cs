using System;
using UnityEngine;

[System.Serializable]
public class Injury
{
    public InjuryType injuryType;
    public Severity severity;
    public float timeSinceOnset;
    public float escalationThreshold = 60f;
    public bool resolved;
    public bool causePlayerDeath;

    public float Tick(float deltaTime)
    {
        if (resolved) return 0f;

        timeSinceOnset += deltaTime;

        if (timeSinceOnset >= escalationThreshold)
        {
            causePlayerDeath = Escalate();
        }

        return GetDrainRate();
    }

    private bool Escalate()
    {
        if (severity == Severity.Minor)
        {
            severity = Severity.Major;
            timeSinceOnset = 0f;
            return false;
        }
        else
        {
            return true;
        }
    }

    private float GetDrainRate()
    {
        return severity == Severity.Major ? 2f : 0.5f;
    }

    // Resolution
    public bool TryResolve(EquipmentType equipment)
    {
        if (Matches(equipment))
        {
            resolved = true;
            return true;
        }
        return false;
    }

    public bool Matches(EquipmentType equipment)
    {
        return (
            equipment == EquipmentType.Bandage && injuryType == InjuryType.Trauma ||
            equipment == EquipmentType.CoolGel && injuryType == InjuryType.Burn ||
            equipment == EquipmentType.Neutraliser && injuryType == InjuryType.Chemical
            );
    }


}
