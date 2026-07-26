using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PatientSpawner : MonoBehaviour
{
    [SerializeField] private Transform patientContainer;
    [SerializeField] private GameObject patientPrefab;
    [SerializeField] private Transform[] spawnPoints;

    public void SpawnPatient(int spawnIndex, string name, List<Injury> injuries, List<Condition> conditions)
    { // this should handle indexing if none given
        Vector2 position = spawnPoints[spawnIndex].position;
        GameObject patientGO = Instantiate(patientPrefab, position, Quaternion.identity, patientContainer);
        
        Patient patient = patientGO.GetComponent<Patient>();
        patient.Initialize(name, injuries, conditions);

        GameManager.Instance.RegisterPatient(patient);
    }
}