using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ResultsPanel : MonoBehaviour
{
    public TMP_Text majScoreText;
    public TMP_Text minScoreText;
    public TMP_Text failedScoreText;
    public TMP_Text pScoreText;
    public TMP_Text finalScoreText;

    public void Show(SceneResult result, int fScore)
    {
        majScoreText.text = $"Major Injuries Treated ({result.majorsTreated}) x ({GameManager.MajorsMultiplier})";
        minScoreText.text = $"+ Minor Injuries Treated ({result.minorsTreated}) x ({GameManager.MinorsMultiplier})";
        failedScoreText.text = $"- Failed Treatments ({result.failedTreatments}) x ({GameManager.FailureMultiplier})";
        pScoreText.text = $"x Patients Fully Treated ({result.patientsTreated})";
        finalScoreText.text = $"= Final Score: {fScore}";
    }
}
