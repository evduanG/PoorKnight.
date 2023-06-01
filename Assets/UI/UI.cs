using System;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public const string k_FormtToShow = @"Score: {0}";
    private int m_Score;
    [SerializeField]
    private Text m_ScoreText;
    [SerializeField]
    private Text m_GameOverText;
    [SerializeField]
    private Text m_OrderText;

    public int Score
    {
        get => m_Score;
        set
        {
            m_Score = Math.Max(0, value);
            updatingText();
        }
    }

    private void Start()
    {
        if (m_ScoreText is null)
        {
            Debug.LogError("cont find the scoreText Text element");
        }
        else
        {
            Score = 0;
        }

        if (m_GameOverText is null)
        {
            Debug.LogError("cont find the gameOverText Text element");
        }
        else
        {
            m_GameOverText.enabled = false;
        }
    }

    private void updatingText()
    {
        m_ScoreText.text = string.Format(k_FormtToShow, m_Score);
    }

    public void Playerkilled()
    {
        PlayersFirstShotWasFired();
        m_GameOverText.enabled = true;
    }

    public void PlayersFirstShotWasFired()
    {
        m_OrderText.enabled = false;
    }
}
