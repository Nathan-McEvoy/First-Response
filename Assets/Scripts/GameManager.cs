using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly Dictionary<Severity, int> severityMultiplier = new()
    {
        { Severity.Minor, 1 },
        { Severity.Major, 3 },
    };

    private int treatmentPoints = 10;

    public static GameManager instance;

    public Transform patientContainer;
    public List<Patient> patients = new();
    public EquipmentType selectedEquipment;
    public int sceneScore;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        patients.AddRange(patientContainer.GetComponentsInChildren<Patient>());

        foreach (var patient in patients)
        {
            patient.OnClicked += HandlePatientClicked;
        }

        // Add injuries via logic once initial testing in TestScene complete:
        //patient.injuries.Add(new Injury
        //{
        //    injuryType = InjuryType.Burns,
        //    severity = Severity.Minor,
        //    escalationThreshold = 60f // explicit, not relying on the field default
        //});
    }

    void Update()
    {
        // pass
    }

    // UI Button Calls
    public void SelectBandage() => SelectEquipment(EquipmentType.Bandage);
    public void SelectCoolGel() => SelectEquipment(EquipmentType.CoolGel);
    public void SelectNeutraliser() => SelectEquipment(EquipmentType.Neutraliser);

    public void SelectEquipment(EquipmentType equipment)
    {
        if (selectedEquipment == equipment)
        {
            selectedEquipment = EquipmentType.None;
        }
        else
        {
            selectedEquipment = equipment;
        }
    }

    public void HandlePatientClicked(Patient patient)
    {
        if (selectedEquipment == EquipmentType.None)
        {
            return;
        }

        int score = 0;
        TreatmentResult result = patient.TryTreatInjury(selectedEquipment);
        if (!result.resolved)
        {
            score -= treatmentPoints * 2;
        }
        else
        {
            score += treatmentPoints * severityMultiplier[result.severity];
        }

        sceneScore += score;
        selectedEquipment = EquipmentType.None;
    }
}
