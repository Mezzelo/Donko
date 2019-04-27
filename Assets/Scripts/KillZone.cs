using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public float drainRate = 1f;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<LightMeter>().modifyDrainRate(drainRate);
        } else if (other.gameObject.CompareTag("Enemy")) {
            other.gameObject.GetComponent<EnemyBase>().takeDamage(99);
        } 
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<LightMeter>().modifyDrainRate(-drainRate);
        }
    }
}
