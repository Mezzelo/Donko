using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class LightMeter : MonoBehaviour
{
    public Transform uiCanvas;
    public GameObject donkoModel;
    public float maxLight = 1.5f;
    float movespeedLit;
    public float movespeedUnlit = 1f;

    float currentLight = 1f;
    float maxIntensity = 2f;

    float movespeedTween = 1f;
    float oldJump;

    float drainRate = 0f;
    
    float vignetteNormal;
    Vignette postVignette;

    float normalMusicVol = 0f;

    float donkoSlow = 0f;

    Transform donkoDeco;

    Vector3 barPosI;

    Vector3 donkoLeftPosI;
    Vector3 donkoLeftHandI;

    Vector3 donkoRightPosI;
    Vector3 donkoRightHandI;



    public void doDeath() {
        if (currentLight > 0f) {
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            currentLight = 0f;
            this.GetComponent<DonkoController>().speed = 0f;
            this.GetComponent<DonkoController>().doDeath();
            this.GetComponent<DonkoController>().enabled = false;
            uiCanvas.GetComponent<GameDriver>().gameOver();
        }
    }

    public void modifyDrainRate(float drainAdd) {
        drainRate += drainAdd;
    }

    public void addSlowModifier(float slowAdd) {
        donkoSlow += slowAdd;
    }

    // Start is called before the first frame update
    void Start() {
        currentLight = maxLight;
        maxIntensity = this.GetComponent<LightDetection>().sunlight.GetComponent<Light>().intensity;
        movespeedLit = this.GetComponent<DonkoController>().speed;
        oldJump = this.GetComponent<DonkoController>().jump;

        uiCanvas.GetComponent<PostProcessVolume>().profile.TryGetSettings(out postVignette);
        vignetteNormal = postVignette.intensity.value;

        donkoDeco = uiCanvas.Find("BarParent").Find("DonkoDeco");
        barPosI = uiCanvas.Find("BarParent").localPosition;
        donkoLeftHandI = donkoDeco.Find("DonkoLeftHand").localPosition;
        donkoLeftPosI = donkoDeco.Find("DonkoLeft").GetChild(0).localPosition;
        donkoRightHandI = donkoDeco.Find("DonkoRightHand").localPosition;
        donkoRightPosI = donkoDeco.Find("DonkoRight").GetChild(0).localPosition;


        if (uiCanvas.GetComponents<AudioSource>().Length > 1) {
            normalMusicVol = uiCanvas.GetComponents<AudioSource>()[1].volume;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (donkoSlow > 0f)
            donkoSlow = Mathf.Max(0f, donkoSlow - Time.deltaTime * 2f);
        if ((!this.GetComponent<LightDetection>().isLit || drainRate > 0f) && currentLight > 0f) {
            if (!this.GetComponent<LightDetection>().isLit)
                currentLight = Mathf.Max(currentLight - Time.fixedDeltaTime * 0.5f - drainRate * Time.fixedDeltaTime, 0f);
            else
                currentLight = Mathf.Max(currentLight + Time.fixedDeltaTime * 0.5f - drainRate * Time.fixedDeltaTime, 0f);

            this.GetComponent<LightDetection>().sunlight.GetComponent<Light>().intensity =
                maxIntensity * currentLight / maxLight;
            postVignette.intensity.value = Mathf.Lerp(vignetteNormal, 1f, 
                1f - currentLight / maxLight + Mathf.Lerp(0f, Mathf.Sin(Time.time * 13f) * 0.1f 
                + Random.Range(-0.15f, 0.15f) * (1f - currentLight / maxLight), 1f - currentLight / maxLight));


            if (movespeedTween > 0f)
                movespeedTween = Mathf.Max(movespeedTween - Time.fixedDeltaTime * 2.5f, 0f);
            this.GetComponent<DonkoController>().speed = 
                Mathf.Max(0f, Mathf.Lerp(movespeedLit * movespeedUnlit, movespeedLit, movespeedTween) - donkoSlow);
            this.GetComponent<DonkoController>().jump = 0f;
        }
        else if (this.GetComponent<LightDetection>().isLit && currentLight > 0f && drainRate <= 0f) {
            currentLight = Mathf.Min(currentLight + Time.fixedDeltaTime * 0.5f, maxLight);

            this.GetComponent<LightDetection>().sunlight.GetComponent<Light>().intensity =
                maxIntensity * currentLight / maxLight;
            postVignette.intensity.value = Mathf.Lerp(vignetteNormal, 1f, 1f - currentLight / maxLight);

            if (movespeedTween < 1f)
                movespeedTween = Mathf.Min(movespeedTween + Time.fixedDeltaTime * 2.5f, 1f);
            this.GetComponent<DonkoController>().speed = 
                Mathf.Max(Mathf.Lerp(movespeedLit * movespeedUnlit, movespeedLit, movespeedTween) - donkoSlow, 0f);
            this.GetComponent<DonkoController>().jump = oldJump;
        }
        if (currentLight <= 0f) {
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            currentLight = 0f;
            this.GetComponent<DonkoController>().speed = 0f;
            this.GetComponent<DonkoController>().doDeath();
            this.GetComponent<DonkoController>().enabled = false;
            uiCanvas.GetComponent<GameDriver>().gameOver();
        }
        // Debug.Log(donkoModel.GetComponent<SkinnedMeshRenderer>().materials[1].shader.);
        uiCanvas.GetComponent<AudioSource>().volume = (1f - currentLight / maxLight) * 0.2f;
        if (uiCanvas.GetComponents<AudioSource>().Length > 1) {
            uiCanvas.GetComponents<AudioSource>()[1].volume = Mathf.Lerp(0f, normalMusicVol * GlobalVars.musicVol / 100f, currentLight / maxLight);
            uiCanvas.GetComponents<AudioSource>()[2].volume = Mathf.Lerp(normalMusicVol * GlobalVars.musicVol / 100f * 3.5f, 0f, currentLight / maxLight);
        }
        if (GlobalVars.uiEnabled) {
            uiCanvas.Find("BarParent").localPosition = barPosI + new Vector3(0f, 1.5f * Mathf.Cos(Time.time * 2.3f + 4.5f), 0f);
            uiCanvas.Find("BarParent").localRotation = Quaternion.Euler(0f, 0f, 1.3f * Mathf.Cos(Time.time * 1.5f + 4.5f));
            uiCanvas.Find("BarParent").Find("BarContainer").Find("BarFill").localScale = new Vector3(MezzMath.fullSine(currentLight / maxLight), 1f, 1f);
            uiCanvas.Find("BarParent").Find("BarContainer").Find("BarFill").localPosition = new Vector3(125f * MezzMath.fullSine(currentLight / maxLight) - 125f, 0f, 0f);

            donkoDeco.Find("DonkoLeft").GetChild(0).localPosition = donkoLeftPosI + ((new Vector3(-5f, -1.5f, 0f) 
                + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))) 
                * MezzMath.fullSine(1f - currentLight / maxLight))
                + new Vector3(0.3f, 0.5f * Mathf.Sin(Time.time * 2.5f + 6f), 0f);
            donkoDeco.Find("DonkoLeft").GetChild(0).localRotation = Quaternion.Euler(0f, 0f, -20f * MezzMath.fullSine(1f - currentLight / maxLight));

            donkoDeco.Find("DonkoLeftHand").localPosition = donkoLeftHandI + new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f)) * MezzMath.fullSine(1f - currentLight / maxLight);

            donkoDeco.Find("DonkoRight").GetChild(0).localPosition = donkoRightPosI + new Vector3(-5f, -3f, 0f) * MezzMath.fullSine(1f - currentLight / maxLight) + new Vector3(0f, 0.85f * Mathf.Sin(Time.time * 2.5f + 10f), 0f);
            donkoDeco.Find("DonkoRightArm").GetChild(0).localPosition = donkoRightPosI + new Vector3(-5f, -3f, 0f) * MezzMath.fullSine(1f - currentLight / maxLight) + new Vector3(0f, 0.85f * Mathf.Sin(Time.time * 2.5f + 10f), 0f);
            donkoDeco.Find("DonkoRightHand").localPosition = donkoRightHandI + new Vector3(3f, 0f, 0f) * MezzMath.fullSine(1f - currentLight / maxLight);
            donkoDeco.Find("DonkoRightHand").localRotation = Quaternion.Euler(0f, 0f, 15f * MezzMath.fullSine(1f - currentLight / maxLight));
            donkoDeco.Find("DonkoRight").GetChild(0).rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -15f), MezzMath.fullSine(1f - currentLight / maxLight));
            donkoDeco.Find("DonkoRightArm").GetChild(0).rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -15f), MezzMath.fullSine(1f - currentLight / maxLight));
            donkoDeco.Find("DonkoRightArm").GetChild(0).GetChild(0).rotation = Quaternion.Euler(0f, 0f, 30f * MezzMath.fullSine(currentLight / maxLight) * Mathf.Sin(Time.time * 1.75f));
        }


    }
}
