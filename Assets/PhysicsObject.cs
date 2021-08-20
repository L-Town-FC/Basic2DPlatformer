using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = 0.65f;
    public float gravityModifier = 1f; //can change gravity for inherited objects

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d; //objects rigidbody
    protected Vector2 velocity; //protected so it can only be access by classes that inherit from this class
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f; //minimum distance an object must try to move before collisions are checked. Makes it so collisions aren't checked too many times which could stress system
    protected const float shellRadius = 0.01f; //padding added to distance to make sure rb2d never gets stuck in other collider

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false; //collisions with triggers aren't checked
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer)); //getting layermask from project settings of physics2D. This makes sure physics2d settings are used for what layers collisions are checked on  
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = Vector2.zero; //zero out velocity before applying new velocity
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity() //function that is overridden in classes that inherit physics object class
    {

    }

    private void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime; //
        velocity.x = targetVelocity.x; //input velocity from player controller

        grounded = false; //defaults grounded state to false. Only changes when a collision is detected

        Vector2 deltaPosition = velocity * Time.deltaTime; //change in position due to velocity

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x); //direction we are traveling whether it be sloped ground or flat ground

        Vector2 move = moveAlongGround * deltaPosition.x; //x component of move

        Movement(move, false); //calculating x movement

        move = Vector2.up * deltaPosition.y; //move vector that causes the object to move

        Movement(move, true);
    }

    void Movement( Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if(distance > minMoveDistance) {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius); //array of collider contacts
            hitBufferList.Clear(); //makes sure list is empty before adding to it
            for (int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]); //moves array of contacts into a list
            }

            for(int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                //this controller wont let players slide down slopes unless additonal stuff is added
                if(currentNormal.y > minGroundNormalY) //checks angle of object we are standing on. If angle is shallower than the minimum angle we are ground. Else we are falling/sliding
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

              

                float projection = Vector2.Dot(velocity,currentNormal); //getting difference between the velocity and current normal and determing whether we need to subtract from our velocity to stop object from entering a collider
                if(projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance; //checks whether the distance traveled is smaller than the shell distance. If it is, the shell distance is used to stop object from going into collider

            }
        }


        rb2d.position = rb2d.position + move.normalized * distance; //moves rigid body move amount
    }
}
