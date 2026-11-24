using UnityEngine;

public class feetScript : MonoBehaviour
{
    public bool isGrounded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
}
