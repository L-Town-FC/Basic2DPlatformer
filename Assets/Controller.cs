using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Vector2 move;
    Rigidbody2D rb2d;
    public float speed = 7f;
    List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
    bool grounded = false;
    public float jumpHeight = 4;
    float jumpVelocity;
    float maxSlopeHeight = 0.65f;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    //move inputs into separate script so this script can be applied to all physics objects

    // Update is called once per frame
    void Update()
    {
        grounded = false;
        rb2d.GetContacts(contactPoints);
        for(int i = 0; i < contactPoints.Count; i++)
        {
            if(contactPoints[i].normal.y > maxSlopeHeight)
            {
                grounded = true;
            }

            Debug.DrawRay(contactPoints[i].point, contactPoints[i].normal);
        }
        move.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && grounded)
        {
            jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.magnitude * rb2d.gravityScale) * jumpHeight);
            rb2d.velocity = Vector2.up * jumpVelocity;
        }
    }

    private void FixedUpdate()
    {
        rb2d.transform.Translate(move * speed* Time.deltaTime);
    }
}
