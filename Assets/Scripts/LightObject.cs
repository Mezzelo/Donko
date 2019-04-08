using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : MonoBehaviour
{
    public bool canBeToggled = false;
    public bool isActive = true;
    public float lightRadius;

    void OnDrawGizmosSelected() {
        // Display the explosion radius when selected
        Gizmos.color = new Color(1f, 1f, 0.4f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, lightRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
