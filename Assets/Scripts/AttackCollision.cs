using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{

    bool canAttack = false;
    List<GameObject> collided;

    public DonkoController playerController;

    public void setCanAttack(bool newAttack) {
        if (!canAttack && newAttack) {
            for (int i = 0; i < collided.Count; i++) {
                collided[i].GetComponent<EnemyBase>().takeDamage(1);
                collided[i].GetComponent<Rigidbody>().AddForce((transform.position - collided[i].transform.position).normalized * -600f, ForceMode.Force);
                playerController.screenShake(0.75f);
            }
            if (collided.Count > 0) {
                gameObject.GetComponents<AudioSource>()[1].pitch = Random.Range(0.95f, 1.05f);
                gameObject.GetComponents<AudioSource>()[1].Play();
            }
        }
        canAttack = newAttack;
    }


    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
            collided.Add(other.gameObject);
            if (canAttack) {
                other.gameObject.GetComponent<EnemyBase>().takeDamage(1);
                other.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position).normalized * -600f, ForceMode.Force);
                playerController.screenShake(0.75f);
                gameObject.GetComponents<AudioSource>()[1].pitch = Random.Range(0.95f, 1.05f);
                gameObject.GetComponents<AudioSource>()[1].Play();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Enemy")) {
            if (collided.Contains(other.gameObject)) {
                collided.Remove(other.gameObject);
            }
        }
    }

    void Start() {
        collided = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
