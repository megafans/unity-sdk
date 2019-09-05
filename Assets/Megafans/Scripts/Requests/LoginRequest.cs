using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public abstract class LoginRequest : Request {

		public string device_token;
		public string email;
		public string password;
		public int device_type;
		public string username;
		public string facebook_login_id;
		public string phone_number;
		public string registerType;
        public string appGameUid;

    }

}
