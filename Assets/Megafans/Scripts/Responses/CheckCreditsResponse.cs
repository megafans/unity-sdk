using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class CheckCreditsResponse : Response {
		
		public CheckCreditsData data;

	}

	[System.Serializable]
	public class CheckCreditsData {
		
		public float credits;

	}

}