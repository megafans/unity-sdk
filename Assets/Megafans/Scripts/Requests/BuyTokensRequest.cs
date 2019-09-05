using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using Newtonsoft.Json;


namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class AppleReceiptData
    {
        public string Store;
        public string TransactionID;
        public string Payload;
    }

    
    [System.Serializable]
    public class GoogleReceiptData
    {
        public string json;
        public string signature;
    }

    [System.Serializable]
    public class BuyTokensRequest : Request
    {

        public string app_game_uid;
        public string tokens;
        public string receipt;
        public string transaction_number;
        public string productId;
        public string deviceType;

        public override WWW GetWWW(string url)
        {
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("app_game_uid", app_game_uid);
            body.Add("tokens", tokens);
            body.Add("transaction_id", transaction_number);
            body.Add("product_id", productId);
            string appID = Application.identifier;
#if UNITY_IPHONE
            AppleReceiptData deSerializedReceipt = MegafansJsonHelper.FromJson<AppleReceiptData>(receipt);
            body.Add("receipt", deSerializedReceipt.Payload);
            body.Add("device_type", "iOS");
#endif
#if UNITY_ANDROID
            //GoogleReceiptData deSerializedReceipt = MegafansJsonHelper.FromJson<GoogleReceiptData>(receipt);
            byte[] bytes = Encoding.ASCII.GetBytes(receipt);
            string converted = Convert.ToBase64String(bytes);
            body.Add("receipt", converted);
            body.Add("device_type", "Android");
            body.Add("package_name", appID);
#endif
            string jsonString = JsonConvert.SerializeObject(body);
            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);
            Debug.Log("Buying Tokens");
            Debug.Log(jsonString);
            WWW www = new WWW(url, pData, headers);

            return www;
        }

    }

}