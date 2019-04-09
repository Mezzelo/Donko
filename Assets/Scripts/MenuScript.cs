using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour
{

    Transform[] menuObjects;
    Transform[] buttons;
    float[] buttonRots;
    public AudioClip[] footstepSounds;

    bool isTransition = false;
    float fadeTween = 0f;
    int currentMenu = 0;

    IEnumerator LoadGame() {
        // yield return new WaitForSeconds(0.35f);
        // transform.Find("Fade").GetComponent<Image>().color = Color.black;
        toggleMenus(-1);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("SampleScene");
    }

    public void toggleMenus(int menu) {
        currentMenu = menu;
        if (menu != -1) {
            for (int i = 0; i < menuObjects.Length; i++) {
                menuObjects[i].gameObject.SetActive(menu == i);
                if (menu == i)
                    EventSystem.current.SetSelectedGameObject(menuObjects[menu].GetChild(0).gameObject);
            }


            int numButtons = 0;
            for (int i = 0; i < menuObjects[currentMenu].childCount; i++) {
                if (menuObjects[currentMenu].GetChild(i).GetComponent<Button>() != null)
                    numButtons++;
            }

            buttons = new Transform[numButtons];
            buttonRots = new float[numButtons];

            int g = 0;
            for (int i = 0; i < menuObjects[currentMenu].childCount && g < numButtons; i++) {
                if (menuObjects[currentMenu].GetChild(i).GetComponent<Button>() != null) {
                    buttons[g] = menuObjects[currentMenu].GetChild(i);
                    buttonRots[g] = Random.Range(-4.5f, 4.5f);
                    if (buttonRots[g] < 0f)
                        buttonRots[g] = 360f + buttonRots[g];
                    buttons[g].rotation = Quaternion.Euler(0f, 0f, buttonRots[g]);
                    g++;
                }
            }
        }
    }

    public void menuSound(int soundType) {
        if (soundType == 0)
            this.GetComponents<AudioSource>()[soundType].clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        this.GetComponents<AudioSource>()[soundType].Play();
    }

    public void quit() {
        Application.Quit();
    }

    public void setDifficulty(int newDifficulty) {
        GlobalVars.difficulty = newDifficulty;
    }

    public void toggleMinimap() {
        GlobalVars.minimap = !GlobalVars.minimap;
    }

    public void togglePlayerModel() {
        GlobalVars.playerShadow = !GlobalVars.playerShadow;
    }

    public void toggleVolume() {
        GlobalVars.gameVol = (GlobalVars.gameVol + 10) % 110;
        AudioListener.volume = (GlobalVars.gameVol / 100f);
    }

    public void updateOptions() {
        if (menuObjects[currentMenu].Find("MinimapOption"))
            menuObjects[currentMenu].Find("MinimapOption").GetComponent<Text>().text =
                ("Minimap: " + (GlobalVars.minimap ? "ON" : "OFF"));
        if (menuObjects[currentMenu].Find("PlayermodelOption"))
            menuObjects[currentMenu].Find("PlayermodelOption").GetComponent<Text>().text =
                ("Player model: " + (GlobalVars.playerShadow ? "ON" : "OFF"));
        if (menuObjects[currentMenu].Find("VolumeOption"))
            menuObjects[currentMenu].Find("VolumeOption").GetComponent<Text>().text =
                ("Volume: " + GlobalVars.gameVol);
    }

    public void setOptionText(string newText) {
        if (currentMenu > -1 && currentMenu < menuObjects.Length) {
            if (menuObjects[currentMenu].Find("OptionText"))
                menuObjects[currentMenu].Find("OptionText").GetComponent<Text>().text = newText;
        }
    }

    public void startGame() {
        if (!isTransition) {
            isTransition = true;
            fadeTween = 0f;
            transform.Find("Fade").GetComponent<Image>().color = Color.black;
            this.GetComponents<AudioSource>()[1].Play();
            StartCoroutine(LoadGame());
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        menuObjects = new Transform[transform.childCount - 2];
        for (int i = 1; i < transform.childCount - 1; i++) {
            menuObjects[i - 1] = transform.GetChild(i);
            menuObjects[i - 1].localPosition = Vector3.zero;
        }
        currentMenu = 4;
        updateOptions();
        toggleMenus(GlobalVars.menuScreen);
        currentMenu = GlobalVars.menuScreen;
        if (GlobalVars.menuScreen != 0) {
            isTransition = true;
            fadeTween = 1f;
            transform.Find("Fade").GetComponent<Image>().color = Color.black;
            if (currentMenu == 1)
                this.GetComponents<AudioSource>()[2].Play();
            else if (currentMenu == 2)
                this.GetComponents<AudioSource>()[3].Play();

        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return)) {
            if (EventSystem.current.currentSelectedGameObject == null && currentMenu >= 0) {
                EventSystem.current.SetSelectedGameObject(menuObjects[currentMenu].GetChild(0).gameObject);
            }
        }
        
        if (buttons.Length > 1 && currentMenu != -1) {
            
            for (int i = 0; i < buttons.Length; i++) {
                if (EventSystem.current.currentSelectedGameObject.transform == buttons[i]) {
                    if (buttons[i].rotation.eulerAngles.z > 300f) {
                        buttons[i].rotation =
                            Quaternion.Euler(0f, 0f, Mathf.Min(buttons[i].rotation.eulerAngles.z + (25f * Time.deltaTime), 360f)%360f);
                    }
                    else if (buttons[i].rotation.eulerAngles.z > 0f)
                        buttons[i].rotation =
                            Quaternion.Euler(0f, 0f, Mathf.Max(buttons[i].rotation.eulerAngles.z - (25f * Time.deltaTime), 0f));
                } else {
                    if (buttons[i].rotation.eulerAngles.z < buttonRots[i] && buttonRots[i] < 60f)
                        buttons[i].rotation =
                            Quaternion.Euler(0f, 0f, Mathf.Min(buttons[i].rotation.eulerAngles.z + (25f * Time.deltaTime), buttonRots[i]));
                    else if (buttons[i].rotation.eulerAngles.z < buttonRots[i] && buttonRots[i] > 60f)
                        buttons[i].rotation =
                            Quaternion.Euler(0f, 0f, Mathf.Max(buttons[i].rotation.eulerAngles.z - (25f * Time.deltaTime), buttonRots[i]));
                }
            }
    }

        if (fadeTween > 0f) {
            fadeTween = Mathf.Max(0f, fadeTween - Time.deltaTime);
            transform.Find("Fade").GetComponent<Image>().color = Color.Lerp(Color.clear, Color.black, fadeTween);
            if (fadeTween == 0f)
                isTransition = false;
        }
    }
}
