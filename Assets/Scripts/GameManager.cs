using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public const int MajorsMultiplier = 15;
    public const int MinorsMultiplier = 5;
    public const int FailureMultiplier = 10;
    
    public Transform patientContainer;
    public List<Patient> patients = new();
    public EquipmentType selectedEquipment;
    public ResultsPanel resultsPanel;
    
    public int patientsTreated;
    public int patientsDead;
    public int majorsTreated;
    public int minorsTreated;
    public int failedTreatments;

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
            patient.OnDeath += HandlePatientDeath;
        }

        // Add injuries via logic once initial testing in TestScene complete: *** Goes in Patient Spawner ?
        //patient.injuries.Add(new Injury
        //{
        //    injuryType = InjuryType.Burns,
        //    severity = Severity.Minor,
        //    escalationThreshold = 60f // explicit, not relying on the field default
        //});
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

    // Event Handlers
    private void HandlePatientClicked(Patient patient)
    {
        if (selectedEquipment == EquipmentType.None) return;

        TreatmentResult result = patient.TryTreatInjury(selectedEquipment);

        if (result.resolved && result.severity == Severity.Major) majorsTreated++;
        else if (result.resolved && result.severity == Severity.Minor) minorsTreated++;
        else failedTreatments++;

        selectedEquipment = EquipmentType.None;

        if (patient.fullyTreated)
        {
            patientsTreated++;
            CheckSceneComplete();
        }
    }

    private void HandlePatientDeath(Patient patient)
    {
        patientsDead++;
        CheckSceneComplete();
    }

    // End Scene
    private void CheckSceneComplete()
    {
        if (patientsTreated + patientsDead >= patients.Count) EndScene();
    }

    private void EndScene()
    {
        Debug.Log($"EnS maj: {majorsTreated} | min:{minorsTreated} | fai: {failedTreatments}");
        var result = new SceneResult
        {
            totalPatients = patients.Count,
            patientsTreated = patientsTreated,
            patientsDead = patientsDead,
            minorsTreated = minorsTreated,
            majorsTreated = majorsTreated,
            failedTreatments = failedTreatments,
        };
        var breakdown = CalcSceneResult(result);
        resultsPanel.Show(result, breakdown);
        Debug.Log($"EnS PRP maj: {majorsTreated} | min:{minorsTreated} | fai: {failedTreatments}");
    }

    private int CalcSceneResult(SceneResult result)
    {
        int pScore = result.patientsTreated;
        int majScore = result.majorsTreated * MajorsMultiplier;
        int minScore = result.minorsTreated * MinorsMultiplier;
        int failScore = result.failedTreatments * FailureMultiplier;
        int finalScore = (majScore + minScore - failScore) * pScore;

        return finalScore;
    }
}
