using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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

    bool isSwinging = false;
    Transform currentVine;

    public void doDeath() {
        if (isSwinging) {
            transform.GetChild(0).localPosition = new Vector3(0f, transform.GetChild(0).localPosition.y, 0f);
            Destroy(gameObject.GetComponent<FixedJoint>());
            isSwinging = false;
        }
        transform.GetChild(1).GetComponent<MeshCollider>().enabled = true;
        donkoAnims.SetTrigger("didDie");
        gameObject.GetComponent<AudioSource>().Stop();
    }

    Vector3 posLastTick;
    float donkoVeloc;


    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        posLastTick = transform.position;
        gameObject.GetComponent<AudioSource>().Play();
    }

    private void FixedUpdate() {
        donkoVeloc = (transform.position - posLastTick).magnitude;
        posLastTick = transform.position;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Vine" && !isSwinging) {
            if (collision.transform.parent.GetComponent<VineScript>().canSwing()) {
                // Debug.Log("attach 2 vine!");
                isSwinging = true;
                currentVine = collision.transform.parent;
                donkoAnims.SetBool("isSwinging", true);
                rb.constraints = RigidbodyConstraints.None;
                transform.rotation = Quaternion.LookRotation((collision.transform.position - transform.position).normalized, Vector3.up);
                transform.GetChild(0).localPosition = new Vector3(0f, transform.GetChild(0).localPosition.y, 0.3f);
                transform.GetChild(0).localRotation = Quaternion.identity;
                gameObject.AddComponent<FixedJoint>();
                gameObject.GetComponent<FixedJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
                gameObject.GetComponent<AudioSource>().Stop();
                // gameObject.GetComponent<FixedJoint>().massScale = 1f;
                // gameObject.AddComponent<FixedJoint>().connectedMassScale = 1f;
            }
        }
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
                airspeedCurrent = Mathf.Max(airspeedCurrent - Time.deltaTime * 0.5f, airspeedMult);
        } else {
            if (airspeedCurrent < 1f)
                airspeedCurrent = Mathf.Min(airspeedCurrent + Time.deltaTime * 0.5f, 1f);
        }

        if (isSwinging) {
            var desiredMoveDirection = (forward * vertical + right * horizontal) * airspeedCurrent;
            if (desiredMoveDirection != Vector3.zero && Time.timeScale > 0f) {
                rb.AddForce(desiredMoveDirection * 200f * Time.deltaTime, ForceMode.Force);
            }
        } else {


            var desiredMoveDirection = (forward * vertical + right * horizontal) * airspeedCurrent;

            // the actual movement
            if (desiredMoveDirection != Vector3.zero && Time.timeScale > 0f) {
                transform.GetChild(0).forward = desiredMoveDirection.normalized;
                transform.Translate(desiredMoveDirection * speed * Time.deltaTime);
                donkoAnims.SetBool("isMoving", true);
                donkoAnims.SetFloat("walkForwards", vertical);
                donkoAnims.SetFloat("walkSide", horizontal);
                donkoAnims.SetFloat("doIdle", 0f);
                if (isGrounded)
                    gameObject.GetComponent<AudioSource>().volume = Mathf.Max(-0.8f + airspeedCurrent, 0f);
                else
                    gameObject.GetComponent<AudioSource>().volume = Mathf.Max(-0.8f + airspeedCurrent, 0f);
                // donkoAnims.speed = donkoVeloc / 0.05f;
            }
            else {
                donkoAnims.SetBool("isMoving", false);
                donkoAnims.SetFloat("doIdle", Mathf.Repeat(donkoAnims.GetFloat("doIdle") + Time.deltaTime, 12f));
                gameObject.GetComponent<AudioSource>().volume = 0f;
                // donkoAnims.speed = 1f;
            }

            // This will keep Donko from toppling over
            // Quaternion rot = rb.rotation;
            // rot[0] = 0;
            // rot[2] = 0;
            // rb.rotation = rot;
        }


        // jump handler
        GroundCheck();
        if (jumpCooldownC > 0f)
            jumpCooldownC = Mathf.Max(0f, jumpCooldownC - Time.deltaTime);
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 1")) && isGrounded && jumpCooldownC <= 0f
            && !isSwinging) {
            rb.AddForce(new Vector3(0, jump, 0), ForceMode.Impulse);
            isGrounded = false;
            donkoAnims.SetTrigger("didJump");
            donkoAnims.SetFloat("doIdle", 0f);
            jumpCooldownC = jumpCooldownMax;
            // donkoAnims.speed = 1f;
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 1")) && isSwinging) {
            Destroy(gameObject.GetComponent<FixedJoint>());
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            isSwinging = false;
            currentVine.gameObject.GetComponent<VineScript>().disableVine();
            donkoAnims.SetBool("isSwinging", false);
            transform.rotation = Quaternion.identity;
            transform.GetChild(0).localPosition = new Vector3(0f, transform.GetChild(0).localPosition.y, 0f);
            rb.AddForce(new Vector3(0, jump * 0.5f, 0), ForceMode.Impulse);
        }
    }

    void GroundCheck() {
        RaycastHit hit;
        float distance = 1.35f;
        Vector3 dir = new Vector3(0, -distance);
        Debug.DrawRay(transform.position + transform.GetChild(0).forward * -0.35f, dir);
        if (Physics.Raycast(transform.position + transform.GetChild(0).forward * -0.35f, dir, out hit, distance)
            && !isSwinging) {
            if (!isGrounded && airspeedCurrent < airspeedMult + 0.2f && donkoAnims.GetBool("isFalling")) {
                // min: 0 /
                gameObject.GetComponents<AudioSource>()[1].Play();
            }
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