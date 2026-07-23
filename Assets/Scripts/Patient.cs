using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Patient : MonoBehaviour
{
    public bool isDead = false;
    public float lifeTotal = 100f;
    public float currentLife;
    public List<Injury> injuries = new();
    public List<Condition> conditions = new();
    public PriorityTag priorityTag = PriorityTag.None;
    public event Action<Patient> OnClicked;

    void Start()
    {
        currentLife = lifeTotal;
    }

    void Update()
    {
        if (isDead) return;

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

    private void Die()
    {
        isDead = true;
        currentLife = 0f;
    }

    public TreatmentResult TryTreatInjury(EquipmentType equipment)
    {
        foreach (var injury in injuries.OrderByDescending(i => i.severity))
        {
            if (injury.resolved)
            {
                continue;
            }

            bool resolved = injury.TryResolve(equipment);
            if (resolved)
            {
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

    

    // OnClick Event Broadcast
    private void OnMouseDown()
    {
        OnClicked?.Invoke(this);
    }

}
