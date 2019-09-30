using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField] private float o_RotateSpeed = 5.0f;
    [SerializeField] private float o_MaxPowerAccumulationTime = 3.0f;
    [SerializeField] private float o_MaxFireForce = 500.0f;
    [SerializeField] private GameObject o_ShellPrefab = null;
    [SerializeField] private Transform o_GunBarrelTrans = null;
    [SerializeField] private Transform o_ShellSpawnTrans = null;

    private Rigidbody2D m_Rigidbody2D;
    private bool m_IsFaceTheRight = true;


    private void Awake()
    {
        // Setting up references.
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        o_GunBarrelTrans.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void Move(float move, bool jump)
    {
        // Move the character
        m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
        if(Mathf.Abs(move) > 0.0001)
        {
            m_IsFaceTheRight = move > 0.0f;
            transform.localScale = new Vector3(m_IsFaceTheRight ? 1.0f : -1.0f, 1.0f, 1.0f);
        }

        // If jump
        if(jump)
        {
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    public void Fire(float accTime)
    {
        float force = (accTime > o_MaxPowerAccumulationTime ? 1.0f : accTime / o_MaxPowerAccumulationTime) * o_MaxFireForce;
        float fireDir = (m_IsFaceTheRight ? o_GunBarrelTrans.localRotation.eulerAngles.z : 180 - o_GunBarrelTrans.localRotation.eulerAngles.z) * Mathf.Deg2Rad;
        Instantiate(o_ShellPrefab, o_ShellSpawnTrans.position, Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(new Vector2(Mathf.Cos(fireDir) * force, Mathf.Sin(fireDir) * force));
    }

    public void Aim(float rotate)
    {
        SetGunBarrelRotation(rotate * o_RotateSpeed);
    }

    private void SetGunBarrelRotation(float angle)
    {
        o_GunBarrelTrans.Rotate(0, 0, angle);
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
