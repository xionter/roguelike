using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float movSpeed;
    private float SpeedX;
    private float SpeedY;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        SpeedX = Input.GetAxis("Horizontal") * movSpeed;
        SpeedY = Input.GetAxis("Vertical") * movSpeed;
        rb.linearVelocity = new Vector2(SpeedX, SpeedY);
        
    }
}
