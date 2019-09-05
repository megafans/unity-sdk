using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	public class MegafansJsonHelper {

		public static T FromJson<T>(string json) {
			T obj = JsonConvert.DeserializeObject<T> (json);

			return obj;
		}

		public static string ToJson<T>(T obj) {
			string json = JsonConvert.SerializeObject (obj);

			return json;
		}

	}

}