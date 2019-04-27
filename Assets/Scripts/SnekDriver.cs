using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnekDriver : MonoBehaviour
{
    public Transform player;
    public float aggroRange = 10f;
    public float attackRange = 5f;
    public float attackTime;
    public float attackCdMax;
    float attackCd = 0f;

    public float damageRate = 5f;
    public float damageTime = 0.5f;

    bool isAggro = false;

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
        if (player == null)
            player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (attackCd > 0f) {
            if (attackCd >= attackCdMax && attackCd - Time.fixedDeltaTime < attackCdMax) {
                if ((player.position - transform.position).magnitude < attackRange ||
                    (player.position.y > transform.position.y + 1.3f &&
                    (player.position - transform.position).magnitude < attackRange * 2f)) {
                    player.GetComponent<LightMeter>().modifyDrainRate(damageRate);
                    player.GetComponent<DonkoController>().screenShake(1f);
                    player.GetComponent<Rigidbody>().AddForce((transform.position - player.position).normalized * -350f, ForceMode.Force);
                    gameObject.GetComponents<AudioSource>()[2].Play();
                    StartCoroutine(StopDamage());
                }
            }
            attackCd = Mathf.Max(0f, attackCd - Time.fixedDeltaTime);
        }

        if ((player.position - transform.position).magnitude <= aggroRange && !isAggro) {
            isAggro = true;
            gameObject.GetComponent<Animator>().SetBool("isAggro", true);
            gameObject.GetComponents<AudioSource>()[0].Play();
        } else if ((player.position - transform.position).magnitude > aggroRange * 1.35f && isAggro) {
            isAggro = false;
            gameObject.GetComponent<Animator>().SetBool("isAggro", false);
        }
        if (isAggro) {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(
                    new Vector3(transform.position.x - player.position.x, (transform.position.y - player.position.y)/7f, transform.position.z - player.position.z)),
                    0.15f
                );
            if (attackCd <= 0f && (player.position - transform.position).magnitude < attackRange &&
                player.GetComponent<DonkoController>().enabled == true) {
                gameObject.GetComponent<Animator>().SetTrigger("doAttack");
                gameObject.GetComponents<AudioSource>()[1].Play();
                attackCd = attackTime + attackCdMax;
            } else if (attackCd <= 0f && (player.position - transform.position).magnitude < attackRange * 1.3f &&
                player.position.y > transform.position.y + 1.7f &&
                player.GetComponent<DonkoController>().enabled == true) {
                // Debug.Log("attackHigh");
                gameObject.GetComponent<Animator>().SetTrigger("doAttackHigh");
                gameObject.GetComponents<AudioSource>()[1].Play();
                attackCd = attackTime + attackCdMax;
            }
        }
    }
}
