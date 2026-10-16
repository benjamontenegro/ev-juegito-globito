using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.SceneManagement;


public class PlayerMain : MonoBehaviour
{
    Rigidbody2D rb;
    bool isGrounded;

    float grav = 4.0f;
    float m_spd = 15.0f;
    float j_spd = 17f;

    float y_pos_ground = 0.6f;

    void Start() // INICIO
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = grav;
    }

    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, 
            Vector2.down, y_pos_ground, LayerMask.GetMask("Wall"));

        if (Keyboard.current.dKey.isPressed)
            rb.linearVelocityX = 100 * m_spd * Time.deltaTime;

        if (Keyboard.current.aKey.isPressed)
            rb.linearVelocityX = 100 * -m_spd * Time.deltaTime;

        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
            rb.AddForce(Vector3.up * j_spd, ForceMode2D.Impulse);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -y_pos_ground));
    }
}
