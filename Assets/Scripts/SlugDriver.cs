using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugDriver : EnemyBase
{
    public Transform player;
    public float aggroRange = 10f;
    public float attackRange = 7f;
    public float attackTime = 0f;
    public float attackCdMax = 1.5f;
    public float moveSpeed = 1f;
    public float aggroMax = 1.5f;

    float attackCd = 0f;

    public float damageRate = 5f;
    public float damageTime = 0.5f;

    float moveAccel = 0f;

    Vector3 originalPos = Vector3.zero;

    Vector3 cTargPos;

    float aggroTime = 0f;
    float damageCd = 1f;

    bool isAggro = false;
    bool isGrounded = true;

    Animator slugAnims;

    public override void takeDamage(int damageTaken) {
        // Debug.Log(health);
        if (damageCd <= 0f) {
            damageCd = 1f;
            attackCd = attackCdMax - 0.01f;
            health -= damageTaken;
            // Debug.Log(health);
            if (health > 0) {
                slugAnims.SetTrigger("wasHurt");
                gameObject.GetComponents<AudioSource>()[1].Play();
            } else if (health == 0) {
                slugAnims.SetTrigger("doDie");
                gameObject.GetComponent<AudioSource>().volume = 0f;
                gameObject.GetComponents<AudioSource>()[4].Play();
            }
        }
    }

    void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1f, 1f, 0.4f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    IEnumerator StopDamage() {
        yield return new WaitForSeconds(damageTime);
        player.GetComponent<LightMeter>().modifyDrainRate(-damageRate);
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
            if (!isAggro) {
                if ((player.position - originalPos).magnitude <= aggroRange) {
                    if (aggroTime == 0f)
                        gameObject.GetComponents<AudioSource>()[1].Play();
                    aggroTime += Time.deltaTime;
                    if (aggroTime > aggroMax) {
                        // Debug.Log("gotAggro");
                        aggroTime = 0f;
                        isAggro = true;
                    }
                }
                else {
                    aggroTime = Mathf.Max(0f, aggroTime - Time.deltaTime);
                }
            } else if (isAggro) {
                if ((player.position - originalPos).magnitude > aggroRange && attackCd <= 0f) {
                    aggroTime += Time.deltaTime;
                    if (aggroTime > aggroMax) {
                        // Debug.Log("lostAggro");
                        aggroTime = 0f;
                        isAggro = false;
                    }
                }
                else {
                    aggroTime = Mathf.Max(0f, aggroTime - Time.deltaTime);
                }
            }
            else {
                if (isAggro) {
                    isAggro = false;
                }
            }
        }

        if (originalPos == Vector3.zero && damageCd > 0f) {
            damageCd = Mathf.Max(damageCd - Time.fixedDeltaTime, 0f);
            if (damageCd <= 0f) {
                originalPos = transform.position;
                // Debug.Log("OriginalPos: " + originalPos);
            }
        }
        Vector3 desiredMoveDirection = (originalPos - transform.position) * 1f / Mathf.Abs((originalPos - transform.position).magnitude);

        if (isAggro) {
            cTargPos = player.position;
            desiredMoveDirection = (cTargPos - transform.position) * 1f / Mathf.Abs((cTargPos - transform.position).magnitude);
            Debug.DrawRay(transform.position, desiredMoveDirection);
        } else {
            // Debug.Log(desiredMoveDirection);
        }

        // Debug.Log(desiredMoveDirection.magnitude);

        if (attackCd > 0f) {
            if (attackCd >= attackCdMax && attackCd - Time.fixedDeltaTime < attackCdMax) {
                if ((player.position - transform.position).magnitude < attackRange * 1.2f) {
                    player.GetComponent<LightMeter>().modifyDrainRate(damageRate);
                    player.GetComponent<Rigidbody>().AddForce((transform.position - player.position).normalized * -300f, ForceMode.Force);
                    player.GetComponent<DonkoController>().screenShake(1f);
                    gameObject.GetComponents<AudioSource>()[3].Play();
                    StartCoroutine(StopDamage());
                }
            }
            attackCd = Mathf.Max(0f, attackCd - Time.fixedDeltaTime);
        }

        if (damageCd > 0f)
            damageCd = Mathf.Max(0f, damageCd - Time.fixedDeltaTime);

        if (!isAggro && health > 0) {
            // look towards the player
            if (aggroTime > 0f) {
                transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation,
                    Quaternion.LookRotation(
                        new Vector3(player.position.x - transform.position.x, (player.position.y - transform.position.y) / 7f, player.position.z - transform.position.z)),
                        0.06f
                );
                slugAnims.SetBool("isMoving", false);
            } else {
                if (desiredMoveDirection != null && desiredMoveDirection != Vector3.zero && (originalPos - transform.position).magnitude > 0.2f &&
                    originalPos != Vector3.zero) {
                    transform.Translate(desiredMoveDirection * moveSpeed * Time.fixedDeltaTime);
                    slugAnims.SetBool("isMoving", true);
                    slugAnims.SetFloat("walkForwards", desiredMoveDirection.magnitude / moveSpeed);
                    gameObject.GetComponent<AudioSource>().volume = desiredMoveDirection.magnitude / moveSpeed * 1.35f;
                    transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation,
                        Quaternion.LookRotation(
                            new Vector3(originalPos.x - transform.position.x, (originalPos.y - transform.position.y) / 7f,
                            originalPos.z - transform.position.z)),
                            0.06f
                    );
                }
                else {
                    slugAnims.SetBool("isMoving", false);
                    gameObject.GetComponent<AudioSource>().volume = 0f;
                }
            }
        } else if (isAggro && health > 0) {
            transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation,
                    Quaternion.LookRotation(
                        new Vector3(player.position.x - transform.position.x, (player.position.y - transform.position.y) / 7f, player.position.z - transform.position.z)),
                        0.06f
                );
            if ((player.position - transform.position).magnitude < attackRange && attackCd <= 0f) {
                // attack the player
                attackCd = attackCdMax + attackTime;
                slugAnims.SetTrigger("doAttack");
                slugAnims.SetBool("isMoving", false);
                gameObject.GetComponents<AudioSource>()[2].Play();
            } else {
                slugAnims.SetBool("isMoving", true);
                slugAnims.SetFloat("walkForwards", desiredMoveDirection.magnitude / moveSpeed);
                if (aggroTime <= 0f)
                    moveAccel = Mathf.Min(1f, moveAccel + Time.fixedDeltaTime);
                else
                    moveAccel = (aggroMax - aggroTime) / aggroMax;
            }
            isGrounded = GroundCheck();
            float attackMoveMult =
                (attackCd <= 0f ? 1f :
                (attackCd > attackCdMax + attackTime - 0.6f ? (attackCd - attackCdMax - attackTime + 0.6f) / 0.6f * 0.95f + 0.05f :
                (attackCd < 0.5f ? (0.5f - attackCd) / 0.5f * 0.95f + 0.05f : 0.05f)));
            // Debug.Log(attackMoveMult);
            transform.Translate(desiredMoveDirection * moveSpeed * Time.fixedDeltaTime * ((1f - (Vector3.Angle(transform.GetChild(0).forward, desiredMoveDirection.normalized) / 180f) *
               0.5f)) * attackMoveMult * moveAccel * ((Mathf.Sin(Time.time * 9f) + 1f)/4f + 0.5f) * (isGrounded ? 1f : 0f));
            gameObject.GetComponent<AudioSource>().volume = desiredMoveDirection.magnitude / moveSpeed * 1.35f * attackMoveMult;
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
