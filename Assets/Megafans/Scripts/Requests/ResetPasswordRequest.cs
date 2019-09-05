using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class ResetPasswordRequest : Request {
		
		public int device_type;
		public string email;

		public override WWW GetWWW(string url) {			
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("email", email);
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Accept", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW(url, pData, headers);

            return www;
        }

	}

}
