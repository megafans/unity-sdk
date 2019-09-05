namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class RegisterFirstTimeResponse : Response
    {

        public RegisterFirstTimeResponseData data;

    }

    [System.Serializable]
    public class RegisterFirstTimeResponseData
    {
        public string username;
        public string token;
        public string refresh;
        public int userId;
        public bool sms;
    }

}