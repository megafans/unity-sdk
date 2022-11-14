using System;
using UnityEngine;
using UnityEngine.UI;

namespace MegafansSDK.Utils
{
    public class ImagePicker : MonoBehaviour
    {
        public void PickImage(Action<Texture2D, String> callback, int maxSize, string title)
        {
            Debug.Log("working here 0.1");
            NativeGallery.GetImageFromGallery((string path) =>
            {
                if (path != null)
                {
                    Debug.Log("working here 1");
                    Texture2D tex = NativeGallery.LoadImageAtPath(path, maxSize, false, false, false);
                    callback(tex, path);
                    Debug.Log("working here 2");
                }

            }, title, "image/*");
        }

    }

}