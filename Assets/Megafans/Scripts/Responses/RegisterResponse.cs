using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class RegisterResponse : Response {
		
		public RegisterResponseData data;

	}

	[System.Serializable]
	public class RegisterResponseData {
		
		public int id;
		public string username;
		public string password;
		public string email;
		public string device_token;
		public string device_type;
		public int status_id;
		public string updated_at;
		public string created_at;

	}

}