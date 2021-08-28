using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : Controller
{
    // Start is called before the first frame update
    void Start()
    {

    }

    protected override void ComputeVelocity()
    {
        //base.ComputeVelocity();
        if(velocity.magnitude != 0)
        {
            velocity = Vector2.zero;
        }
        grounded = false;
        onWall = false;
        rb2d.GetContacts(contactPoints);
        for (int i = 0; i < contactPoints.Count; i++)
        {
            if (contactPoints[i].normal.y > maxSlopeHeight) //checks if player is ground that is shallow ground. If not, character will slide, NOT IMPLEMENTED YET
            {
                grounded = true;
                groundNormal = contactPoints[i].normal; //used to calculate direction that player should move along slope
                Debug.DrawRay(contactPoints[i].point, new Vector2(groundNormal.y, -groundNormal.x), Color.red);

            }

            if (Mathf.Abs(contactPoints[i].normal.x) == 1) //Checks if player is on a wall
            {
                onWall = true;
                groundNormal = contactPoints[i].normal;
            }

            Debug.DrawRay(contactPoints[i].point, contactPoints[i].normal);
        }

        if (Input.GetButtonDown("Jump")) //Move this into separate player controller script
        {
            if (grounded)
            {
                jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.magnitude * rb2d.gravityScale) * jumpHeight);
                rb2d.velocity = Vector2.up * jumpVelocity;
            }
            else if (onWall)
            {
                jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.magnitude * rb2d.gravityScale) * jumpHeight);

                if (move.x == 0)
                {
                    rb2d.velocity = new Vector2(Mathf.Cos(neutralWallJumpAngle * Mathf.Deg2Rad) * groundNormal.x, Mathf.Sign(Mathf.Sin(neutralWallJumpAngle * Mathf.Deg2Rad))) * neutralWallJumpVelocity; //makes sure jump angle is always positive
                }
                else
                {
                    rb2d.velocity = new Vector2(Mathf.Cos(towardsWallJumpAngle * Mathf.Deg2Rad) * groundNormal.x, Mathf.Sign(Mathf.Sin(towardsWallJumpAngle * Mathf.Deg2Rad))) * towardsWallJumpVelocity; //makes sure jump angle is always positive
                }
            }
            velocity = rb2d.velocity;
        }
        move.x = Input.GetAxisRaw("Horizontal");
    }
}
