using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Patient : MonoBehaviour
{
    public bool IsDead { get; private set; }
    public bool IsInteractable => conditions.All(c => c.resolved);
    
    public bool fullyTreated;
    public float lifeTotal = 100f;
    public float currentLife;
    public List<Injury> injuries = new();
    public List<Condition> conditions = new();
    public PriorityTag priorityTag = PriorityTag.None;
    public new String name = "Bob";

    public event Action<Patient> OnClicked;
    public event Action<Patient> OnDeath;

    void Start()
    {
        currentLife = lifeTotal;
    }

    void Update()
    {
        if (IsDead) return;

        float totalDrain = 0f;
        foreach (var injury in injuries)
        {
            if (!injury.resolved)
            {
                totalDrain += injury.Tick(Time.deltaTime);
                if (injury.causePlayerDeath)
                {
                    Die();
                    break;
                }
            }
        }
        currentLife -= totalDrain * Time.deltaTime;
        if (currentLife <= 0f)
        {
            Die();
        }
    }

    public String GetInjurySummary()
    {
        return String.Join("\n", injuries.Select(i => $"{i.injuryType} ({i.severity}) - {(i.resolved ? "Resolved" : "Active")}"));
    }

    public TreatmentResult TryTreatInjury(EquipmentType equipment)
    {
        foreach (var injury in injuries.OrderByDescending(i => i.severity))
        {
            if (injury.resolved) continue;

            bool resolved = injury.TryResolve(equipment);
            if (resolved)
            {
                CheckFullyTreated();

                return new TreatmentResult
                {
                    resolved = true,
                    injuryType = injury.injuryType,
                    severity = injury.severity,
                };
            }   
        }

        return TreatmentResult.Failed;
    }

    // Event Broadcasts
    private void CheckFullyTreated() // was broadcaster
    {
        if (!fullyTreated && !IsDead && injuries.All(i => i.resolved))
        {
            fullyTreated = true;
            //announcedFullyTreated = true;
            //OnFullyTreated?.Invoke(this);
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        currentLife = 0f;
        OnDeath?.Invoke(this);
    }

    private void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }

}
