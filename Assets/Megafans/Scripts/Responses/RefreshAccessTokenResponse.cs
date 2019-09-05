
namespace MegafansSDK.Utils {
    [System.Serializable]
    public class RefreshAccessTokenResponse : Response {
        public RefreshAccessTokenResponseData data;
    }

    [System.Serializable]
    public class RefreshAccessTokenResponseData {
        public string token;
        public string refresh;
    }
}