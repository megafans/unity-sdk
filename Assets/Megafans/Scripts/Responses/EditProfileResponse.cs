using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class EditProfileResponse : Response {
		
		public EditProfileData data;

	}

	[System.Serializable]
	public class EditProfileData {
		
		public int id;
		public string username;
		public string email;
		public string password;
		public string phone_number;
		public string device_type;
		public string device_token;
		public string gender;
		public string date_of_birth;
		public int client_balance;
		public string facebook_login_id;
		public string image;
		public string secret_token;
		public bool phone_verified;
		public string role_id;
		public int status_id;
		public string created_at;
		public string updated_at;

	}

}