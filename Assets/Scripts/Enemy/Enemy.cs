using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public UnityEvent OnObjectDestroyed = new UnityEvent();
    private byte m_LifePoints = 3;
    private Rigidbody2D m_Rigidbody;
    private float m_Speed;
    private bool m_IsToRight = true;
    public float Speed 
    { 
        get => m_Speed * Direction; 
        set => m_Speed = value; 
    }
    public float Direction
    {
        get => m_IsToRight ? -1 : 1;
    }
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        Debug.Assert(m_Rigidbody != null, "Enemy rigidbody is null ");
    }

    void OnDestroy()
    {
        OnObjectDestroyed.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            if (Waskilled())
            {
                Destroy(gameObject);
            }
            else
            {
                m_LifePoints--;
            }
        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        this.m_Rigidbody.velocity = new Vector2(Speed, this.m_Rigidbody.velocity.y);
    }

    internal bool Waskilled()
    {
        return m_LifePoints - 1 <= 0;
    }

    internal void PlayerKilled()
    {
        Destroy(gameObject);
    }
}