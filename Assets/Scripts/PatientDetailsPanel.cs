using TMPro;
using UnityEngine;

public class PatientDetailsPanel : MonoBehaviour
{
    public GameObject blockedSection;
    public GameObject minimalInfoSection;
    public GameObject fullInfoSection;
    public GameObject triageSubPanel;
    public GameObject treatSubPanel;
    public TMP_Text patientNameText;
    public TMP_Text injuryDetailsText;

    private Patient currentPatient;
    private bool analysed;

    public void Show(Patient patient)
    {
        currentPatient = patient;
        gameObject.SetActive(true);

        bool blocked = !patient.IsInteractable;
        blockedSection.SetActive(blocked);
        minimalInfoSection.SetActive(!blocked);
        fullInfoSection.SetActive(analysed);
        triageSubPanel.SetActive(false);
        treatSubPanel.SetActive(false);

        patientNameText.text = patient.name;
    }

    private void BuildInjurySummary()
        {
            injuryDetailsText.text = currentPatient.GetInjurySummary();
        }

    // Button Event Handlers
    public void OnAnalysedPressed()
    {
        if (currentPatient == null || !currentPatient.IsInteractable) return;
        analysed = true;
        fullInfoSection.SetActive(true);
        BuildInjurySummary();
    }

    public void OnTriagePressed() => triageSubPanel.SetActive(!triageSubPanel.activeSelf);
    public void OnTreatPressed() => treatSubPanel.SetActive(!treatSubPanel.activeSelf);

    public void OnBandagePressed()
    {
        GameManager.instance.TryTreat(EquipmentType.Bandage, currentPatient);
        BuildInjurySummary();
    }
    
    public void OnCoolGelPressed()
    {
        GameManager.instance.TryTreat(EquipmentType.CoolGel, currentPatient);
        BuildInjurySummary();
    }
    public void OnNeutraliserPressed()
    {
        GameManager.instance.TryTreat(EquipmentType.Neutraliser, currentPatient);
        BuildInjurySummary();
    }
}
