using UnityEngine;

public class MainPanelsHandler : MonoBehaviour
{
    public static MainPanelsHandler Instance;

    public GameObject patientsDetailsPanelObject;
    public PatientDetailsPanel patientDetailsPanel;
    public GameObject resultsPanelObject;
    public ResultsPanel resultsPanel;

    private void Awake()
    {
        Instance = this;

        resultsPanelObject.SetActive(false);
        patientsDetailsPanelObject.SetActive(false);
    }

    public void ShowPatientDetails(Patient patient)
    {
        patientsDetailsPanelObject.SetActive(true);
        patientDetailsPanel.Show(patient);
    }

    public void ShowEndPanel(SceneResult result, int fScore)
    {
        resultsPanelObject.SetActive(true);
        resultsPanel.Show(result,fScore);
    }
}