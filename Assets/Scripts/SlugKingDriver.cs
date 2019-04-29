using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugKingDriver : EnemyBase {
    public Transform player;
    public float moveSpeed = 1f;
    public Transform[] wave1Snails;
    public Transform[] wave2Snails;
    public Transform[] wave3Snails;
    public Transform wave1halfSnail;
    public Transform wave2halfSnail;
    public Transform rewardFlicker;

    float damageCd = 1f;

    // float attackCd = 0f;

    // float moveAccel = 0f;

    Vector3 originalPos = Vector3.zero;
    Vector3 pos2;
    Vector3 pos3;

    Vector3 cTargPos = Vector3.zero;

    // float aggroTime = 0f;

    // bool isAggro = false;
    // bool isGrounded = true;

    Animator slugAnims;

    public override void takeDamage(int damageTaken) {
        // Debug.Log(health);
        if (damageCd <= 0f) {
            damageCd = 1f;
            health -= damageTaken;
            if (health == 5) {
                cTargPos = pos2;
                for (int i = 0; i < wave1Snails.Length; i++)
                    wave1Snails[i].GetComponent<SlugDriver>().canAggro = true;
            }
            else if (health == 4)
                wave1halfSnail.GetComponent<SlugDriver>().canAggro = true;
            else if (health == 3) {
                cTargPos = pos3;
                for (int i = 0; i < wave2Snails.Length; i++)
                    wave2Snails[i].GetComponent<SlugDriver>().canAggro = true;
            }
            else if (health == 2)
                wave2halfSnail.GetComponent<SlugDriver>().canAggro = true;
            else if (health == 1) {
                cTargPos = originalPos;
                for (int i = 0; i < wave3Snails.Length; i++)
                    wave3Snails[i].GetComponent<SlugDriver>().canAggro = true;
            }
            // Debug.Log(health);
            if (health > 0) {
                slugAnims.SetTrigger("wasHurt");
                gameObject.GetComponents<AudioSource>()[1].Play();
            }
            else if (health == 0) {
                rewardFlicker.GetComponent<InteractionObject>().toggleActivate();
                slugAnims.SetTrigger("doDie");
                gameObject.GetComponent<AudioSource>().volume = 0f;
                gameObject.GetComponents<AudioSource>()[3].Play();
                GameObject.Find("Canvas").GetComponent<GameDriver>().endLevel();
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        slugAnims = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        if (player == null)
            player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (player.GetComponent<DonkoController>().enabled == true && health > 0) {
            // 1.5 second delay between gaining and losing aggro.
        }

        if (originalPos == Vector3.zero && damageCd > 0f) {
            damageCd = Mathf.Max(damageCd - Time.fixedDeltaTime, 0f);
            if (damageCd <= 0f) {
                originalPos = transform.position;
                cTargPos = originalPos;
                pos2 = new Vector3(transform.parent.GetChild(0).position.x, originalPos.y, transform.parent.GetChild(0).position.z);
                pos3 = new Vector3(transform.parent.GetChild(1).position.x, originalPos.y, transform.parent.GetChild(1).position.z);
                // Debug.Log("OriginalPos: " + originalPos);
            }
        }
        Vector3 desiredMoveDirection = (cTargPos - transform.position) * 1f / Mathf.Abs((cTargPos - transform.position).magnitude);
        

        // Debug.Log(desiredMoveDirection.magnitude);

        if (damageCd > 0f) { }
            damageCd = Mathf.Max(0f, damageCd - Time.fixedDeltaTime);

        if (health > 0) {
            if (desiredMoveDirection != null && desiredMoveDirection != Vector3.zero && (cTargPos - transform.position).magnitude > 0.6f &&
                        cTargPos != Vector3.zero) {
                transform.Translate(desiredMoveDirection * moveSpeed * Time.fixedDeltaTime * (1f - damageCd) / 1f * (GroundCheck() ? 1f : 0f));
                slugAnims.SetBool("isMoving", true);
                slugAnims.SetFloat("walkForwards", desiredMoveDirection.magnitude / moveSpeed);
                gameObject.GetComponent<AudioSource>().volume = desiredMoveDirection.magnitude / moveSpeed * 1.35f;
                transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation,
                    Quaternion.LookRotation(
                        new Vector3(cTargPos.x - transform.position.x, (cTargPos.y - transform.position.y) / 7f,
                        cTargPos.z - transform.position.z)),
                        0.06f
                );
            }
            else {
                slugAnims.SetBool("isMoving", false);
                gameObject.GetComponent<AudioSource>().volume = 0f;
                transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation,
                    Quaternion.LookRotation(
                        new Vector3(player.position.x - transform.position.x, (player.position.y - transform.position.y) / 7f,
                        player.position.z - transform.position.z)),
                        0.06f
                );
            }
        }
    }

    bool GroundCheck() {
        RaycastHit hit;
        float distance = 1.85f;
        Vector3 dir = new Vector3(0, -distance, 0f);
        Debug.DrawRay(transform.position + new Vector3(0f, 0.3f, 0f), dir);
        if (Physics.Raycast(transform.position + new Vector3(0f, 0.3f, 0f), dir, out hit, distance)) {
            return true;
        }
        else {
            return false;
        }
    }
}
