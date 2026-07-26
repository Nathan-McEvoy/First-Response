using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ResultsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _majScoreText;
    [SerializeField] private TMP_Text _minScoreText;
    [SerializeField] private TMP_Text _failedScoreText;
    [SerializeField] private TMP_Text _pScoreText;
    [SerializeField] private TMP_Text _finalScoreText;

    public void Show(SceneResult result, int fScore)
    {
        _majScoreText.text = $"Major Injuries Treated ({result.majorsTreated}) x ({GameManager.MajorsMultiplier})";
        _minScoreText.text = $"+ Minor Injuries Treated ({result.minorsTreated}) x ({GameManager.MinorsMultiplier})";
        _failedScoreText.text = $"- Failed Treatments ({result.failedTreatments}) x ({GameManager.FailureMultiplier})";
        _pScoreText.text = $"x Patients Fully Treated ({result.patientsTreated})";
        _finalScoreText.text = $"= Final Score: {fScore}";
    }
}
