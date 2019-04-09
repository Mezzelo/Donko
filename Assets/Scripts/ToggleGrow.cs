using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGrow : ToggleBase
{
    public float animTime;
    public Light lightComponent;
    public Vector3 posOffsetDeactivated;
    public Vector3 rotOffsetDeactivated;
    public Vector3 scaleDeactivated;
    public float intensityOffset;

    public bool debug = false;

    Vector3 origPos;
    Vector3 origRot;
    Vector3 origScale;

    float intensityOrig;
    float animC;

    public override void toggleActivation(int mode) {
        base.toggleActivation(mode);
        if (isActivated) {
            if (gameObject.GetComponent<AudioSource>() != null && mode == 0) {
                gameObject.GetComponent<AudioSource>().Play();
            }
            if (mode == 1) {
                animC = animTime - 0.01f;
            }
        } else {
            if (gameObject.GetComponent<AudioSource>() != null && mode == 0) {
                if (gameObject.GetComponents<AudioSource>().Length > 1)
                    gameObject.GetComponents<AudioSource>()[1].Play();
                else
                    gameObject.GetComponent<AudioSource>().Play();
            }
            if (mode == 1) {
                animC = 0.01f;
                // Debug.Log("tween off");
            }
        }
    }
    
    // Start is called before the first frame update
    void Start() {
        animC = animTime;
        origPos = transform.position;
        origRot = transform.rotation.eulerAngles;
        origScale = transform.localScale;
        if (lightComponent != null)
            intensityOrig = lightComponent.intensity;
    }

    // Update is called once per frame
    void Update() {
        // if (debug)
        //     Debug.Log(isActivated + ", " + animC);
        if (animC < animTime && isActivated) {
            animC = Mathf.Min(animC + Time.deltaTime, animTime);
            transform.position = Vector3.Lerp(origPos + posOffsetDeactivated, origPos, MezzMath.fullSine(animC / animTime));
            transform.rotation = Quaternion.Euler(Vector3.Lerp(origRot + rotOffsetDeactivated,
                origRot, MezzMath.fullSine(animC / animTime)));
            transform.localScale = Vector3.Lerp(scaleDeactivated, origScale, MezzMath.fullSine(animC / animTime));
            if (lightComponent != null)
                lightComponent.intensity = Mathf.Lerp(intensityOrig + intensityOffset, intensityOrig, MezzMath.fullSine(animC / animTime));
        }
        else if (animC > 0f && !isActivated) {
            animC = Mathf.Max(animC - Time.deltaTime, 0f);
            transform.position = Vector3.Lerp(origPos + posOffsetDeactivated, origPos, MezzMath.fullSine(animC / animTime));
            transform.rotation = Quaternion.Euler(Vector3.Lerp(origRot + rotOffsetDeactivated,
                origRot, MezzMath.fullSine(animC / animTime)));
            transform.localScale = Vector3.Lerp(scaleDeactivated, origScale, MezzMath.fullSine(animC / animTime));
            if (lightComponent != null)
                lightComponent.intensity = Mathf.Lerp(intensityOrig + intensityOffset, intensityOrig, MezzMath.fullSine(animC / animTime));
        }
    }
}
