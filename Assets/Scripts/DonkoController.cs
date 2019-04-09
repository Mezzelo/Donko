using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonkoController : MonoBehaviour {

    public float speed;
    public float jump;
    private Rigidbody rb;
    public bool isGrounded = true;

    public float airspeedMult = 0.55f;
    float airspeedCurrent = 1f;

    public Animator donkoAnims;

    public float jumpCooldownMax = 0.4f;

    float jumpCooldownC = 0f;

    public void doDeath() {
        transform.GetChild(1).GetComponent<MeshCollider>().enabled = true;
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
        if (!isGrounded) {
            if (airspeedCurrent > airspeedMult)
                airspeedCurrent = Mathf.Max(airspeedCurrent - Time.deltaTime * 1.5f, airspeedMult);
        } else {
            if (airspeedCurrent < 1f)
                airspeedCurrent = Mathf.Min(airspeedCurrent + Time.deltaTime * 1.5f, 1f);
        }

        var desiredMoveDirection = (forward * vertical + right * horizontal) * airspeedCurrent;

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
        GroundCheck();
        if (jumpCooldownC > 0f)
            jumpCooldownC = Mathf.Max(0f, jumpCooldownC - Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true && jumpCooldownC <= 0f) {
            rb.AddForce(new Vector3(0, jump, 0), ForceMode.Impulse);
            isGrounded = false;
            donkoAnims.SetTrigger("didJump");
            donkoAnims.SetFloat("doIdle", 0f);
            jumpCooldownC = jumpCooldownMax;
            // donkoAnims.speed = 1f;
        }
    }

    void GroundCheck() {
        RaycastHit hit;
        float distance = 1.35f;
        Vector3 dir = new Vector3(0, -distance);
        Debug.DrawRay(transform.position + transform.GetChild(0).forward * -0.35f, dir);
        if (Physics.Raycast(transform.position + transform.GetChild(0).forward * -0.35f, dir, out hit, distance)) {
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