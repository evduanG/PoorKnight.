using Assets.player;
using System;
using System.Collections;
using UnityEngine;

public class EnemySponder : MonoBehaviour
{
    public const string k_SpanwnPoint = "EnemySpawnPoint";
    private float m_EnemySpawnTime = 1;
    private int m_EnemySppedCunt = 1;

    private byte m_NunOfLiveingEnemy;
    private Transform m_EnemySpawnPoint;
    private bool m_IsRunning;

    [SerializeField]
    private GameObject m_EnemyPrefab;
    [SerializeField]
    [Range(1, 20)]
    private byte m_MaxNumOfEnemy;

    [SerializeField]
    [Range(1f, 100)]
    private float m_MinSpeedForEnemy;

    public byte MunOfLiveingEnemy
    {
        get => m_NunOfLiveingEnemy;
        set => m_NunOfLiveingEnemy = (byte)(value > 0 ? value : 0);
    }
    public bool IsPlayerAlive 
    { 
        get => m_IsRunning; 
        set => m_IsRunning = value; 
    }

    private void Awake()
    {
        m_NunOfLiveingEnemy = 0;
    }

    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            IsPlayerAlive = true;
            PlayerController.Instance.OnPlayerDying.AddListener(() => this.IsPlayerAlive = false);
            m_EnemySpawnPoint = transform.Find(k_SpanwnPoint);
            StartCoroutine(EnemyGenerator());
        }
    }

    IEnumerator EnemyGenerator()
    {
        while (this.IsPlayerAlive)
        {
            yield return new WaitForSeconds(m_EnemySpawnTime);

            if (MunOfLiveingEnemy < m_MaxNumOfEnemy)
            {
                MunOfLiveingEnemy++;

                GameObject enemyUnityGO = Instantiate(m_EnemyPrefab, m_EnemySpawnPoint.position, Quaternion.identity);
                Enemy enemy = enemyUnityGO.GetComponent<Enemy>();

                if (IsPlayerAlive)
                {
                    PlayerController.Instance.OnPlayerDying.AddListener(enemy.PlayerKilled);
                    enemy.OnObjectDestroyed.AddListener(EnemyDestroy);
                    enemy.Speed = GetSpeedForNewEnemy();
                }
                else
                {
                    enemy.PlayerKilled();
                }
            }
        }
    }

    public float GetSpeedForNewEnemy()
    {
        m_EnemySppedCunt++;

        return (float)m_MinSpeedForEnemy + m_EnemySppedCunt / 10;
    }

    public void EnemyDestroy()
    {
        MunOfLiveingEnemy--;
    }
}