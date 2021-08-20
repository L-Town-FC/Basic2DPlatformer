using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxisRaw("Horizontal"); //horizontal movement input

        if(Input.GetButtonDown("Jump") && grounded) //lets player jump only when grounded
        {
            velocity.y = jumpTakeOffSpeed;
        }else if (Input.GetButtonUp("Jump")) //lets player cancel jump. when player lets go of jump it reduces their vertical velocity by half until it is zero
        {
            if(velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        targetVelocity = move * maxSpeed;
    }
}
