using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineScript : ToggleBase
{
    float cooldown = 0f;

    public bool canSwing() {
        return isActivated;
    }

    public void disableVine() {
        for (int i = 0; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<CapsuleCollider>() != null)
                transform.GetChild(i).GetComponent<CapsuleCollider>().enabled = false;
        }
        cooldown = 1f;
    }

    public override void toggleActivation(int mode) {
        base.toggleActivation(mode);
        if (isActivated) {
            // for (int i = 0; i < transform.childCount; i++) {
            //     transform.GetChild(i).gameObject.SetActive(true);
            // }
        }
        else {
            // for (int i = 0; i < transform.childCount; i++) {
            //     transform.GetChild(i).gameObject.SetActive(false);
            // }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!isActivated) {
            for (int i = 1; i < transform.childCount; i++) {
                if (transform.GetChild(i).GetComponent<Rigidbody>() != null) {
                    transform.GetChild(i).GetComponent<Rigidbody>().AddForce(
                        (transform.GetChild(0).position - transform.GetChild(i).position) * 10f,
                        ForceMode.Force);
                }
            }
        }
        if (cooldown > 0f) {
            cooldown = Mathf.Max(0f, cooldown - Time.fixedDeltaTime);
            if (cooldown <= 0f) {
                for (int i = 0; i < transform.childCount; i++) {
                    if (transform.GetChild(i).GetComponent<CapsuleCollider>() != null)
                        transform.GetChild(i).GetComponent<CapsuleCollider>().enabled = true;
                }
            }
        }
    }
}
