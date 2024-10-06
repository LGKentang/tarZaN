using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEditor.Animations;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public DialogueManager dialogueManager;

    [Header("Animation")]
    public Animator animation;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Player Capsule")]
    public CapsuleCollider capsuleCollider;
    private float normalCrouchValue;
    private float currentYCapsuleCenter;
    const float yCenterCrouchValue = .59f;
    const float crouchedValue = 1.7f;

    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    public float walkSpeed;
    public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    public bool climbing;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Camera camera;

    public static bool isNPCinteractable;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    public bool isPunching;

    Vector3 moveDirection;
    public bool isPause;
    Rigidbody rb;

    bool bearHit;

    public MovementState state;

    public GameObject IsAttackingWhat;

    public Canvas UICanvas;
    public Canvas PauseCanvas;

    // Sounds
    public AudioSource grassSound;


    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        isNPCinteractable = false;
        isPunching = false;
        isPause = false;
        readyToJump = true;
        PauseCanvas.enabled = false;

        startYScale = transform.localScale.y;
        normalCrouchValue = capsuleCollider.height;
        currentYCapsuleCenter = capsuleCollider.center.y;
    }

    private void Update()
    {
        //print(moveSpeed);
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight, whatIsGround);
  
        MyInput();
        SpeedControl();
        StateHandler();

        //print(this.rb.velocity.y);

        if (rb.velocity.y < -15f)
        animation.SetBool("isFalling", true);
        else
        animation.SetBool("isFalling", false);
        

        if (grounded)
        rb.drag = groundDrag;   
        else
        rb.drag = 0;
        

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()

    {
        //print(isNPCinteractable);


      if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause) isPause = false;
            else isPause = true;

            if (isPause)
            {
                PauseCanvas.enabled = true;
                UICanvas.enabled = false;
                UnityEngine.Cursor.visible = true;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }
            else
            {
                PauseCanvas.enabled = false;
                UICanvas.enabled = true;
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }
            
        }

        if (!isPause)
        {



            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            if (horizontalInput != 0 || verticalInput != 0)
            {
                animation.SetBool("isWalking", true);

            }
            else
            {
                animation.SetBool("isWalking", false);
            }

            if (Input.GetKey(KeyCode.Mouse0) && isPunching == false && !isNPCinteractable)
            {
                print("Punch");
                StartCoroutine(PunchCooldown());
            }

            if (Input.GetKey(jumpKey) && readyToJump && grounded && !isNPCinteractable)
            {


                readyToJump = false;

                Jump();
                animation.SetTrigger("jumpTrigger");

                Invoke(nameof(ResetJump), jumpCooldown);


            }

            if (Input.GetKeyDown(jumpKey) && isNPCinteractable)
            {
                if (dialogueManager.sentences.Count > 0) dialogueManager.DisplayNextSentence();
            }

            if (Input.GetKeyDown(crouchKey))
            {
                animation.SetBool("isCrouch", true);
                capsuleCollider.height = crouchedValue;
                capsuleCollider.center = new Vector3(capsuleCollider.center.x, yCenterCrouchValue, capsuleCollider.center.z);

                // ...

                Vector3 now = camera.transform.position;
                camera.transform.position = new Vector3(now.x, now.y - 1f, now.z);
            }
            if (Input.GetKeyUp(crouchKey))
            {
                animation.SetBool("isCrouch", false);
                Vector3 now = camera.transform.position;
                camera.transform.position = new Vector3(now.x, now.y + 1f, now.z);
                capsuleCollider.height = normalCrouchValue;
                capsuleCollider.center = new Vector3(capsuleCollider.center.x, currentYCapsuleCenter, capsuleCollider.center.z);
            }
        }




    }


    IEnumerator PunchCooldown()
    {
        isPunching = true;


        print("Punch");
        animation.SetTrigger("punchTrigger");

        yield return new WaitForSeconds(.7f); 


    
        isPunching = false;
        bearHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("TDEnemy"))
        {
            //print("Trigger");
            TDEnemy tde = other.GetComponent<TDEnemy>();
            if (tde.gameObject != null)
            {
                print(tde.health);
                tde.TakeDamage((int)PlayerAttributes.GetInstance().Attack);

                if (tde.gameObject == null || tde.IsDead())
                {
                    TDManager.enemyKilled++;
                    print("death");
                    PlayerAttributes.GetInstance().AddExperience(10);
                }
            }
        }
        if (other.CompareTag("Dragon"))
        {
            //print("Trigger");
            DragonController dg = other.GetComponent<DragonController>();
            if (dg != null & !dg.isDead)
            {
                dg.TakeDamage(20);

                if (dg.isDead)
                {
                    EndScene.IsWon = true;
                    SceneManager.LoadScene("EndScene");
                    print("Add XP");
                    PlayerAttributes.GetInstance().AddExperience(dg.health);
                }
            }
        }

        if (other.CompareTag("Bear"))
        {

            AnimalController bear = other.GetComponent<AnimalController>();
            if (bear != null && isPunching && !bearHit && !bear.becomePet)
            {
                IsAttackingWhat = bear.gameObject;
                
                StartCoroutine(PunchCooldown());

                bear.TakeDamage((int)PlayerAttributes.GetInstance().Attack);
                if (bear.becomePet)
                {
                    //print("jadi pet");
                    PlayerAttributes.GetInstance().AddExperience(bear.health);

                }
                bearHit = true;
            }
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            //print("crouching");
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        else if (grounded && Input.GetKey(sprintKey))
        {
            animation.SetBool("isSprinting", true);
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            animation.SetBool("isSprinting", false);
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
       
        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        //rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

   
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}