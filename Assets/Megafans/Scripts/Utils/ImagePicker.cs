using System;
using UnityEngine;
using UnityEngine.UI;

namespace MegafansSDK.Utils
{
    public class ImagePicker : MonoBehaviour
    {
        public void PickImage(Action<Texture2D, String> callback, int maxSize, string title)
        {

            NativeGallery.GetImageFromGallery((string path) =>
            {
                if (path != null)
                {
                    Texture2D tex = NativeGallery.LoadImageAtPath(path, maxSize, false, false, false);
                    callback(tex, path);
  
                }

            }, title, "image/*");
        }

    }

}