using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.SceneManagement;


public class PlayerMain : MonoBehaviour
{
    Rigidbody2D rb;

    public Transform groundCheck;
    //public float groundDistance
    public LayerMask groundMask;
    public bool isGrounded;

    public float grav = 4.0f;
    public float m_spd = 10.0f;
    public float j_spd = 15.0f;

    void Start() // INICIO
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = grav;
    }

    void Update()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector2.down, 0.2f, groundMask);

        //float _dir = Mathf.Sign(Keyboard.current.dKey.isPressed - Keyboard.current.aKey.isPressed);

        if (Keyboard.current.dKey.isPressed)
            rb.linearVelocityX = 100 * m_spd * Time.deltaTime;

        if (Keyboard.current.aKey.isPressed)
            rb.linearVelocityX = 100 * -m_spd * Time.deltaTime;

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
            rb.AddForce(Vector3.up * j_spd, ForceMode2D.Impulse);
    }
}
