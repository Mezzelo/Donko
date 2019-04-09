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
                InteractionObject interComponent = interactionObjects.GetChild(i).GetComponent<InteractionObject>();
                if ((this.transform.position - interactionObjects.GetChild(i).position).magnitude <
                     interComponent.interactionRadius) {
                    if (this.GetComponent<DonkoInventory>().flickers[interComponent.flickerType] > 0 && !interComponent.isActivated ||
                        interComponent.isActivated) {
                        bool didActivate = interComponent.toggleActivate();
                        if (didActivate) {
                            this.GetComponent<DonkoInventory>().changeFlickerCount(interComponent.flickerType,
                                (interComponent.isActivated ? -1 : 1));
                            break;
                        }
                    }
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
