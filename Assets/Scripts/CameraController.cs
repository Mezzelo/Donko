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

    float screenShake = 0f;

    Vector3 normalPosition;

    public void addShake(float amount) {
        screenShake += amount;
    }

    // Use this for initialization
    void Start() {
        offsetX = new Vector3(0, 0, distance);
        offsetY = new Vector3(0, 0, distance);
        StartPos = offsetX + offsetY;
    }

    // Update is called once per frame
    void LateUpdate() {
        if (screenShake > 0f)
            screenShake = Mathf.Max(0f, screenShake - Time.deltaTime * 2.5f);

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
        normalPosition = player.position - new Vector3(0, 0, distance) + offsetX + offsetY;
        transform.position = normalPosition;
        CameraClose();
        transform.LookAt(player.position);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(
            Random.Range(-screenShake, screenShake) * 1.5f, 
            Random.Range(-screenShake, screenShake) * 1.5f, 
            Random.Range(-screenShake, screenShake) * 1.5f));
    }

    void CameraClose() {
        Debug.DrawRay(player.position, (transform.position - player.position), Color.green);
        Ray cameraRay = new Ray(player.position, (transform.position - player.position));
        RaycastHit ray;
        if (Physics.Raycast(cameraRay, out ray, (transform.position - player.position).magnitude, ~(1 << 12))) {
        if (ray.distance < (transform.position - player.position).magnitude) {
                transform.position = player.position + (transform.position - player.position).normalized * ray.distance * 0.8f;
            }
        }
    }

    private void Update() {
        // zoomDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        // zoomDistance = Mathf.Clamp(distance, zoomDistanceMin, zoomDistanceMax);
        // transform.localPosition += new Vector3(0, 0, distance);
    }
}