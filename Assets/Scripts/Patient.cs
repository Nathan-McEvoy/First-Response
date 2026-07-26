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
    [field: SerializeField] public float CurrentLife { get; private set; }
    [field: SerializeField] public String Name { get; private set; }

    public float LifeTotal { get; private set; }

    public bool IsFullyTreated => _injuries.All(i => i.Resolved);
    public bool IsInteractable => _conditions.All(c => c.Resolved);
    public IReadOnlyList<Injury> Injuries => _injuries;
    public IReadOnlyList<Condition> Conditions => _conditions;

    public event Action<Patient> OnClicked;
    public event Action<Patient> OnDeath;
    public event Action<Patient> HealthChanged;

    [SerializeField] private List<Injury> _injuries = new();
    [SerializeField] private List<Condition> _conditions = new();

    [SerializeField] private GameObject _sparks;
    [SerializeField] private GameObject _flames;
    [SerializeField] private GameObject _rocks;

    private Dictionary<ConditionType, GameObject> _conditionVisuals;

    void Awake()
    {
        _conditionVisuals = new Dictionary<ConditionType, GameObject>
        {
            { ConditionType.OnFire, _flames },
            { ConditionType.Electrocuted, _sparks },
            { ConditionType.Trapped, _rocks }
        };
    }

    public void Initialize(string pName, List<Injury> pInjuries, List<Condition> pConditions)
    {
        LifeTotal = 100f;
        Name = pName;
        _injuries.AddRange(pInjuries);
        _conditions.AddRange(pConditions);
        CurrentLife = LifeTotal;

        foreach (var condition in _conditions)
        {
            SetConditionVisual(condition.Type, true);
        }
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
        if (!IsInteractable) return "Conditions are blocking analysis!";

        return String.Join("\n", _injuries.Select(i => $"{i.Type} ({i.Level}) - {(i.Resolved ? "Resolved" : "Active")}"));
    }

    public TreatmentResult TryTreatInjury(EquipmentType equipment)
    {
        if (!IsInteractable) return TreatmentResult.Failed;

        foreach (var injury in _injuries.OrderByDescending(i => i.Level))
        {
            if (injury.Resolved) continue;

            if (injury.TryResolve(equipment))
            {
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

    public bool TryTreatCondition(EquipmentType equipment)
    {
        foreach (var condition in _conditions)
        {
            if (condition.Resolved) continue;

            if (condition.TryResolve(equipment)) 
            {
                SetConditionVisual(condition.Type, false);
                return true;
            }
        }
        return false;
    }

    private void SetConditionVisual(ConditionType type, bool active)
    {
        if (_conditionVisuals.TryGetValue(type, out GameObject visual))
        {
            visual.SetActive(active);
        }
    }

    // Event Broadcasts
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
