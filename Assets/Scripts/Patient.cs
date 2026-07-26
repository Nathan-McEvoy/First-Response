using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Patient : MonoBehaviour
{
    [field: SerializeField] public PriorityTag PriorityTag { get; set; }
    [field: SerializeField] public bool Analysed { get; set; }

    [field: SerializeField] public bool IsDead { get; private set; }
    [field: SerializeField] public bool FullyTreated { get; private set; }
    [field: SerializeField] public float CurrentLife { get; private set; }
    [field: SerializeField] public String Name { get; private set; }

    public float LifeTotal { get; private set; }

    public bool IsInteractable => _conditions.All(c => c.Resolved);
    public IReadOnlyList<Injury> Injuries => _injuries;
    
    public event Action<Patient> OnClicked;
    public event Action<Patient> OnDeath;
    public event Action<Patient> HealthChanged;

    [SerializeField] private List<Injury> _injuries = new();
    [SerializeField] private List<Condition> _conditions = new();
    

    public void Initialize(string pName, List<Injury> pInjuries, List<Condition> pConditions)
    {
        LifeTotal = 100f;
        Name = pName;
        _injuries.AddRange(pInjuries);
        _conditions.AddRange(pConditions);
        CurrentLife = LifeTotal;
    }

    void Update()
    {
        if (IsDead) return;

        float totalDrain = 0f;
        foreach (var injury in _injuries)
        {
            if (!injury.Resolved)
            {
                totalDrain += injury.Tick(Time.deltaTime);
                if (injury.CausePlayerDeath)
                {
                    Die();
                    break;
                }
            }
        }
        CurrentLife -= totalDrain * Time.deltaTime;
        HealthChanged?.Invoke(this);
        if (CurrentLife <= 0f)
        {
            Die();
        }
    }

    public String GetInjurySummary()
    {
        return String.Join("\n", _injuries.Select(i => $"{i.Type} ({i.Level}) - {(i.Resolved ? "Resolved" : "Active")}"));
    }

    public TreatmentResult TryTreatInjury(EquipmentType equipment)
    {
        foreach (var injury in _injuries.OrderByDescending(i => i.Level))
        {
            if (injury.Resolved) continue;

            bool resolved = injury.TryResolve(equipment);
            if (resolved)
            {
                CheckFullyTreated();

                return new TreatmentResult
                {
                    resolved = true,
                    injuryType = injury.Type,
                    severity = injury.Level,
                };
            }   
        }

        return TreatmentResult.Failed;
    }

    // Event Broadcasts
    private void CheckFullyTreated() // was broadcaster
    {
        if (!FullyTreated && !IsDead && _injuries.All(i => i.Resolved))
        {
            FullyTreated = true;
            //announcedFullyTreated = true;
            //OnFullyTreated?.Invoke(this);
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        CurrentLife = 0f;
        HealthChanged?.Invoke(this);
        OnDeath?.Invoke(this);
    }

    private void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }

}
