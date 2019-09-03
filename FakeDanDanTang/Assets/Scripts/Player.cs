using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.


    private Rigidbody2D m_Rigidbody2D;


    private void Awake()
    {
        // Setting up references.
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Move(float move, bool jump)
    {
        // Move the character
        m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

        // If jump
        if(jump)
        {
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player Collide Enter " + collision.gameObject.name);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Player Collide Stay " + collision.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Player Collide Exit " + collision.gameObject.name);
    }
}
