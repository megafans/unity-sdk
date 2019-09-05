using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public abstract class RegisterRequest : Request {

		public string device_token;
		public string email;
		public string password;
        public string device_type;
		public string username;
		public string facebook_login_id;
		public string registerType;
		public string phone_number;
		public string image;
        public string appGameUid;
    }

}
