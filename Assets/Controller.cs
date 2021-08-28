using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected Vector2 move;
    protected Vector2 velocity;
    protected Rigidbody2D rb2d;
    protected float speed = 7f;
    protected List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
    protected bool grounded;
    protected bool onWall;
    protected float jumpHeight = 4;
    protected float jumpVelocity;
    protected float maxSlopeHeight = 0.65f;
    protected Vector2 groundNormal;
    protected Vector2 moveDirection;
    protected float neutralWallJumpAngle = 10;
    protected float neutralWallJumpVelocity = 20;
    protected float towardsWallJumpAngle = 65;
    protected float towardsWallJumpVelocity = 20;

    // Start is called before the first frame update
    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    //move inputs into separate script so this script can be applied to all physics objects
    protected virtual void ComputeVelocity() //function that is overridden in classes that inherit physics object class
    {

    }


    // Update is called once per frame
    void Update()
    {
        ComputeVelocity();
    }


    protected void FixedUpdate()
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
