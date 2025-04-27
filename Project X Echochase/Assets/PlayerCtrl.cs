using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    // Start после первого кола Update
    public float movSpeed;
    private float SpeedX;
    private float SpeedY;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // update колится каждый фрейм 
    void Update()
    {
        SpeedX = Input.GetAxis("Horizontal") * movSpeed;
        SpeedY = Input.GetAxis("Vertical") * movSpeed;
        rb.linearVelocity = new Vector2(SpeedX, SpeedY);
        
    }
}
