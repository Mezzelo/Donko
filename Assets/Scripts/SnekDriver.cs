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
    void Update() {
        if (attackCd > 0f) {
            if (attackCd >= attackCdMax && attackCd - Time.deltaTime < attackCdMax) {
                if ((player.position - transform.position).magnitude < attackRange) {
                    player.GetComponent<LightMeter>().modifyDrainRate(damageRate);
                    player.GetComponent<Rigidbody>().AddForce((transform.position - player.position).normalized * -350f, ForceMode.Force);
                    gameObject.GetComponents<AudioSource>()[2].Play();
                    StartCoroutine(StopDamage());
                }
            }
            attackCd = Mathf.Max(0f, attackCd - Time.deltaTime);
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
            transform.LookAt(transform.position + new Vector3(transform.position.x - player.position.x, 0f, transform.position.z - player.position.z));
            if (attackCd <= 0f && (player.position - transform.position).magnitude < attackRange &&
                player.GetComponent<DonkoController>().enabled == true) {
                gameObject.GetComponent<Animator>().SetTrigger("doAttack");
                gameObject.GetComponents<AudioSource>()[1].Play();
                attackCd = attackTime + attackCdMax;
            }
        }
    }
}
