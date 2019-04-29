using System.Collections;
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
    public int levelMaxScore = 150;

    float levelTime;

    void OnDrawGizmosSelected() {
        if (levelEndPoint != null) {
            Gizmos.color = new Color(1f, 1f, 0.4f, 0.3f);
            Gizmos.DrawWireSphere(levelEndPoint.position, levelEndRadius);
        }
    }

    public void endLevel() {
        GlobalVars.currentScore += (int)Mathf.Max(0f, levelMaxScore - (levelTime + GlobalVars.combinedLevelTime));
        GlobalVars.combinedLevelTime = 0f;
        levelStatus = 1;
        isTransition = true;
        gameObject.GetComponents<AudioSource>()[3].Play();
    }

    public void gameOver() {
        if (levelStatus == 0) {
            levelStatus = 2;
            isTransition = true;
            GlobalVars.combinedLevelTime += levelTime;
        }
    }

    // Start is called before the first frame update
    void Start() {
        Physics.gravity = new Vector3(0f, -13f, 0f);
        AudioListener.volume = 0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (!GlobalVars.uiEnabled) {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {
        levelTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R)) {
            GlobalVars.combinedLevelTime += levelTime;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if ((Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.JoystickButton6)) && !isTransition && isPaused) {
            isTransition = true;
            SceneManager.LoadScene("Menu");
        }

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7)) && !isTransition) {
            isPaused = !isPaused;
            Cursor.visible = isPaused;
            if (isPaused)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;

            transform.Find("PauseTitle").GetComponent<Text>().enabled = isPaused;
            transform.Find("PauseText").GetComponent<Text>().enabled = isPaused;
            transform.Find("PauseText2").GetComponent<Text>().enabled = isPaused;
            transform.Find("PauseGrey").GetComponent<Image>().color = (isPaused ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.clear);
            AudioListener.volume = (isPaused ? 0f : (GlobalVars.gameVol / 100f));
            Time.timeScale = (isPaused ? 0f : 1f);
        }



        if (((player.position - levelEndPoint.position).magnitude < levelEndRadius
            || Input.GetKeyDown(KeyCode.K)
            ) && levelStatus == 0) {
            // Debug.Log(GlobalVars.combinedLevelTime);
            GlobalVars.currentScore += (int)Mathf.Max(0f, levelMaxScore - (levelTime + GlobalVars.combinedLevelTime));
            // Debug.Log(GlobalVars.currentScore);
            GlobalVars.combinedLevelTime = 0f;
            levelStatus = 1;
            isTransition = true;
            gameObject.GetComponents<AudioSource>()[3].Play();
        }

        if (isTransition) {
            if (tween < 4f && levelStatus == 2) {
                tween = tween + Time.deltaTime;
                transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, Mathf.Min(3f, tween) / 3f);
                AudioListener.volume = Mathf.Max(0f, (1f - (tween) / 2f) * (GlobalVars.gameVol / 100f));
                if (tween >= 4f) {
                    AudioListener.volume = (GlobalVars.gameVol / 100f);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            } else if (tween < 3.5f && levelStatus == 1) {
                tween = tween + Time.deltaTime;
                transform.Find("FadeGrey").GetComponent<Image>().color = Color.Lerp(Color.clear, 
                    new Color(0.78f, 0.78f, 0.78f), MezzMath.fullSine(Mathf.Min(3.5f, tween) / 3.5f));
            }
            else if (tween < 7f && levelStatus == 1) {
                tween = tween + Time.deltaTime;
                transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear,
                    Color.black, MezzMath.fullSine(Mathf.Min(3.5f, tween - 3.5f) / 3.5f));
                AudioListener.volume = (1f - (tween - 3.5f)/3.5f) * (GlobalVars.gameVol / 100f);
                if (tween >= 7f) {
                    GlobalVars.lastLevelCompleted++;
                    SceneManager.LoadScene(nextScene);
                }
            }
        } else {
            if (tween > 0f) {
                tween = Mathf.Max(tween - Time.deltaTime, 0f);
                AudioListener.volume = ((1.3f - tween) / 1.3f) * (GlobalVars.gameVol / 100f);
                transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, tween);
            }
        }
    }
}
