using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MenuScript : MonoBehaviour
{

    Transform[] menuObjects;
    Transform[] buttons;
    // Transform[] menuItems;
    Vector3[] origPositions;
    float[] buttonRots;

    public InputField highscoreName;

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
        if (currentMenu != -1 && origPositions != null) {
            for (int i = 0; i < menuObjects[currentMenu].childCount && i < origPositions.Length; i++) {
                 menuObjects[currentMenu].GetChild(i).localPosition = origPositions[i];
            }
        }
        currentMenu = menu;
        if (menu != -1) {

            for (int i = 0; i < menuObjects.Length; i++) {
                menuObjects[i].gameObject.SetActive(menu == i);
                if (menu == i)
                    EventSystem.current.SetSelectedGameObject(menuObjects[menu].GetChild(0).gameObject);
            }


            int numButtons = 0;
            if (menu < menuObjects.Length) {
                origPositions = new Vector3[menuObjects[currentMenu].childCount];
                for (int i = 0; i < menuObjects[currentMenu].childCount; i++) {
                    origPositions[i] = menuObjects[currentMenu].GetChild(i).localPosition;
                    if (menuObjects[currentMenu].GetChild(i).GetComponent<Button>() != null)
                        numButtons++;
                }
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
        this.GetComponents<AudioSource>()[soundType].Play();
    }

    public void quit() {
        Application.Quit();
    }

    public void toggleUiEnabled() {
        GlobalVars.uiEnabled = !GlobalVars.uiEnabled;
    }

    public void toggleVolume() {
        GlobalVars.gameVol = (GlobalVars.gameVol + 10) % 110;
        AudioListener.volume = (GlobalVars.gameVol / 100f);
    }

    public void toggleMusicVolume() {
        GlobalVars.musicVol = (GlobalVars.musicVol + 10) % 110;
        AudioListener.volume = (GlobalVars.musicVol / 100f);
        GameObject.Find("Camera").GetComponent<AudioSource>().volume = 0.15f * (GlobalVars.musicVol / 100f);
    }

    public void updateOptions() {
        if (menuObjects[4].Find("UIOption"))
            menuObjects[4].Find("UIOption").GetComponent<Text>().text =
                ("USER INTERFACE: " + (GlobalVars.uiEnabled ? "ON" : "OFF"));
        if (menuObjects[4].Find("VolumeOption"))
            menuObjects[4].Find("VolumeOption").GetComponent<Text>().text =
                ("Master Volume: " + GlobalVars.gameVol);
        if (menuObjects[4].Find("MusicOption"))
            menuObjects[4].Find("MusicOption").GetComponent<Text>().text =
                ("Music Volume: " + GlobalVars.musicVol);
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

    void updateScoreTable() {
        for (int i = 0; i < GlobalVars.highScores.Length; i++) {
            transform.Find("ScoreScreen").GetChild(i + 2).GetComponent<Text>().text =
                GlobalVars.highScoreNames[i] + " - " + GlobalVars.highScores[i];
        }
    }

    public void addNewScore() {
        GlobalVars.highScores[GlobalVars.newScoreIndex] = GlobalVars.currentScore;
        if (highscoreName.text == "")
            GlobalVars.highScoreNames[GlobalVars.newScoreIndex] = "???";
        else
            GlobalVars.highScoreNames[GlobalVars.newScoreIndex] = highscoreName.text;

        string dataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        BinaryFormatter bf = new BinaryFormatter();
        Directory.CreateDirectory(dataPath + "/ProdForestMushroomEmoji");
        FileStream file = File.Create(dataPath + "/ProdForestMushroomEmoji/donkodata.dat");

        PlayerData data = new PlayerData();
        for (int i = 0; i < GlobalVars.highScores.Length; i++) {
            data.highScores[i] = GlobalVars.highScores[i];
            data.highScoreNames[i] = GlobalVars.highScoreNames[i];
        }

        bf.Serialize(file, data);
        file.Close();
        updateScoreTable();
        GlobalVars.currentScore = 0;
        toggleMenus(5);
    }


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = GlobalVars.gameVol / 100f;
        Time.timeScale = 1f;
        menuObjects = new Transform[transform.childCount - 2];
        for (int i = 1; i < transform.childCount - 1; i++) {
            menuObjects[i - 1] = transform.GetChild(i);
            menuObjects[i - 1].localPosition = Vector3.zero;
        }
        updateOptions();
        // if (GlobalVars.menuScreen != 0) {
            isTransition = true;
            fadeTween = 1f;
            transform.Find("Fade").GetComponent<Image>().color = Color.black;
            // if (currentMenu == 1)
            //     this.GetComponents<AudioSource>()[2].Play();
            // else if (currentMenu == 2)
            //     this.GetComponents<AudioSource>()[3].Play();

        // }
        bool gotHighScore = false;
        if (GlobalVars.lastLevelCompleted == -1) {
            GlobalVars.lastLevelCompleted = 0;
            GlobalVars.highScores[0] = 50;
            GlobalVars.highScores[1] = 150;
            GlobalVars.highScores[2] = 250;
            GlobalVars.highScores[3] = 400;
            GlobalVars.highScores[4] = 450;
            GlobalVars.highScoreNames[0] = "Pwease no Steppy";
            GlobalVars.highScoreNames[1] = "Disco Broccoli";
            GlobalVars.highScoreNames[2] = "Robert_Cheeto";
            GlobalVars.highScoreNames[3] = "Matt from Wii Sports";
            GlobalVars.highScoreNames[4] = "JEX";

            if (!GlobalVars.dataHasLoaded) {
                GlobalVars.dataHasLoaded = true;
                string dataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                BinaryFormatter bf = new BinaryFormatter();
                if (File.Exists(dataPath + "/ProdForestMushroomEmoji/donkodata.dat")) {
                    FileStream file = File.Open(dataPath + "/ProdForestMushroomEmoji/donkodata.dat", FileMode.Open);
                    PlayerData data;
                    try {
                        data = (PlayerData)bf.Deserialize(file);
                        for (int i = 0; i < GlobalVars.highScores.Length; i++) {
                            GlobalVars.highScores[i] = data.highScores[i];
                            GlobalVars.highScoreNames[i] = data.highScoreNames[i];
                        }
                    }
                    catch (EndOfStreamException) {
                    }
                    catch (System.Runtime.Serialization.SerializationException) {
                    }
                    file.Close();
                } else {
                    Directory.CreateDirectory(dataPath + "/ProdForestMushroomEmoji");
                    FileStream file = File.Create(dataPath + "/ProdForestMushroomEmoji/donkodata.dat");

                    PlayerData data = new PlayerData();
                    for (int i = 0; i < GlobalVars.highScores.Length; i++) {
                        data.highScores[i] = GlobalVars.highScores[i];
                        data.highScoreNames[i] = GlobalVars.highScoreNames[i];
                    }

                    bf.Serialize(file, data);
                    file.Close();
                }
            }
        } else if (GlobalVars.lastLevelCompleted >= 1) {
            Debug.Log("checkScore");
            for (int i = 4; i > -1; i--) {
                if (GlobalVars.currentScore > GlobalVars.highScores[i]) {
                    for (int g = 1; g <= i; g++) {
                        GlobalVars.highScores[g - 1] = GlobalVars.highScores[g];
                        GlobalVars.highScoreNames[g - 1] = GlobalVars.highScoreNames[g];
                    }
                    GlobalVars.newScoreIndex = i;
                    toggleMenus(6);
                    gotHighScore = true;
                    break;
                }
            }
        }
        updateScoreTable();
        GlobalVars.lastLevelCompleted = 0;
        if (!gotHighScore) {
            GlobalVars.currentScore = 0;
            toggleMenus(GlobalVars.menuScreen);
        }
        GlobalVars.combinedLevelTime = 0f;

    }

    // Update is called once per frame
    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.P)) {
            GlobalVars.currentScore = 150;
            GlobalVars.newScoreIndex = 0;
            highscoreName.text = "reee";
            addNewScore();

            updateScoreTable();
        }*/
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return)) {
            if (EventSystem.current.currentSelectedGameObject == null && currentMenu >= 0) {
                EventSystem.current.SetSelectedGameObject(menuObjects[currentMenu].GetChild(0).gameObject);
            }
        }
        
        if (currentMenu > -1 && currentMenu < menuObjects.Length && currentMenu != 6) {
            for (int i = 0; i < menuObjects[currentMenu].childCount && i < origPositions.Length; i++) {
                menuObjects[currentMenu].GetChild(i).localPosition = origPositions[i] + new Vector3(0f,
                    Mathf.Sin(Time.time + i * 1.5f) * 10f, 0f);
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

[System.Serializable]
class PlayerData {
    public int[] highScores = { 0, 0, 0, 0, 0 };
    public string[] highScoreNames = { "", "", "", "", "" };
}
