using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DonkoInteraction : MonoBehaviour
{

    public Transform interactionObjects;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            for (int i = 0; i < interactionObjects.childCount; i++) {
                if ((this.transform.position - interactionObjects.GetChild(i).position).magnitude <
                     interactionObjects.GetChild(i).GetComponent<InteractionObject>().interactionRadius) {
                    interactionObjects.GetChild(i).GetComponent<InteractionObject>().toggleActivate();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
