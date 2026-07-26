using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PatientDetailsPanel : MonoBehaviour
{
    [SerializeField] private GameObject _blockedPanel;
    [SerializeField] private GameObject _basicInfoPanel;
    [SerializeField] private GameObject _injuryInfoPanel;
    [SerializeField] private GameObject _triageSubPanel;
    [SerializeField] private GameObject _treatSubPanel;
    [SerializeField] private GameObject _patientHealthBar;
    [SerializeField] private TMP_Text _patientNameText;
    [SerializeField] private TMP_Text _injuryDetailsText;
    [SerializeField] private UnityEngine.UI.Slider _patientHealthSlider;

    private Patient currentPatient;
    

    public void Show(Patient patient)
    {
        currentPatient = patient;
        gameObject.SetActive(true);

        bool blocked = !patient.IsInteractable;
        _blockedPanel.SetActive(blocked);
        _basicInfoPanel.SetActive(true);
        _injuryInfoPanel.SetActive(patient.Analysed);
        _patientHealthBar.SetActive(patient.Analysed);
        _triageSubPanel.SetActive(false);
        _treatSubPanel.SetActive(false);

        _patientNameText.text = patient.Name;

        if (patient.Analysed) BuildInjurySummary();
    }

    private void BuildInjurySummary()
        {
        _injuryDetailsText.text = currentPatient.GetInjurySummary();
        }

    public void UpdateHealthSlider(Patient patient)
    {
        float NormaliseHealth()
        {
            return Mathf.Clamp01(patient.CurrentLife / patient.LifeTotal);
        }
        
        if (patient != currentPatient) return;

        _patientHealthSlider.value = NormaliseHealth();
    }

    // Button Event Handlers
    public void OnAnalysedPressed()
    {
        if (currentPatient == null || !currentPatient.IsInteractable) return;
        currentPatient.Analysed = true;
        _injuryInfoPanel.SetActive(true);
        BuildInjurySummary();
        _patientHealthBar.SetActive(true);
    }

    public void OnTriagePressed() => _triageSubPanel.SetActive(!_triageSubPanel.activeSelf);
    public void OnTreatPressed() => _treatSubPanel.SetActive(!_treatSubPanel.activeSelf);

    public void OnBandagePressed()
    {
        GameManager.Instance.TryTreat(EquipmentType.Bandage, currentPatient);
        BuildInjurySummary();
    }
    
    public void OnCoolGelPressed()
    {
        GameManager.Instance.TryTreat(EquipmentType.CoolGel, currentPatient);
        BuildInjurySummary();
    }
    public void OnNeutraliserPressed()
    {
        GameManager.Instance.TryTreat(EquipmentType.Neutraliser, currentPatient);
        BuildInjurySummary();
    }
}
