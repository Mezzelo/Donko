using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerAnim : MonoBehaviour
{
    public Vector3 startPos;
    float startTick;
    float dir = 1f;
    float animSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x * (Random.Range(0, 2) * -2 + 1), transform.localScale.y, transform.localScale.z);
        startPos = transform.position;
        startTick = Time.time;
        dir = Random.Range(0.8f, 1.2f) * (Random.Range(0, 2) * -2 + 1);
        animSpeed = Random.Range(1.5f, 3.5f);
    }

    // Update is called once per frame
    void Update() {
        transform.position = startPos + new Vector3(0f, 10f, 0f) * Mathf.Sin(Time.time * animSpeed - startTick * 2f) * dir;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f + 10f * Mathf.Sin(Time.time * animSpeed - startTick) * dir));
    }
}
