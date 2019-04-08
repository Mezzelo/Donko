using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    public float interactionRadius = 4f;
    public int flickerType = 0;
    public bool isActivated = false;

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
                Debug.Log("deac");
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

    public void toggleActivate() {
        isActivated = !isActivated;
        for (int i = 0; i < unactivatedObjects.Length; i++) {
            if (unactivatedObjects[i].GetComponent<ToggleBase>() != null) {
                unactivatedObjects[i].GetComponent<ToggleBase>().toggleActivation(0);
            }
            else {
                unactivatedObjects[i].gameObject.SetActive(isActivated);
            }
        }
        for (int i = 0; i < activatedObjects.Length; i++) {
            if (activatedObjects[i].GetComponent<ToggleBase>() != null) {
                activatedObjects[i].GetComponent<ToggleBase>().toggleActivation(0);
            }
            else {
                activatedObjects[i].gameObject.SetActive(!isActivated);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
