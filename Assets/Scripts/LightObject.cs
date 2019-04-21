using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : MonoBehaviour
{
    public bool canBeToggled = false;
    public bool isActive = true;
    public float lightRadius;
    public float cullDistance = 0f;

    Transform player;

    void OnDrawGizmosSelected() {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1f, 1f, 0.4f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, lightRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cullDistance > 0f) {
            player = GameObject.Find("Player").transform;
        }
    }

    // Update is called once per frame
    void Update() {
        if (cullDistance > 0f) {
            transform.GetChild(0).GetComponent<Light>().enabled = (transform.position - player.position).magnitude < cullDistance;
        }
    }
}
