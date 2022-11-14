using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaFilter : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log(SystemInfo.deviceModel + " Sohaib");
        if (string.Compare(SystemInfo.deviceModel, "iPhone15") == 1)
        {
            Debug.Log("working with iphone");
            var RectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= (Screen.height * 1.05f);
            anchorMax.x /= Screen.width;
            anchorMax.y /= (Screen.height * 1.05f);

            RectTransform.anchorMin = anchorMin;
            RectTransform.anchorMax = anchorMax;
        }/*
        else if (Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            var RectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            RectTransform.anchorMin = anchorMin;
            RectTransform.anchorMax = anchorMax;
        }*/
    }
}
