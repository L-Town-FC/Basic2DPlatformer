using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Vector2 move;
    Rigidbody2D rb2d;
    public float speed = 7f;
    List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
    bool grounded;
    bool onWall;
    public float jumpHeight = 4;
    float jumpVelocity;
    float maxSlopeHeight = 0.65f;
    Vector2 groundNormal;
    Vector2 moveDirection;
    public float neutralWallJumpAngle;
    public float neutralWallJumpVelocity;
    public float towardsWallJumpAngle;
    public float towardsWallJumpVelocity;

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
        onWall = false;
        rb2d.GetContacts(contactPoints);
        for(int i = 0; i < contactPoints.Count; i++)
        {
            if(contactPoints[i].normal.y > maxSlopeHeight) //checks if player is ground that is shallow ground. If not, character will slide, NOT IMPLEMENTED YET
            {
                grounded = true;
                groundNormal = contactPoints[i].normal; //used to calculate direction that player should move along slope
                Debug.DrawRay(contactPoints[i].point, new Vector2(groundNormal.y, -groundNormal.x), Color.red);

            }

            if(Mathf.Abs(contactPoints[i].normal.x) == 1) //Checks if player is on a wall
            {
                onWall = true;
                groundNormal = contactPoints[i].normal;
            }

            Debug.DrawRay(contactPoints[i].point, contactPoints[i].normal);
        }

        move.x = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) //Move this into separate player controller script
        {
            if (grounded)
            {
                jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.magnitude * rb2d.gravityScale) * jumpHeight);
                rb2d.velocity = Vector2.up * jumpVelocity;
            }else if (onWall)
            {
                jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.magnitude * rb2d.gravityScale) * jumpHeight);

                if(move.x == 0)
                {
                    rb2d.velocity = new Vector2(Mathf.Cos(neutralWallJumpAngle * Mathf.Deg2Rad) * groundNormal.x, Mathf.Sign(Mathf.Sin(neutralWallJumpAngle * Mathf.Deg2Rad))) * neutralWallJumpVelocity; //makes sure jump angle is always positive
                }
                else
                {
                    rb2d.velocity = new Vector2(Mathf.Cos(towardsWallJumpAngle * Mathf.Deg2Rad) * groundNormal.x, Mathf.Sign(Mathf.Sin(towardsWallJumpAngle * Mathf.Deg2Rad))) * towardsWallJumpVelocity; //makes sure jump angle is always positive
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (grounded)//if grounded additional checks are needed in case there is a slope
        {
            moveDirection = new Vector2(groundNormal.y, -groundNormal.x);
        }
        else
        {
            moveDirection = Vector2.right;
        }
        rb2d.transform.Translate(moveDirection * move.x * speed * Time.deltaTime);

    }
}
