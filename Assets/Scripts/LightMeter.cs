using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightMeter : MonoBehaviour
{
    public Transform uiCanvas;
    public float maxLight = 1.5f;
    public float movespeedLit = 5f;
    public float movespeedUnlit = 1f;

    float currentLight = 1f;
    // Start is called before the first frame update
    void Start() {
        currentLight = maxLight;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!this.GetComponent<LightDetection>().isLit && currentLight > 0f) {
            currentLight -= Time.fixedDeltaTime;
            this.GetComponent<DonkoController>().speed = movespeedUnlit;
            this.GetComponent<DonkoController>().jump = 0f;
            if (currentLight <= 0f) {
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                currentLight = 0f;
                this.GetComponent<DonkoController>().speed = 0f;
                this.GetComponent<DonkoController>().doDeath();
                this.GetComponent<DonkoController>().enabled = false;
            }
        }
        else if (this.GetComponent<LightDetection>().isLit && currentLight > 0f) {
            currentLight = Mathf.Min(currentLight + Time.fixedDeltaTime, maxLight);
            this.GetComponent<DonkoController>().speed = movespeedLit;
            this.GetComponent<DonkoController>().jump = 7f;

        }
        uiCanvas.Find("BarContainer").Find("BarFill").localScale = new Vector3(MezzMath.fullSine(currentLight / maxLight), 1f, 1f);
        uiCanvas.Find("BarContainer").Find("BarFill").localPosition = new Vector3(125f * MezzMath.fullSine(currentLight / maxLight) - 125f, 0f, 0f);
    }
}
