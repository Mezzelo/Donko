﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDriver : MonoBehaviour
{

    public Transform player;

    float tween = 1.3f;

    bool isTransition = false;
    bool isPaused = false;

    int levelStatus = 0;
    // 0 = normal
    // 1 = level complete
    // 2 = die

    public Transform levelEndPoint;
    public float levelEndRadius = 20f;
    public string nextScene;

    float levelTime;

    void OnDrawGizmosSelected() {
        if (levelEndPoint != null) {
            Gizmos.color = new Color(1f, 1f, 0.4f, 0.3f);
            Gizmos.DrawWireSphere(levelEndPoint.position, levelEndRadius);
        }
    }

    public void gameOver() {
        if (levelStatus == 0) {
            levelStatus = 2;
            isTransition = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0f, -13f, 0f);
        AudioListener.volume = 0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        levelTime += Time.deltaTime;
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7)) && !isTransition) {
            isPaused = !isPaused;
            Cursor.visible = isPaused;
            if (isPaused)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            transform.Find("PauseTitle").GetComponent<Text>().enabled = isPaused;
            transform.Find("PauseText").GetComponent<Text>().enabled = isPaused;
            transform.Find("PauseGrey").GetComponent<Image>().color = (isPaused ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.clear);
            AudioListener.volume = (isPaused ? 0f : (GlobalVars.gameVol / 100f));
            Time.timeScale = (isPaused ? 0f : 1f);
        }



        if ((player.position - levelEndPoint.position).magnitude < levelEndRadius && levelStatus == 0) {
            levelStatus = 1;
            isTransition = true;
            gameObject.GetComponents<AudioSource>()[3].Play();
        }

        if (isTransition) {
            if (tween < 4f && levelStatus == 2) {
                tween = tween + Time.deltaTime;
                transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, Mathf.Min(3f, tween) / 3f);
                AudioListener.volume = Mathf.Max(0f, 1f - (tween) / 2f);
                if (tween >= 4f) {
                    AudioListener.volume = 1f;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            } else if (tween < 3.5f && levelStatus == 1) {
                tween = tween + Time.deltaTime;
                transform.Find("FadeGrey").GetComponent<Image>().color = Color.Lerp(Color.clear, 
                    new Color(0.78f, 0.78f, 0.78f), MezzMath.fullSine(Mathf.Min(3.5f, tween) / 3.5f));
            }
            else if (tween < 7f && levelStatus == 1) {
                Debug.Log(levelTime);
                tween = tween + Time.deltaTime;
                transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear,
                    Color.black, MezzMath.fullSine(Mathf.Min(3.5f, tween - 3.5f) / 3.5f));
                AudioListener.volume = 1f - (tween - 3.5f)/3.5f;
                if (tween >= 7f) {
                    SceneManager.LoadScene(nextScene);
                }
            }
        } else {
            if (tween > 0f) {
                tween = Mathf.Max(tween - Time.deltaTime, 0f);
                AudioListener.volume = (1.3f - tween) / 1.3f;
                transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, tween);
            }
        }
    }
}
