using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    public float interactionRadius = 4f;
    public int flickerType = 0;
    public bool isActivated = false;

    float interactionCooldown = 0f;
    int interactionIndex = 0;

    public Transform[] unactivatedObjects;
    public Transform[] activatedObjects;

    void OnDrawGizmosSelected() {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1f, 1f, 0.4f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < activatedObjects.Length; i++) {
            if (activatedObjects[i].GetComponent<ToggleBase>() != null && !isActivated) {
                activatedObjects[i].GetComponent<ToggleBase>().toggleActivation(1);
                // Debug.Log("deac");
            } else {
                activatedObjects[i].gameObject.SetActive(isActivated);
            }
        }
        for (int i = 0; i < unactivatedObjects.Length; i++) {
            if (unactivatedObjects[i].GetComponent<ToggleBase>() != null && isActivated) {
                unactivatedObjects[i].GetComponent<ToggleBase>().toggleActivation(1);
            }
            else {
                unactivatedObjects[i].gameObject.SetActive(!isActivated);
            }
        }
    }

    void FixedUpdate() {
        if (interactionCooldown > 0f) {
            interactionCooldown = Mathf.Max(0f, interactionCooldown - Time.fixedDeltaTime);
            if (interactionCooldown < interactionIndex * 0.15f + 0.8f && interactionIndex > 0) {
                interactionIndex--;
                if (unactivatedObjects.Length > interactionIndex) {
                    if (unactivatedObjects[interactionIndex].GetComponent<ToggleBase>() != null) {
                        unactivatedObjects[interactionIndex].GetComponent<ToggleBase>().toggleActivation(0);
                    }
                    else {
                        unactivatedObjects[interactionIndex].gameObject.SetActive(!isActivated);
                    }
                }
                if (activatedObjects.Length > interactionIndex) {
                    if (activatedObjects[interactionIndex].GetComponent<ToggleBase>() != null) {
                        activatedObjects[interactionIndex].GetComponent<ToggleBase>().toggleActivation(0);
                    }
                    else {
                        activatedObjects[interactionIndex].gameObject.SetActive(isActivated);
                    }
                }

            }
        }
    }

    public bool toggleActivate() {
        bool success = false;
        if (interactionCooldown <= 0f) {
            success = true;
            isActivated = !isActivated;
            interactionIndex = Mathf.Max((unactivatedObjects.Length), (activatedObjects.Length));
            interactionCooldown = Mathf.Max(0.15f * (unactivatedObjects.Length), 0.15f * (activatedObjects.Length)) + 0.8f;
            /*
            for (int i = 0; i < unactivatedObjects.Length; i++) {
                if (unactivatedObjects[i].GetComponent<ToggleBase>() != null) {
                    unactivatedObjects[i].GetComponent<ToggleBase>().toggleActivation(0);
                }
                else {
                    unactivatedObjects[i].gameObject.SetActive(!isActivated);
                }
            }
            for (int i = 0; i < activatedObjects.Length; i++) {
                if (activatedObjects[i].GetComponent<ToggleBase>() != null) {
                    activatedObjects[i].GetComponent<ToggleBase>().toggleActivation(0);
                }
                else {
                    activatedObjects[i].gameObject.SetActive(isActivated);
                }
            }
            */
        }
        return success;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
