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
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < activatedObjects.Length; i++)
            activatedObjects[i].gameObject.SetActive(isActivated);
        for (int i = 0; i < unactivatedObjects.Length; i++)
            unactivatedObjects[i].gameObject.SetActive(!isActivated);
    }

    public void toggleActivate() {
        isActivated = !isActivated;
        for (int i = 0; i < unactivatedObjects.Length; i++)
            unactivatedObjects[i].gameObject.SetActive(!isActivated);
        for (int i = 0; i < activatedObjects.Length; i++)
            activatedObjects[i].gameObject.SetActive(isActivated);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
