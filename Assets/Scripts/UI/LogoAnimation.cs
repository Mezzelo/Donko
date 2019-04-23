using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoAnimation : MonoBehaviour {

    float logoTween = 0f;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.rotation = Quaternion.Euler(0f, 0f, 2f * Mathf.Sin(Time.time));
        logoTween = Mathf.Repeat(logoTween + Time.deltaTime, 18f);
        transform.GetChild(0).GetComponent<Image>().color = Color.Lerp(Color.cyan,
            Color.white, 0.65f + 0.25f * Mathf.Sin(Time.time/3f));
        if (logoTween < 4f) {
            transform.GetChild(1).GetComponent<Image>().color = Color.Lerp(Color.white,
                new Color(1f, 1f, 1f, 0f), logoTween / 4f);
            transform.GetChild(2).GetComponent<Image>().color = Color.white;
            transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(Color.white,
                new Color(1f, 1f, 1f, 0f), 1f - logoTween / 4f);
            transform.GetChild(4).GetComponent<Image>().color = Color.clear;
            transform.GetChild(5).GetComponent<Image>().color = Color.clear;
        } else if (logoTween < 6f) {
            transform.GetChild(1).GetComponent<Image>().color = Color.clear;
            transform.GetChild(2).GetComponent<Image>().color = Color.white;
            transform.GetChild(3).GetComponent<Image>().color = Color.white;
            transform.GetChild(4).GetComponent<Image>().color = Color.clear;
            transform.GetChild(5).GetComponent<Image>().color = Color.clear;
        } else if (logoTween < 10f) {
            transform.GetChild(1).GetComponent<Image>().color = Color.clear;
            transform.GetChild(2).GetComponent<Image>().color = Color.Lerp(Color.white,
                new Color(1f, 1f, 1f, 0f), (logoTween - 6f) / 4f);
            transform.GetChild(3).GetComponent<Image>().color = Color.white;
            transform.GetChild(4).GetComponent<Image>().color = Color.Lerp(Color.white,
                new Color(1f, 1f, 1f, 0f), 1f - (logoTween - 6f) / 4f);
            transform.GetChild(5).GetComponent<Image>().color = Color.clear;
        }
        else if (logoTween < 12f) {
            transform.GetChild(1).GetComponent<Image>().color = Color.clear;
            transform.GetChild(2).GetComponent<Image>().color = Color.clear;
            transform.GetChild(3).GetComponent<Image>().color = Color.white;
            transform.GetChild(4).GetComponent<Image>().color = Color.white;
            transform.GetChild(5).GetComponent<Image>().color = Color.clear;
        }
        else if (logoTween < 16f) {
            transform.GetChild(1).GetComponent<Image>().color = Color.clear;
            transform.GetChild(2).GetComponent<Image>().color = Color.clear;
            transform.GetChild(3).GetComponent<Image>().color = Color.Lerp(Color.white,
                new Color(1f, 1f, 1f, 0f), (logoTween - 12f) / 4f);
            transform.GetChild(4).GetComponent<Image>().color = Color.white;
            transform.GetChild(5).GetComponent<Image>().color = Color.Lerp(Color.white,
                new Color(1f, 1f, 1f, 0f), 1f - (logoTween - 12f) / 4f);
        }
        else if (logoTween < 18f) {
            transform.GetChild(1).GetComponent<Image>().color = Color.clear;
            transform.GetChild(2).GetComponent<Image>().color = Color.clear;
            transform.GetChild(3).GetComponent<Image>().color = Color.clear;
            transform.GetChild(4).GetComponent<Image>().color = Color.white;
            transform.GetChild(5).GetComponent<Image>().color = Color.white;
        }
    }
}
