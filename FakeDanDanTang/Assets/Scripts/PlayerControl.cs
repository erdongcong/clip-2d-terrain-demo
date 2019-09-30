using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Player m_Character;
    private bool m_Jump;
    private float m_PowerAccumulationTime = 0.0f;


    private void Awake()
    {
        m_Character = GetComponent<Player>();
    }


    private void Update()
    {
        if (!m_Jump)
        {
            // Read the jump input in Update so button presses aren't missed.
            m_Jump = Input.GetButtonDown("Jump");
        }

        if (Input.GetAxis("Fire1") > 0.00001f)
        {
            m_PowerAccumulationTime += Time.deltaTime;
        }

        bool fire = Input.GetButtonUp("Fire1");
        if (fire)
        {
            m_Character.Fire(m_PowerAccumulationTime);
            m_PowerAccumulationTime = 0.0f;
        }
    }


    private void FixedUpdate()
    {
        // Read the inputs.
        float h = Input.GetAxis("Horizontal");
        // Pass all parameters to the character control script.
        m_Character.Move(h, m_Jump);

        float v = Input.GetAxis("Vertical");
        m_Character.Aim(v);

        m_Jump = false;
    }
}
