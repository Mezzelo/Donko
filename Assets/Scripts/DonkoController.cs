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
    public Transform capCollision;

    public float jumpCooldownMax = 0.4f;

    float jumpCooldownC = 0f;

    bool isSwinging = false;
    Transform currentVine;
    
    float attackTimeC = 0f;
    float attackTimeMax = 1.266f;

    Transform gameCam;

    public void doDeath() {
        if (isSwinging) {
            transform.GetChild(0).localPosition = new Vector3(0f, transform.GetChild(0).localPosition.y, 0f);
            Destroy(gameObject.GetComponent<FixedJoint>());
            isSwinging = false;
        }
        capCollision.GetComponent<MeshCollider>().enabled = true;
        capCollision.GetChild(0).GetComponent<MeshCollider>().enabled = false;
        donkoAnims.SetTrigger("didDie");
        gameObject.GetComponent<AudioSource>().Stop();
    }

    Vector3 posLastTick;
    Vector3 donkoVeloc;

    public void screenShake(float amount) {
        gameCam.GetComponent<CameraController>().addShake(amount);
    }


    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
        posLastTick = transform.position;
        gameObject.GetComponent<AudioSource>().Play();
        gameCam = Camera.main.transform;
    }

    private void FixedUpdate() {
        donkoVeloc = (transform.position - posLastTick)/Time.fixedDeltaTime;
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
                transform.position = collision.transform.position + (transform.position - collision.transform.position).normalized * 0.75f;
                transform.rotation = Quaternion.LookRotation((collision.transform.position - transform.position).normalized, Vector3.up);
                transform.GetChild(0).localPosition = new Vector3(0f, transform.GetChild(0).localPosition.y, 0.3f);
                transform.GetChild(0).localRotation = Quaternion.identity;
                gameObject.AddComponent<FixedJoint>();
                gameObject.GetComponent<FixedJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
                gameObject.GetComponent<AudioSource>().Stop();
                collision.gameObject.GetComponent<Rigidbody>().AddForce(donkoVeloc * 75f, ForceMode.Force);
                collision.transform.parent.GetComponent<VineScript>().toggleCollisions(false);
                // gameObject.GetComponent<FixedJoint>().massScale = 1f;
                // gameObject.AddComponent<FixedJoint>().connectedMassScale = 1f;
            }
        }
    }
    // Update is called once per frame
    void Update() {
        // if (Mathf.Abs(rb.velocity.y) > 0.1f)
        //     isGrounded = false;
        // get the movement axes for the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        GroundCheck();
        // Debug.Log(isGrounded + ", " + airspeedCurrent);

        if (attackTimeC > 0f) {
            if (attackTimeC > attackTimeMax - 0.2f && attackTimeC - Time.deltaTime < attackTimeMax - 0.2f) {
                capCollision.GetChild(0).GetComponents<AudioSource>()[0].pitch = Random.Range(0.9f, 1.1f);
                capCollision.GetChild(0).GetComponents<AudioSource>()[0].Play();
            }
            attackTimeC = Mathf.Max(0f, attackTimeC - Time.deltaTime);
            // Debug.Log(attackTimeC + ", " + (attackTimeC > 0.5f && attackTimeC < attackTimeMax - 0.5f));
            capCollision.GetChild(0).GetComponent<AttackCollision>().setCanAttack(
                (attackTimeC > 0.5f && attackTimeC < attackTimeMax - 0.4f));
        }
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 1")) && attackTimeC <= 0f && !isSwinging
            && airspeedCurrent > airspeedMult + (1f - airspeedMult) / 3f * 2f
            ) {
            donkoAnims.SetTrigger("didAttack");
            donkoAnims.SetFloat("doIdle", 0f);
            attackTimeC = attackTimeMax;
        }

        // this is our camera

        // these are the forward/right vectors for the camera
        var forward = gameCam.forward;
        var right = gameCam.transform.right;

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
                airspeedCurrent = Mathf.Min(airspeedCurrent + Time.deltaTime * 1f, 1f);
        }

        if (isSwinging && attackTimeC <= 0f) {
            var desiredMoveDirection = (forward * vertical + right * horizontal) * airspeedCurrent;
            if (desiredMoveDirection != Vector3.zero && Time.timeScale > 0f) {
                rb.AddForce(desiredMoveDirection * 650f * Time.deltaTime, ForceMode.Force);
            }
        } else {
            float attackMoveMult =
                (attackTimeC <= 0f ? 1f :
                (attackTimeC > attackTimeMax - 0.6f ? (attackTimeC - attackTimeMax + 0.6f) / 0.6f * 0.75f + 0.25f :
                (attackTimeC < 0.5f ? (0.5f - attackTimeC) / 0.5f * 0.75f + 0.25f : 0.25f)));
            var desiredMoveDirection = (forward * vertical + right * horizontal) * airspeedCurrent * attackMoveMult;

            // the actual movement
            if (desiredMoveDirection != Vector3.zero && Time.timeScale > 0f) {
                if (attackMoveMult > 0.25f) {
                    transform.GetChild(0).forward = Vector3.Lerp(transform.GetChild(0).forward, 
                        desiredMoveDirection.normalized, 0.25f);
                }
                transform.Translate(desiredMoveDirection * speed * Time.deltaTime *
                    ((1f - (Vector3.Angle(transform.GetChild(0).forward, desiredMoveDirection.normalized)/180f) *
                    0.5f) * 0.85f + 0.15f));
                donkoAnims.SetBool("isMoving", true);
                donkoAnims.SetFloat("walkForwards",
                    ((1f - (Vector3.Angle(transform.GetChild(0).forward, desiredMoveDirection.normalized) / 180f) *
                    0.5f) * 0.85f + 0.15f) * airspeedCurrent);
                donkoAnims.SetFloat("doIdle", 0f);
                if (isGrounded)
                    gameObject.GetComponent<AudioSource>().volume = Mathf.Max(-0.8f + airspeedCurrent, 0f) * attackMoveMult;
                else
                    gameObject.GetComponent<AudioSource>().volume = Mathf.Max(-0.8f + airspeedCurrent, 0f) * attackMoveMult;
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
        if (jumpCooldownC > 0f) {
            donkoAnims.SetBool("jumpFrame", jumpCooldownC > jumpCooldownMax - 0.05f);
            jumpCooldownC = Mathf.Max(0f, jumpCooldownC - Time.deltaTime);
        }
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0")) && isGrounded && jumpCooldownC <= 0f
            && !isSwinging && attackTimeC <= 0f) {
            rb.AddForce(new Vector3(0, jump, 0), ForceMode.Impulse);
            isGrounded = false;
            gameObject.GetComponents<AudioSource>()[2].Play();
            donkoAnims.SetTrigger("didJump");
            donkoAnims.SetBool("jumpFrame", true);
            donkoAnims.SetFloat("doIdle", 0f);
            jumpCooldownC = jumpCooldownMax;
            // donkoAnims.speed = 1f;
        } else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0")) && isSwinging) {
            Destroy(gameObject.GetComponent<FixedJoint>());
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            isSwinging = false;
            currentVine.gameObject.GetComponent<VineScript>().disableVine();
            donkoAnims.SetBool("isSwinging", false);
            transform.rotation = Quaternion.identity;
            transform.GetChild(0).localPosition = new Vector3(0f, transform.GetChild(0).localPosition.y, 0f);
            rb.AddForce(new Vector3(0, jump * 0.5f, 0), ForceMode.Impulse);
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    void GroundCheck() {
        RaycastHit hit;
        float distance = 1.85f;
        Vector3 dir = new Vector3(0, -distance);
        Debug.DrawRay(transform.position + transform.GetChild(0).forward * -0.2f, dir);
        if (Physics.Raycast(transform.position + transform.GetChild(0).forward * -0.2f, dir, out hit, distance)
            && !isSwinging) {
            if (!isGrounded && airspeedCurrent < airspeedMult + 0.2f && donkoAnims.GetBool("isFalling")) {
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