using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetection : MonoBehaviour
{
    public Transform sunlight;
    public Vector3 rayStartOffset;
    public Transform lightObjects;
    public bool isLit = true;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(sunlight.forward);
    }

    // Update is called once per frame
    void FixedUpdate() {
        Ray sunlightRay = new Ray(transform.position + rayStartOffset, -sunlight.forward * 100f);

        Debug.DrawRay(transform.position + rayStartOffset, -sunlight.forward * 100f, Color.green);
        // RaycastHit terrainDetection = new RaycastHit();
        // Terrain.activeTerrain.GetComponent<Collider>().Raycast(sunlightRay,
        //     out terrainDetection, 100f);
        // Debug.Log(terrainDetection.point);
        if (Physics.Raycast(transform.position + rayStartOffset, -sunlight.forward * 100f, 100f, ~(1 << 10))
            ) {
            isLit = false;
        } else {
            isLit = true;
        }
        if (!isLit) {
            for (int i = 0; i < lightObjects.childCount && !isLit; i++) {
                if (lightObjects.GetChild(i).gameObject.activeInHierarchy &&
                    lightObjects.GetChild(i).GetComponent<LightObject>().isActive &&
                    (lightObjects.GetChild(i).transform.position - transform.position).magnitude <
                    lightObjects.GetChild(i).GetComponent<LightObject>().lightRadius) {
                    isLit = true;
                    // Debug.Log(lightObjects.GetChild(i).name);
                }
            }
        }
    }
}
