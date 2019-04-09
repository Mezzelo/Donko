using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DonkoInventory : MonoBehaviour
{
    public int[] flickers;
    public Transform canvas;
    List<Transform> flickerObjs;

    public GameObject[] flickerPrefabs;

    void Start() {
        flickerObjs = new List<Transform>();
    }

    public void changeFlickerCount(int index, int increment) {
        flickers[index] += increment;
        if (index == 0) {
            if (increment > 0) {
                Transform newFlicker = GameObject.Instantiate(flickerPrefabs[index], canvas.Find("FlickerPos")).transform;
                newFlicker.position = canvas.Find("FlickerPos").position + new Vector3(Random.Range(-30f, 30f) + flickerObjs.Count * 100f, Random.Range(-30f, 30f), 0f);
                flickerObjs.Add(newFlicker);
            } else {
                Transform removeFlicker = flickerObjs[flickerObjs.Count - 1];
                flickerObjs.RemoveAt(flickerObjs.Count - 1);
                GameObject.Destroy(removeFlicker.gameObject);
            }
        }
    }
}
