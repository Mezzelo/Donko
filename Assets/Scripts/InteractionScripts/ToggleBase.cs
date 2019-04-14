using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBase : MonoBehaviour
{
    protected bool isActivated = true;

    public virtual void toggleActivation(int mode) {
        isActivated = !isActivated;
        if (gameObject.GetComponent<LightObject>() != null)
            gameObject.GetComponent<LightObject>().isActive = isActivated;
    }
    
}
