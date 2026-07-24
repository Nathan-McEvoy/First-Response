using UnityEngine;

[System.Serializable]
public struct SceneResult
{
    public int totalPatients;
    public int patientsTreated;
    public int patientsDead;
    public int minorsTreated;
    public int majorsTreated;
    public int failedTreatments;
}
