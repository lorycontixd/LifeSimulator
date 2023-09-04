using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastScorePanelLobby : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI detailScoreText;
    [SerializeField] private Button closeButton;
    public bool CanBeOpened = true;

    private ScoreManager.Scores scores = null;


    private void Start()
    {
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Open()
    {
        if (!CanBeOpened)
        {
            return;
        }
        if (scores == null)
        {
            scores = ScoreManager.Instance.lastScores;
            if (scores == null)
            {
                Debug.LogWarning($"[LastscoresPanel] scores is null!");
                return;
            }
        }
        gameObject.SetActive(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {scores.Score}";
        detailScoreText.text = $"" +
            $"- Game Days Survived: {scores.GameDaysSurvived}\n" +
            $"- Total Seconds Survived: {scores.SecondsSurvived}\n" +
            $"- Final Money: {scores.FinalMoney}\n" +
            $"- Diseases Cured: {scores.DiseasesCured}\n" +
            $"- Buildings Constructed: {scores.BuildingsConstructed}\n" +
            $"- Money From Investments: {scores.MoneyGainedFromInvestments}";
    }

    public void SetScores(ScoreManager.Scores scores)
    {
        this.scores = scores;
    }
}
