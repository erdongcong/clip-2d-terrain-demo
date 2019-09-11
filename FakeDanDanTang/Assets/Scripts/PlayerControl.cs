using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Player m_Character;
    private bool m_Jump;


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

        bool fire = Input.GetButtonDown("Fire1");
        if (fire)
        {
            Debug.Log("Fire Input: " + Input.GetAxis("Fire1"));
            m_Character.Fire();
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
