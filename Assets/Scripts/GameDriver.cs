using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDriver : MonoBehaviour
{

    public Transform player;

    float tween = 0f;

    bool isTransition = false;
    bool isPaused = false;
    
    Transform levelEndPoint;
    

    public void gameOver() {
        GlobalVars.menuScreen = 1;
        isTransition = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0f, -13f, 0f);
    }

    // Update is called once per frame
    void Update() {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.JoystickButton7)) && !isTransition) {
            isPaused = !isPaused;
            transform.Find("PauseTitle").GetComponent<Text>().enabled = isPaused;
            transform.Find("PauseText").GetComponent<Text>().enabled = isPaused;
            transform.Find("PauseGrey").GetComponent<Image>().color = (isPaused ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.clear);
            AudioListener.volume = (isPaused ? 0f : (GlobalVars.gameVol / 100f));
            Time.timeScale = (isPaused ? 0f : 1f);
        }

        // if (player.position.x > 60f) {
        //     GlobalVars.menuScreen = 2;
        //     isTransition = true;
        // }
        if (isTransition) {
            if (tween < 4f) {
                tween = tween + Time.deltaTime;
                transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, Mathf.Min(3f, tween)/3f);
                AudioListener.volume = Mathf.Max(0f, 1f - (tween) / 2f);
                if (tween >= 4f) {
                    AudioListener.volume = 1f;
                    SceneManager.LoadScene("Menu");
                }
            }
        }
    }
}
