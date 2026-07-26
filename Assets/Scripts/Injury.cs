using System;
using UnityEngine;

[System.Serializable]
public class Injury
{
    public InjuryType Type { get; }
    public Severity Level { get; private set; }
    public bool Resolved { get; private set; }
    public bool CausePlayerDeath { get; private set; }

    private float _timeSinceOnset;
    private readonly float _escalationThreshold = 60f;
    
    public Injury(InjuryType type, Severity level) 
    {
        Type = type;
        Level = level;
    }

    public float Tick(float deltaTime)
    {
        if (Resolved) return 0f;

        _timeSinceOnset += deltaTime;

        if (_timeSinceOnset >= _escalationThreshold)
        {
            CausePlayerDeath = Escalate();
        }

        return GetDrainRate();
    }

    private bool Escalate()
    {
        if (Level == Severity.Minor)
        {
            Level = Severity.Major;
            _timeSinceOnset = 0f;
            return false;
        }
        else
        {
            return true;
        }
    }

    private float GetDrainRate()
    {
        return Level == Severity.Major ? 2f : 0.5f;
    }

    // Resolution
    public bool TryResolve(EquipmentType equipment)
    {
        if (Matches(equipment))
        {
            Resolved = true;
            return true;
        }
        return false;
    }

    public bool Matches(EquipmentType equipment)
    {
        return (
            equipment == EquipmentType.Bandage && Type == InjuryType.Trauma ||
            equipment == EquipmentType.CoolGel && Type == InjuryType.Burn ||
            equipment == EquipmentType.Neutraliser && Type == InjuryType.Chemical
            );
    }


}
