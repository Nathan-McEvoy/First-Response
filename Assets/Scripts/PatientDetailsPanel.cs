using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject conditionIconPrefab;
    [SerializeField] private TMP_Text _patientNameText;
    [SerializeField] private TMP_Text _injuryDetailsText;
    [SerializeField] private Transform conditionIconContainer;
    [SerializeField] private List<ConditionIconEntry> _conditionIcons;
    [SerializeField] private UnityEngine.UI.Slider _patientHealthSlider;

    private Dictionary<ConditionType, Sprite> _conditionIconLookup;

    private Patient currentPatient;
    
    void Awake()
    {
        _conditionIconLookup = _conditionIcons.ToDictionary(e => e.type, e => e.icon);
    }

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

        if (blocked)
        {
            DisplayConditionIcons(patient);
        }
        if (patient.Analysed) BuildInjurySummary();
    }

    private void DisplayConditionIcons(Patient patient)
    {
        foreach (Transform child in conditionIconContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var condition in patient.Conditions)
        {
            if (condition.Resolved) continue;

            GameObject iconGO = Instantiate(conditionIconPrefab, conditionIconContainer);
            iconGO.GetComponent<UnityEngine.UI.Image>().sprite = _conditionIconLookup[condition.Type];
        }
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

    // Button Event Handlers //
    // Analyse Button
    public void OnAnalysedPressed()
    {
        if (currentPatient == null || !currentPatient.IsInteractable) return;
        currentPatient.Analysed = true;
        _injuryInfoPanel.SetActive(true);
        BuildInjurySummary();
        _patientHealthBar.SetActive(true);
    }

    // Sub-Panel Buttons
    public void OnTriagePressed() => _triageSubPanel.SetActive(!_triageSubPanel.activeSelf);
    public void OnTreatPressed() => _treatSubPanel.SetActive(!_treatSubPanel.activeSelf);

    // Treat Injury Buttons
    public void OnBandagePressed() => OnTreatInjuryPressed(EquipmentType.Bandage);
    public void OnCoolGelPressed() => OnTreatInjuryPressed(EquipmentType.CoolGel);
    public void OnNeutraliserPressed() => OnTreatInjuryPressed(EquipmentType.Neutraliser);

    private void OnTreatInjuryPressed(EquipmentType type)
    {
        GameManager.Instance.TryTreat(type, currentPatient);
        BuildInjurySummary();
    }

    // Treat Condition Buttons
    public void OnWaterPressed() => OnTreatConditionPressed(EquipmentType.Water);
    public void OnExtracatorPressed() => OnTreatConditionPressed(EquipmentType.Extracator);
    public void OnRubberGlovesPressed() => OnTreatConditionPressed(EquipmentType.RubberGloves);

    private void OnTreatConditionPressed(EquipmentType type)
    {
        GameManager.Instance.TryResolveCondition(type, currentPatient);
        DisplayConditionIcons(currentPatient);

        if (currentPatient.IsInteractable)
        {
            _blockedPanel.SetActive(false);
        }
    }
}
