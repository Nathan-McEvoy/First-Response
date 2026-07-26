using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    
    public const int MajorsMultiplier = 15;
    public const int MinorsMultiplier = 5;
    public const int FailureMultiplier = 10;
    
    [SerializeField] private MainPanelsHandler mainPanelsHandler;
    [SerializeField] private List<Patient> _patients = new();
    [SerializeField] private PatientSpawner _patientSpawner;

    private int _patientsTreated;
    private int _patientsDead;
    private int _majorsTreated;
    private int _minorsTreated;
    private int _failedTreatments;

    void Awake()
    {
        Instance = this;
    }

    private void Start() // move to RandomPatientGenerator
    {
        _patientSpawner.SpawnPatient(
            0,
            "Dave McGuiness",
            new List<Injury> 
            {
                new(InjuryType.Burn, Severity.Minor),
            },
            new List<Condition> {}
            );

        _patientSpawner.SpawnPatient(
            1,
            "Mary Jazca",
            new List<Injury>
            {
                new(InjuryType.Chemical, Severity.Minor),
                new(InjuryType.Chemical, Severity.Major),
            },
            new List<Condition> { }
            );
    }

    public void RegisterPatient(Patient patient)
    {
        _patients.Add(patient);
        patient.OnClicked += mainPanelsHandler.ShowPatientDetails;
        patient.OnDeath += HandlePatientDeath;
        patient.HealthChanged += mainPanelsHandler.UpdatePDPHealthSlider;
    }

    // Event Handlers
    private void HandlePatientDeath(Patient patient)
    {
        _patientsDead++;
        CheckSceneComplete();
    }

    public void TryTreat(EquipmentType equipment, Patient patient)
    {
        TreatmentResult result = patient.TryTreatInjury(equipment);

        if (result.resolved && result.severity == Severity.Major) _majorsTreated++;
        else if (result.resolved && result.severity == Severity.Minor) _minorsTreated++;
        else _failedTreatments++;

        if (patient.FullyTreated)
        {
            _patientsTreated++;
            CheckSceneComplete();
        }
    }

    // End Scene
    private void CheckSceneComplete()
    {
        if (_patientsTreated + _patientsDead >= _patients.Count) EndScene();
    }

    private void EndScene()
    {
        Debug.Log($"EnS maj: {_majorsTreated} | min:{_minorsTreated} | fai: {_failedTreatments}");
        Debug.Log($"EnD pCount: {_patients.Count} | pTreat: {_patientsTreated} | pDead: {_patientsDead}");
        var result = new SceneResult
        {
            patientsTreated = _patientsTreated,
            minorsTreated = _minorsTreated,
            majorsTreated = _majorsTreated,
            failedTreatments = _failedTreatments,
        };
        var finalScore = CalcSceneResult(result);
        mainPanelsHandler.ShowEndPanel(result, finalScore);
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
