using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class SafeArea : MonoBehaviour
{
    private void OnEnable()
    {
        var rt = GetComponent<RectTransform>();
        UpdateSafeArea(rt, Screen.safeArea);
    }

    private void OnDisable()
    {

    }

    private void UpdateSafeArea(RectTransform rt, Rect safeArea)
    {
        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
    }
}
