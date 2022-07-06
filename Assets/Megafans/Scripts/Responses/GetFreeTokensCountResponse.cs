using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class GetFreeTokensCountResponse : Response {
		
		public FreeTokens data;

	}

	[System.Serializable]
	public class FreeTokens {
		
		public float tokens;

	}

}