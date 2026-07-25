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
    
    public MainPanelsHandler mainPanelsHandler;
    
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
            //patient.OnClicked += HandlePatientClicked;;
            patient.OnClicked += mainPanelsHandler.ShowPatientDetails;
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

    // Event Handlers
    private void HandlePatientDeath(Patient patient)
    {
        patientsDead++;
        CheckSceneComplete();
    }

    public void TryTreat(EquipmentType equipment, Patient patient)
    {
        TreatmentResult result = patient.TryTreatInjury(equipment);

        if (result.resolved && result.severity == Severity.Major) majorsTreated++;
        else if (result.resolved && result.severity == Severity.Minor) minorsTreated++;
        else failedTreatments++;

        if (patient.fullyTreated)
        {
            patientsTreated++;
            CheckSceneComplete();
        }
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
        mainPanelsHandler.ShowEndPanel(result, breakdown);
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
