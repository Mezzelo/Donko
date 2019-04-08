using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonkoController : MonoBehaviour {

    public float speed;
    public float jump;
    private Rigidbody rb;
    public bool isGrounded = true;

    public Animator donkoAnims;

    public void doDeath() {
        donkoAnims.SetTrigger("didDie");
    }

    Vector3 posLastTick;
    float donkoVeloc;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        posLastTick = transform.position;
    }

    private void FixedUpdate() {
        donkoVeloc = (transform.position - posLastTick).magnitude;
        posLastTick = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (Mathf.Abs(rb.velocity.y) > 0.1f)
            isGrounded = false;
        // get the movement axes for the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // this is our camera
        var camera = Camera.main;

        // these are the forward/right vectors for the camera
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        // project hese vectors onto the horizontal plane
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // the direction the player moves
        var desiredMoveDirection = forward * vertical + right * horizontal;

        // the actual movement
        if (desiredMoveDirection != Vector3.zero) {
            transform.GetChild(0).forward = desiredMoveDirection.normalized;
            transform.Translate(desiredMoveDirection * speed * Time.deltaTime);
            donkoAnims.SetBool("isMoving", true);
            donkoAnims.SetFloat("walkForwards", vertical);
            donkoAnims.SetFloat("walkSide", horizontal);
            donkoAnims.SetFloat("doIdle", 0f);
            // donkoAnims.speed = donkoVeloc / 0.05f;
        } else {
            donkoAnims.SetBool("isMoving", false);
            donkoAnims.SetFloat("doIdle", Mathf.Repeat(donkoAnims.GetFloat("doIdle") + Time.deltaTime, 12f));
            // donkoAnims.speed = 1f;
        }

        // This will keep Donko from toppling over
        // Quaternion rot = rb.rotation;
        // rot[0] = 0;
        // rot[2] = 0;
        // rb.rotation = rot;


        // jump handler
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true) {
            rb.AddForce(new Vector3(0, jump, 0), ForceMode.Impulse);
            isGrounded = false;
            donkoAnims.SetTrigger("didJump");
            donkoAnims.SetFloat("doIdle", 0f);
            // donkoAnims.speed = 1f;
        }

        GroundCheck();
    }

    void GroundCheck() {
        RaycastHit hit;
        float distance = 2f;
        Vector3 dir = new Vector3(0, -0.45f);
        Debug.DrawRay(transform.position, dir);
        if (Physics.Raycast(transform.position, dir, out hit, distance)) {
            isGrounded = true;
            // donkoAnims.SetBool("isJumping", false);
            donkoAnims.SetBool("isFalling", false);
        }
        else {
            isGrounded = false;
            donkoAnims.SetBool("isFalling", true);
            // donkoAnims.speed = 1f;
        }
    }
}