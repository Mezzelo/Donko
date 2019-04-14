using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DonkoInventory : MonoBehaviour
{
    public int[] flickers;
    public Transform canvas;
    List<Transform> flickerObjs;
    List<int> flickerTypes;

    public GameObject[] flickerPrefabs;

    void Start() {
        flickerObjs = new List<Transform>();
        flickerTypes = new List<int>();
    }

    public void changeFlickerCount(int index, int increment) {
        flickers[index] += increment;
        if (increment > 0) {
            Transform newFlicker = GameObject.Instantiate(flickerPrefabs[index], canvas.Find("FlickerPos")).transform;
            newFlicker.position = canvas.Find("FlickerPos").position + new Vector3(Random.Range(-30f, 30f) + (flickerObjs.Count + 1) * 100f, Random.Range(-30f, 30f), 0f);
            flickerObjs.Add(newFlicker);
            flickerTypes.Add(index);
        }
        else {
            for (int i = 0; i < flickerObjs.Count; i++) {
                if (flickerTypes[i] == index) {
                    Transform removeFlicker = flickerObjs[i];
                    flickerObjs.RemoveAt(i);
                    flickerTypes.RemoveAt(i);
                    GameObject.Destroy(removeFlicker.gameObject);
                    for (int g = 0; g < flickerObjs.Count; g++)
                        flickerObjs[g].GetComponent<FlickerAnim>().startPos = canvas.Find("FlickerPos").position + 
                            new Vector3(Random.Range(-30f, 30f) + (g + 1) * 100f, Random.Range(-30f, 30f), 0f);
                    break;
                }
            }
        }
    }
}
