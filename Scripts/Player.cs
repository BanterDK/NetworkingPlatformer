using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    // ALT + SHIFT + (UP/DOWN)
    public int health = 100;
    public int stamina = 50;
    public int armor = 100;
    public float jumpHeight = 10;
    public float movementSpeed = 20;
    public float rayDistance = 1;
    public float movementDampening = 0.5f;
    public LayerMask layerMask;

    private bool isJumping = false;
    private bool canJump = true;
    private bool isSpaceDown = false;
    private Camera cam;
    private Rigidbody2D rigid;
    private MeshRenderer renderer;
    private Vector2 hitNormal;


    // Use this for initialization
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<MeshRenderer>();
        HandleNetwork();
    }

    void HandleNetwork()
    {
        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            Jump();
            Movement();
            //OnCollisionStay2D();
        }
    }

    void FixedUpdate()
    {
        //Perform raycast underneath player to ground

        //Code cleanup
        Bounds bounds = renderer.bounds;
        Vector3 size = bounds.size;
        Vector3 scale = transform.localScale;
        float height = (size.y * 0.5f) * scale.y;

        //Create a ray form  the bottom of the player
        Vector3 origin = transform.position + Vector3.down * height;
        Vector3 direction = Vector3.down;

        //Perform raycast
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, ~layerMask.value);
        Debug.DrawLine(origin, origin + direction * rayDistance, Color.red);

        //Check if we hit something with the Ray
        if (hit.collider != null)
        {
            isJumping = false;
        }
    }
    void Jump()
    {
        //If we are not yet jumping
        if (!isJumping)
        {
            //Obtain input for jump
            float inputVert = Input.GetAxis("Jump");
            if (inputVert > 0 && canJump)
            {
                canJump = false;
                //Appending movement formula
                rigid.AddForce(Vector3.up * inputVert * jumpHeight, ForceMode2D.Impulse);
                isJumping = true;
            }
            if (inputVert == 0)
            {
                canJump = true;
            }
        }
    }

    void Movement()
    {
        //Obtain input for movement/horizontal
        float inputHoriz = Input.GetAxis("Horizontal");

        rigid.velocity = new Vector2(Mathf.Lerp(rigid.velocity.x, 0, movementDampening), rigid.velocity.y);
        //Append movement formula 
        //move right
        if (inputHoriz > 0 && hitNormal.x >= 0)
        {
            rigid.velocity = new Vector2(movementSpeed, rigid.velocity.y);
        }
        if (inputHoriz < 0 && hitNormal.x <= 0)
        {
            rigid.velocity = new Vector2(-movementSpeed, rigid.velocity.y);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        //Obtains the contact normal of the collision
        hitNormal = col.contacts[0].normal;
        Vector3 point = col.contacts[0].point;
        //Draw a line from  the collision point to direction of the normal
        Debug.DrawLine(point, point + (Vector3)hitNormal * 5, Color.red);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        hitNormal = Vector2.zero;

    }
}
