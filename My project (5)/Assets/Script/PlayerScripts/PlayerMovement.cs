using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Run variables
    private Rigidbody2D rb;
    public float playerSpeed;
    private float xAxis;
    
    //Jump variables
    public Transform groundCheckPoint;
    public float groundCheckY = 0.2f;
    public float groundCheckX = 0.5f;
    public LayerMask whatIsGround;
    public float jumpForce = 45f;


    //Jump Buffer
    PlayerStateList pState;
    private int jumpBufferCounter;
    public int jumpBufferFrames;

    //Coyote Time
    private float coyoteTimeCounter = 0;
    public float coyoteTime;

    //DoubleJump
    private int airJumpCounter = 0;
    public int maxAirJumps = 1;

    //Dash
    private bool canDash = true;
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    private float gravity;
    public bool dashed;

    //Dash Timers
    public float dashTimer = 0;
    public float dashCooldownTimer = 0;


    //Camera Flip
    public bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInputs();
        JumpVariables();

        //While the player is dashing we cannot do anything else
        if (pState.dashing)
        {
            HandleDash();
            return;
        }

        Run();
        Jump();
        Flip();
        StartDash();

        // Cooldown for the dash
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
            {
                canDash = true;
            }
        }
    }

    void Flip()
    {
        //We transform the rotation in the y axis because it respects the script
        //for the camera controll and actually turns the perspective of the camera with each flip
        if (xAxis < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (xAxis > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }




    void GetPlayerInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }
    void Run()
    {
        rb.velocity = new Vector2(playerSpeed * xAxis, rb.velocity.y);
    }
    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            //We apply all the cooldowns before actually dashing and then add a boost of speed that we decide with a public float 
            pState.dashing = true;
            dashTimer = dashTime;
            canDash = false;
            dashCooldownTimer = dashCooldown;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
            dashed = true;
        }
        //Once we are touching the ground we can start another dash after its cooldown
        if (Grounded())
        {
            dashed = false;
        }
    }
    //We desactivate gravity while doing the dash and apply DeltaTime
    void HandleDash()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            pState.dashing = false;
            rb.gravityScale = gravity;
        }
    }
    
    bool Grounded()
    {
        //Easy Raycast script not entirely functional
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        } else
        {
            return false;
        }
    }

    void Jump()
    {
        //We are only able to jump while we are touching the gorund, and if we have not already jumped instantly before 
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            //We can jump when we are on the ground plus the time it gives us the CoyoteTime
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jumping = true;

            }else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                //If we have jumped, we update the player state while adding a count of the jumpCounter
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

            }
        }

    }

    //Easy coyote time function
    void JumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else {
            coyoteTimeCounter -= Time.deltaTime; 
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else if (jumpBufferCounter > 0)
        {
            jumpBufferCounter--;
        }
    }
    
    
}
