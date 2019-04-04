using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonkoController : MonoBehaviour {

    public float speed;
    public float jump;
    private Rigidbody rb;
    public bool isGrounded = true;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
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
        if (desiredMoveDirection != Vector3.zero)
            transform.GetChild(0).forward = desiredMoveDirection.normalized;
        transform.Translate(desiredMoveDirection * speed * Time.deltaTime);

        // This will keep Donko from toppling over
        // Quaternion rot = rb.rotation;
        // rot[0] = 0;
        // rot[2] = 0;
        // rb.rotation = rot;


        // jump handler
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true) {
            rb.AddForce(new Vector3(0, jump, 0), ForceMode.Impulse);
        }

        GroundCheck();
    }

    void GroundCheck() {
        RaycastHit hit;
        float distance = 2f;
        Vector3 dir = new Vector3(0, -1);
        Debug.DrawRay(transform.position, dir);
        if (Physics.Raycast(transform.position, dir, out hit, distance)) {
            isGrounded = true;
        }
        else {
            isGrounded = false;
        }
    }
}