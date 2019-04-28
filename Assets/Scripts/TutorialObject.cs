using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialObject : MonoBehaviour 
{

    public Sprite controllerGraphic;
    public Transform player;
    public Transform thisCamera;
    public Transform disablePosition;
    public bool disableOnEnter = true;
    public float disableDistance;
    public bool startHidden;
    public GameObject[] nextTutorial;

    Vector3 originalPosition;

    float fadeTween = 1f;

    void OnDrawGizmosSelected() {
        if (disablePosition != null) {
            Gizmos.color = new Color(1f, 1f, 0.4f, 0.3f);
            Gizmos.DrawWireSphere(disablePosition.position, disableDistance);
        }
    }

    public void disableTutorial() {
        fadeTween = 0.99f;
        if (nextTutorial.Length > 0) {
            for (int i = 0; i < nextTutorial.Length; i++) {
                nextTutorial[i].GetComponent<TutorialObject>().showTutorial();
            }
        }
    }

    public void showTutorial() {
        fadeTween = 2f;
    }


    // Start is called before the first frame update
    void Start() {
        if (!GlobalVars.uiEnabled) {
            GameObject.Destroy(this.gameObject);
        } else {
            originalPosition = transform.position;
            if (startHidden) {
                gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
                fadeTween = -1f;
            }
            if (player == null) {
                player = GameObject.Find("Player").transform;
            }
            if (thisCamera == null) {
                thisCamera = GameObject.Find("Main Camera").transform;
            }
            if (Input.GetJoystickNames().Length > 0) {
                gameObject.GetComponent<SpriteRenderer>().sprite = controllerGraphic;
            }
            if (disablePosition != null)
                disablePosition.transform.parent = transform.parent;
        }
    }

    // Update is called once per frame
    void Update() {
        transform.position = originalPosition + new Vector3(0f, Mathf.Sin(Time.time * 2.3f) * 0.2f, 0f);
        transform.LookAt(transform.position + (transform.position - thisCamera.position));
        if (fadeTween > 1f) {
            fadeTween = Mathf.Max(1f, fadeTween - Time.deltaTime);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 2f - fadeTween);
        }
        if (disablePosition != null && fadeTween == 1f) {
            if (((player.position - disablePosition.position).magnitude <= disableDistance) == disableOnEnter) {
                if (nextTutorial.Length > 0) {
                    for (int i = 0; i < nextTutorial.Length; i++) {
                        nextTutorial[i].GetComponent<TutorialObject>().showTutorial();
                    }
                }
                fadeTween = 0.99f;
            }
        } else if (fadeTween < 1f && fadeTween > 0f) {
            fadeTween = Mathf.Max(0f, fadeTween - Time.deltaTime);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, fadeTween);
            if (fadeTween <= 0f) {
                if (disablePosition != null)
                    disablePosition.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }
}
