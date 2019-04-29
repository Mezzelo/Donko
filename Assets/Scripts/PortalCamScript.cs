using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamScript : MonoBehaviour
{
    public Transform mainCam;
    public Transform portalSprite;
    Vector3 mainCamRot;
    Vector3 origCamRot;

    // Start is called before the first frame update
    void Start() {
        mainCamRot = mainCam.rotation.eulerAngles;
        origCamRot = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update() {
        transform.rotation = Quaternion.Euler(origCamRot + mainCam.rotation.eulerAngles - mainCamRot);
        portalSprite.LookAt(transform.position);
    }
}
