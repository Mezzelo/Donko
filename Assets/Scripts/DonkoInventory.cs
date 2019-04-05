using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DonkoInventory : MonoBehaviour
{
    public int[] flickers;
    public Transform canvas;

    public void changeFlickerCount(int index, int increment) {
        flickers[index] += increment;
        for (int i = 0; i < 3; i++) {
            canvas.Find("FlickerC" + (i + 1)).GetComponent<Text>().text =
                (i == 0 ? ("Blue") : (i == 1 ? "Red?" : "White?")) + " Flickers: " + flickers[i];
        }
    }
}
