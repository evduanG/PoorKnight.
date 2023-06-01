using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.player
{
    public class PlayerController : MonoBehaviour
    {
        public const string k_ArrowSpawnPoint = "ArrowSpawnPoint";
        public static PlayerController Instance { get; private set; }
        public bool m_IsCollidingWithEnemy;
        public bool m_IsOnGround = true;
        public GameObject m_ArrowPrefab;

        private int m_Score;
        private float m_LastShootTime;
        private Rigidbody2D m_RigidbodyPlayer;
        private Transform m_ArrowSpawnPoint;
        private SpriteRenderer m_FlipPlayer;
        private Animator m_Anim;

        public UnityEvent OnPlayerDying = new UnityEvent();

        [SerializeField]
        [Range(5f, 100f)]
        private float m_Jump;
        [SerializeField]
        private float m_Speed = 35f;
        [SerializeField]
        private float m_ShootCooldown = 0.45f;
        [SerializeField]
        private UI m_PlayerUi;
        private bool m_IsFirstShot;

        public bool IsJump => Input.GetKey(KeyCode.Space) && m_IsOnGround;
        public bool IsShoot => Input.GetKeyDown(KeyCode.E);

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            m_Score = 0;
            m_RigidbodyPlayer = GetComponent<Rigidbody2D>();
            m_IsFirstShot = true;
        }

        private void Start()
        {
            m_ArrowSpawnPoint = transform.Find(k_ArrowSpawnPoint);
            Debug.Assert(m_ArrowSpawnPoint is not null, "m_ArrowSpawnPoint is null");
            m_Anim = GetComponentInChildren<Animator>();
            m_LastShootTime = Time.time;
            m_FlipPlayer = GetComponentInChildren<SpriteRenderer>();

            if (m_PlayerUi is null)
            {
                Debug.LogError("playerUi is null");
            }
            else
            {
                OnPlayerDying.AddListener(m_PlayerUi.Playerkilled);
            }
        }


        private void Update()
        {
            Walk();
            UpdateJump();
            UpdateShooting();
        }

        private void UpdateShooting()
        {
            bool runShootingAnimation = false;

            if (IsShoot)
            {
                if (HasPlayerCooldownExpired())
                {
                    StartCoroutine(ArrowGenerator());

                    if (m_IsFirstShot)
                    {
                        m_PlayerUi.PlayersFirstShotWasFired();
                        m_IsFirstShot = false;
                    }
                    runShootingAnimation = true;
                }
            }

            m_Anim.SetBool("isShooting", runShootingAnimation);
        }

        private void UpdateJump()
        {
            bool runJumpingAnimation = false;

            if (IsJump)
            {
                runJumpingAnimation = true;
                m_RigidbodyPlayer.velocity = new Vector2(0, m_Jump);
                m_IsOnGround = false;
            }

            m_Anim.SetBool("isJumping", runJumpingAnimation);
        }

        IEnumerator ArrowGenerator()
        {
            yield return new WaitForSeconds(0.3f);
            if (m_ArrowPrefab is null)
            {
                Debug.Log("m_ArrowPrefab is null");
            }
            GameObject arrow = Instantiate(m_ArrowPrefab, m_ArrowSpawnPoint.position, Quaternion.Euler(0, 0, -90));
            arrow.GetComponent<ArrowShooting>().onKillingEnemy.AddListener(KillEnemy);
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(70f, 3f);
        }

        private void KillEnemy()
        {
            m_Score++;
            m_PlayerUi.Score = m_Score;
        }

        private void Walk()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            m_RigidbodyPlayer.velocity = new Vector2(moveHorizontal * m_Speed, m_RigidbodyPlayer.velocity.y);
            bool runWalkingAnimation = false;

            if (moveHorizontal > 0.01f)
            {
                m_FlipPlayer.flipX = false;
                runWalkingAnimation = true;
            }
            else if (moveHorizontal < -0.01f)
            {
                m_FlipPlayer.flipX = true;
                runWalkingAnimation = true;
            }

            m_Anim.SetBool("isWalking", runWalkingAnimation);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                PlayerDie();
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                m_IsOnGround = true;
            }
        }

        private void PlayerDie()
        {
            m_Anim.SetBool("isDie", true);
            m_IsCollidingWithEnemy = true;
            Destroy(gameObject, 0.3f);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                m_IsCollidingWithEnemy = false;
            }
        }

        private bool HasPlayerCooldownExpired()
        {
            float timeSinceLastAction = Time.time - m_LastShootTime;
            bool canPlayrShootArrow = timeSinceLastAction >= m_ShootCooldown && !m_FlipPlayer.flipX;

            if (canPlayrShootArrow)
            {
                m_LastShootTime = Time.time;
            }

            return canPlayrShootArrow;
        }

        private void OnDestroy()
        {
            OnPlayerDying?.Invoke();
        }
    }
}
