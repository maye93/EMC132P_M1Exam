using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    // public CharacterController controller;

    [Header("Movement")]
    float movementSpeed = 5f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public bool onJumpInput;
    public float jumpCooldown;
    bool readyToJump = true;

    bool isGrounded = true;

    public Transform orientation;

    float onHorizontalInput;
    float onVerticalInput;

    Vector3 movementDirection;
    public Transform playerObj;
    
    public Rigidbody rb;
          
    [SerializeField] GameObject onGround;

    float jumpCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    { 
        PlayerInput();   

        if(Input.GetKeyDown(KeyCode.Space) && readyToJump && isGrounded){

            isGrounded = false;
            readyToJump = false;

            Jump();

            Invoke(nameof(ReadyJump), jumpCooldown);
        }

        if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
        {
            rb.velocity = Vector3.zero;
            // Debug.Log("Released");
        }
    }

    
    public void PlayerInput(){
        onHorizontalInput = Input.GetAxis("Horizontal");
        onVerticalInput = Input.GetAxis("Vertical");    
    }

    private void PlayerMovement(){
        Vector3 direction = new Vector3(onHorizontalInput, 0f, onVerticalInput).normalized;

        if(direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            // controller.Move(moveDir * movementSpeed * Time.fixedDeltaTime);
        }
        
        movementDirection = orientation.forward * onVerticalInput + orientation.right * onHorizontalInput;
    }

    private void Jump(){
        jumpCount += 1;
        isGrounded = true;
        onJumpInput = false;
        rb.AddForce(Vector3.up*4f,ForceMode.VelocityChange);
    }

    private void ReadyJump(){
        readyToJump = true;
    }

    void FixedUpdate()
    {
        PlayerMovement();
        rb.MovePosition(rb.position+movementDirection*movementSpeed*Time.fixedDeltaTime);

        if (Physics.OverlapSphere(onGround.transform.position, 0.01f).Length == 1)        
        {
            return;
        }

        if (onJumpInput == true) {
            Jump();
        } else if (jumpCount == 2) {
            jumpCount = 0;
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin"){
            Debug.Log("Collected Coin.");
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}