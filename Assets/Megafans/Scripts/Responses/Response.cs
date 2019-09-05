
namespace MegafansSDK.Utils {

	[System.Serializable]
	public class Response {

		public string success;
		public string message;

		public string ToJson() {
			return MegafansJsonHelper.ToJson<Response> (this);
		}

	}

}