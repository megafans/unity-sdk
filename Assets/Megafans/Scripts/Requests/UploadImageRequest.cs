//using UnityEngine;
//using System.IO;
//using System;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using UnityEngine.Networking;

//namespace MegafansSDK.Utils
//{
//    [System.Serializable]
//    public class UploadImageRequest : Request
//    {
//        public string image_file;

//        public override WWW GetWWW(string url) {       
//            //Dictionary<string, string> body = new Dictionary<string, string>();
//            //body.Add("image", image);
//            //string jsonString = JsonConvert.SerializeObject(body);

//            Dictionary<string, string> headers = new Dictionary<string, string>();
//            string authorization = MegafansWebService.GetBearerToken();
//            headers.Add("Authorization", authorization);
//            headers.Add("Content-Type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");

//            WWW localFile = new WWW("file://" + image_file);

//            //Debug.Log(localFile);
//            //if (localFile.error == null)
//            //{
//            //    Debug.Log("No error getting local file");
//            //}
//            //else
//            //{
//            //    Debug.Log("ERROR getting local file");
//            //    Debug.Log(localFile.error);
//            //    //yield break;
//            //}
//            //WWWForm myForm = new WWWForm();

//            //FileInfo file = new FileInfo(image_file);

//            //string fileExtension = file.Extension;
//            //string[] tempArray = fileExtension.Split("."[0]);
//            //fileExtension = tempArray[1];
//            //string result = Convert.ToBase64String(localFile.bytes);
//            //myForm.AddField("type", fileExtension);
//            //myForm.AddField("image", result);

//            //Debug.Log(result);

//            //WWW www = new WWW(url, myForm.data, headers);

//            //return www;
//            byte[] myData = System.Text.Encoding.UTF8.GetBytes("This is some test data");
//            UnityWebRequest www = UnityWebRequest.Put(url, myData);
//            www.SetRequestHeader("Authorization", authorization);
//            yield return www.SendWebRequest();
//        }
//    }
//}