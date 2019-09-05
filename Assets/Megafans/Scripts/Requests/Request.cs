using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public abstract class Request {

		public abstract WWW GetWWW (string url);

		public string ToJson() {
			return MegafansJsonHelper.ToJson<Request> (this);
		}

	}

}