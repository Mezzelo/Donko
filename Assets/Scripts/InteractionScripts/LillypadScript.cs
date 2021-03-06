﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LillypadScript : ToggleBase
{

    public float bobMultiplier = 1f;
    public float bobRotMultiplier = 1f;
    public float bobSpeed = 1f;
    public bool doSpin = true;

    float startTick;
    float animC;

    float sineOffset;

    Vector3 origPos;
    Vector3 origRot;

    float dir;



    public override void toggleActivation(int mode) {
        base.toggleActivation(mode);
        if (isActivated) {
            gameObject.GetComponent<BoxCollider>().enabled = true;
            if (mode == 1) {
                animC = 0.4f - 0.01f;
            }
        }
        else {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            if (mode == 1) {
                animC = 0.01f;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.position;
        origRot = transform.rotation.eulerAngles;
        startTick = Time.time;
        sineOffset = Random.Range(0, Mathf.PI);
        dir = Random.Range(0.8f, 1.2f) * (Random.Range(0, 2) * -2 + 1);
        if (isActivated)
            animC = 1.5f;
    }

    // Update is called once per frame
    void Update() {
        if (animC < 1.5f && isActivated) {
            animC = Mathf.Min(animC + Time.deltaTime, 1.5f);
            if (animC <= 0.55f && animC + Time.deltaTime > 0.55f) {
                if (gameObject.GetComponent<ParticleSystem>() != null)
                    gameObject.GetComponent<ParticleSystem>().Play();
                if (gameObject.GetComponent<AudioSource>() != null) {
                    gameObject.GetComponent<AudioSource>().Play();
                }
            }
        }
        else if (animC > 0f && !isActivated) {
            animC = Mathf.Max(animC - Time.deltaTime, 0f);
            if (animC > 0.7f && animC - Time.deltaTime <= 0.7f) {
                if (gameObject.GetComponent<ParticleSystem>() != null)
                    gameObject.GetComponent<ParticleSystem>().Play();
                if (gameObject.GetComponent<AudioSource>() != null) {
                    if (gameObject.GetComponents<AudioSource>().Length > 1)
                        gameObject.GetComponents<AudioSource>()[1].Play();
                    else
                        gameObject.GetComponent<AudioSource>().Play();
                }
            }
        }
        transform.position = origPos + new Vector3(0f, Mathf.Sin(Time.time * bobSpeed - startTick + sineOffset) * 0.05f * dir * bobMultiplier, 0f) 
            + new Vector3(0f, -1f * bobMultiplier + 1f * MezzMath.halfSine(animC/ 1.5f) * bobMultiplier, 0f);
        transform.rotation = Quaternion.Euler(origRot + new Vector3(Mathf.Sin(Time.time * bobSpeed * 2f - startTick + sineOffset) * 2.5f * dir * bobRotMultiplier, (doSpin ? (Time.time - startTick + sineOffset) * 5f * dir : 0f), 0f));
    }
}
