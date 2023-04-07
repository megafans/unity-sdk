using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class LoginResponse : Response {

		public LoginResponseData data;

	}

	[System.Serializable]
	public class LoginResponseData {

		public int id;
		public string username;
        public string token;
		public int userId;
        public string refresh;
        public string email;
		public string password;
		public string phone_number;
		public string device_type;
		public string device_token;
		public object gender;
		public object date_of_birth;
		public int client_balance;
		public object facebook_login_id;
		public string image;
		public object secret_token;
		public bool phone_verified;
		public object role_id;
		public int status_id;
		public string created_at;
		public string updated_at;

	}

}