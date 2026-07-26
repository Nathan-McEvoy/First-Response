using UnityEngine;

public class MainPanelsHandler : MonoBehaviour
{
    public static MainPanelsHandler Instance { get; private set; }

    [SerializeField] private GameObject _patientsDetailsPanelObject;
    [SerializeField] private PatientDetailsPanel _patientDetailsPanel;
    [SerializeField] private GameObject _resultsPanelObject;
    [SerializeField] private ResultsPanel _resultsPanel;

    private void Awake()
    {
        Instance = this;

        _resultsPanelObject.SetActive(false);
        _patientsDetailsPanelObject.SetActive(false);
    }

    public void ShowPatientDetails(Patient patient)
    {
        _patientsDetailsPanelObject.SetActive(true);
        _patientDetailsPanel.Show(patient);
    }

    public void ShowEndPanel(SceneResult result, int fScore)
    {
        _resultsPanelObject.SetActive(true);
        _resultsPanel.Show(result,fScore);
    }

    public void UpdatePDPHealthSlider(Patient patient)
    {
        _patientDetailsPanel.UpdateHealthSlider(patient);
    }
}