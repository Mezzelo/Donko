using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightMeter : MonoBehaviour
{
    public Transform uiCanvas;
    public float maxLight = 1.5f;
    float movespeedLit;
    public float movespeedUnlit = 1f;

    float currentLight = 1f;
    float maxIntensity = 2f;

    float movespeedTween = 1f;
    // Start is called before the first frame update
    void Start() {
        currentLight = maxLight;
        maxIntensity = this.GetComponent<LightDetection>().sunlight.GetComponent<Light>().intensity;
        movespeedLit = this.GetComponent<DonkoController>().speed;
    }

    // Update is called once per frame
    void FixedUpdate() {

        if (!this.GetComponent<LightDetection>().isLit && currentLight > 0f) {
            currentLight = Mathf.Max(currentLight - Time.fixedDeltaTime, 0f);
            this.GetComponent<LightDetection>().sunlight.GetComponent<Light>().intensity =
                maxIntensity * currentLight / maxLight;
            if (movespeedTween > 0f)
                movespeedTween = Mathf.Max(movespeedTween - Time.fixedDeltaTime * 2.5f, 0f);
            this.GetComponent<DonkoController>().speed = Mathf.Lerp(movespeedLit * movespeedUnlit, movespeedLit, movespeedTween);
            this.GetComponent<DonkoController>().jump = 0f;
            if (currentLight <= 0f) {
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                currentLight = 0f;
                this.GetComponent<DonkoController>().speed = 0f;
                this.GetComponent<DonkoController>().doDeath();
                this.GetComponent<DonkoController>().enabled = false;
                uiCanvas.GetComponent<GameDriver>().gameOver();
            }
        }
        else if (this.GetComponent<LightDetection>().isLit && currentLight > 0f) {
            currentLight = Mathf.Min(currentLight + Time.fixedDeltaTime, maxLight);
            this.GetComponent<LightDetection>().sunlight.GetComponent<Light>().intensity =
                maxIntensity * currentLight / maxLight;
            if (movespeedTween < 1f)
                movespeedTween = Mathf.Min(movespeedTween + Time.fixedDeltaTime * 2.5f, 1f);
            this.GetComponent<DonkoController>().speed = Mathf.Lerp(movespeedLit * movespeedUnlit, movespeedLit, movespeedTween);
            this.GetComponent<DonkoController>().jump = 7f;
        }
        uiCanvas.GetComponent<AudioSource>().volume = (1f - currentLight / maxLight) * 0.2f;
        // uiCanvas.Find("BarContainer").Find("BarFill").localScale = new Vector3(MezzMath.fullSine(currentLight / maxLight), 1f, 1f);
        // uiCanvas.Find("BarContainer").Find("BarFill").localPosition = new Vector3(125f * MezzMath.fullSine(currentLight / maxLight) - 125f, 0f, 0f);
    }
}
