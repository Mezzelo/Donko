using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float turnSpeed = 4.0f;
    public Transform player;

    private Vector3 offsetX;
    private Vector3 offsetY;
    public Vector3 StartPos;

    public float height = 4f;
    public float distance = 8f;

    public float maxY = 5f;
    public float minY = -5f;
    float currY = 0f;

    public float zoomDistance;
    public float zoomDistanceMax = 16f;
    public float zoomDistanceMin = 4f;
    public float scrollSpeed = 0.75f;


    // Use this for initialization
    void Start() {
        offsetX = new Vector3(0, 0, distance);
        offsetY = new Vector3(0, 0, distance);
        StartPos = offsetX + offsetY;
    }

    // Update is called once per frame
    void LateUpdate() {
        // Debug.Log(currY);
        if (Time.timeScale > 0f) {
            float offY = Input.GetAxis("Mouse Y");
            if (currY > minY && currY + Input.GetAxis("Mouse Y") < minY) {
                offY = minY - currY;
                currY = minY;
            }
            else if (currY < maxY && currY + Input.GetAxis("Mouse Y") > maxY) {
                offY = maxY - currY;
                currY = maxY;
            }
            else if (currY + Input.GetAxis("Mouse Y") < maxY && currY + Input.GetAxis("Mouse Y") > minY) {
                currY = currY + offY;
            }
            else {
                offY = 0f;
            }
            offsetX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offsetX;
            offsetY = Quaternion.AngleAxis(offY * turnSpeed, Vector3.right) * offsetY;
        }
        transform.position = player.position - new Vector3(0, 0, distance) + offsetX + offsetY;
        // CameraClose();
        transform.LookAt(player.position);
    }

    void CameraClose() {
        RaycastHit ray;
        Debug.DrawRay(transform.position, -transform.forward, Color.red, .001f);
        Debug.DrawRay(transform.position, transform.right, Color.red, .001f);
        Debug.DrawRay(transform.position, -transform.right, Color.red, .001f);
        if ((Physics.Raycast(transform.position, -transform.forward, out ray, 10f, 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) ||
               (Physics.Raycast(transform.position, transform.right, out ray, 10f, 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) ||
               (Physics.Raycast(transform.position, -transform.right, out ray, 10f, 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore))) {
            // Debug.Log("rayHist");
            if (ray.distance < 0.5f) {
                transform.localPosition += new Vector3(0, 0, .1f);
            }
            else if (transform.localPosition.z > StartPos.z && ray.distance > .8f) {
                transform.localPosition -= new Vector3(0, 0, .05f);
            }

            if (Physics.Raycast(transform.position, transform.up, out ray, 10f, 3, queryTriggerInteraction: QueryTriggerInteraction.Ignore)) {
                if (ray.distance < 0.65f) {
                    transform.localPosition -= new Vector3(0, .005f, 0);
                }
                else if (transform.localPosition.y < StartPos.y && ray.distance > .65f) {
                    transform.localPosition += new Vector3(0, .005f, 0);
                }
            }
        }
    }

    private void Update() {
        // zoomDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        // zoomDistance = Mathf.Clamp(distance, zoomDistanceMin, zoomDistanceMax);
        // transform.localPosition += new Vector3(0, 0, distance);
    }
}