using System.Collections.Generic;

namespace MegafansSDK.Utils
{
    [System.Serializable]
    public class TermsOrPrivacyPolicyResponse: Response {
        public TermsOrPrivacyPolicyResponseData data;
    }

    [System.Serializable]
    public class TermsOrPrivacyPolicyResponseData
    {
        public string description;
    }
}