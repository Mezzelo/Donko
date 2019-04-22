using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnekDriver : MonoBehaviour
{
    public Transform player;
    float attackCd = 0f;

    // Start is called before the first frame update
    void Start() {
        if (player == null)
            player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update() {
        if (attackCd > 0f)
            attackCd = Mathf.Max(0f, attackCd - Time.time);
        if (attackCd <= 0f && (player.position - transform.position).magnitude < 4f) {
            gameObject.GetComponent<Animator>().SetTrigger("doAttack");
            attackCd = 1.3f;
        }
    }
}
