using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MezzMath {
    public static float halfSine(float input) {
        return Mathf.Clamp(Mathf.Sin(Mathf.Min(Mathf.Max(input * Mathf.PI / 2f, 0f), Mathf.PI / 2f)), 0f, 1f);
    }

    public static float fullSine(float input) {
        return (Mathf.Sin(Mathf.Min(Mathf.Max(input * Mathf.PI - Mathf.PI / 2f, -Mathf.PI / 2f), Mathf.PI / 2f)) + 1f) / 2f;
    }

    public static float revSine(float input) {
        return Mathf.Clamp(1f - Mathf.Sin(Mathf.Min(Mathf.Max(Mathf.PI / 2f - input * Mathf.PI / 2f, 0f), Mathf.PI / 2f)), 0f, 1f);
    }
}
