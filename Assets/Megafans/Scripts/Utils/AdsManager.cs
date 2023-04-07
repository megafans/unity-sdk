using MegafansSDK.UI;
using MegafansSDK.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MegafansSDK.AdsManagerAPI
{

    public class AdsManager : MonoBehaviour
    {
        //[SerializeField]
        public UniGifImage m_uniGifImage, m_uniGifVideo;
        [SerializeField] string ironSrcIdIOS;
        [SerializeField] string ironSrcIdAndroid;
        public RawImage adImage, advideo;
        public GameObject crossButtonState, background;
        public Image adImage1, adVideo1;
        public GameObject canvasObj;
        public Button redirectionButtonBanner;

        private Sprite targetSprite;


        string IronSrcId => Application.platform == RuntimePlatform.IPhonePlayer ? ironSrcIdIOS : ironSrcIdAndroid;
        Action onReward;
        string adUrl;
        private bool m_mutex;
        internal int m_MaxMultiplier = 4;
        internal int m_MinMultiplier = 2;
        internal int m_VideRewardMultiplier = 2;
        internal bool m_HasBannerAds = false;
        internal int m_TournamentPlayCount = 0;
        internal BaseTenjin m_TenginInstance;

        public static AdsManager instance
        {
            get;
            private set;
        }


        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this);
            }

            m_TenginInstance = Tenjin.getInstance("XAWZ2XFRSFXVFEASMWQK8G2XPHC638VV");
            m_TenginInstance.Connect();

            IronSource.Agent.init(IronSrcId);
            IronSource.Agent.validateIntegration();
            IronSource.Agent.loadInterstitial();
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);

            //Events
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent += IronSourceEvents_onBannerAdLoadFailedEvent;

            IronSourceEvents.onInterstitialAdOpenedEvent += () =>
            {
                MusicBase.Instance.ToggleMusic(false);
            };
            IronSourceEvents.onInterstitialAdClosedEvent += () =>
            {
                MusicBase.Instance.ToggleMusic(true);
                IronSource.Agent.loadInterstitial();
            };

            IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;

            IronSourceEvents.onRewardedVideoAdOpenedEvent += () => { MusicBase.Instance.ToggleMusic(false); };
            IronSourceEvents.onRewardedVideoAdClosedEvent += () => { MusicBase.Instance.ToggleMusic(true); };
            IronSourceEvents.onRewardedVideoAdRewardedEvent += placement => { onReward?.Invoke(); onReward = null; };


        }

        public void initIronSourceWithUserId(string userId)
        {         
            if (IronSource.Agent != null)
            {
                IronSource.Agent.setUserId(userId);
                IronSource.Agent.init(IronSrcId, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);
                IronSource.Agent.validateIntegration();
                IronSource.Agent.loadInterstitial();
                IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);

                //Events
                IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
                IronSourceEvents.onBannerAdLoadFailedEvent += IronSourceEvents_onBannerAdLoadFailedEvent;

                IronSourceEvents.onInterstitialAdOpenedEvent += () =>
                {
                    MusicBase.Instance.ToggleMusic(false);
                };
                IronSourceEvents.onInterstitialAdClosedEvent += () =>
                {
                    MusicBase.Instance.ToggleMusic(true);
                    IronSource.Agent.loadInterstitial();
                };

                IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;
                IronSourceEvents.onOfferwallShowFailedEvent += OfferwallShowFailedEvent;
                IronSourceEvents.onOfferwallAvailableEvent += OfferwallAvailableEvent;
                IronSourceEvents.onRewardedVideoAdRewardedEvent += placement => { onReward?.Invoke(); onReward = null; };

                IronSource.Agent.isOfferwallAvailable();
                IronSource.Agent.isRewardedVideoAvailable();
            }
        }


        private void Start()
        {
            m_TenginInstance.SendEvent("open app");
        }

        #region OfferWall
        void OfferwallClosedEvent()
        {
            if (MegafansSDK.UI.MegafansUI.Instance.isUIenabled)
                MegafansWebService.Instance.GetCredits(MegafansPrefs.UserId, OnGetCreditsSuccess,OnGetCreditsFailure);
        }

        void OfferwallShowFailedEvent(IronSourceError error)
        {
            Debug.Log("===========================");
            Debug.Log(error.ToString());
            Debug.Log("===========================");
        }

        void OfferwallAvailableEvent(bool canShowOfferwall)
        {
            Debug.Log("===========================");
            Debug.Log("Offer wall available: " + canShowOfferwall);
            Debug.Log("===========================");
        }

        private void OnGetCreditsFailure(string obj)
        {
            Debug.Log("Getting credits FAILED");
        }

        private void OnGetCreditsSuccess(CheckCreditsResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.CurrentTokenBalance = response.data.credits;

                MegafansUI.Instance.UpdateAllTokenTexts();
            }
        }

        #endregion

        private void IronSourceEvents_onBannerAdLoadFailedEvent(IronSourceError obj)
        {
            /*if (LevelManager.Instance != null)
            {
                LevelManager.Instance.UpdateUIToFitBanner(false);
            }*/
        }
#if UNITY_EDITOR
        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.U))
                LevelManager.Instance.UpdateUIToFitBanner(true);

            if (Input.GetKeyDown(KeyCode.I))
                LevelManager.Instance.UpdateUIToFitBanner(false);*/
        }
#endif

        void BannerAdLoadedEvent()
        {
            Debug.Log("Ashish: BannerAdLoadedEvent" + MegafansSDK.UI.MegafansUI.Instance.isUIenabled);
            //update UI here
            //DisplayBanner(!MegafansSDK.UI.MegafansUI.Instance.isUIenabled);

            m_HasBannerAds = true;

            /*if (LevelManager.Instance != null)
            {
                LevelManager.Instance.UpdateUIToFitBanner(true);
            }*/
        }

        public void DisplayBanner(bool _Show)
        {
            Debug.Log(" DisplayBanner ");
            if (_Show)
            {
                Debug.Log(" DisplayBannerAds1");
                MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_Banner(needtoShowThirdPartyAds => {
                    Debug.Log("DisplayBannerAds2 " + needtoShowThirdPartyAds);
                    if (needtoShowThirdPartyAds)
                    {
                        adImage.enabled = false;
                        adImage1.enabled = false;
                        redirectionButtonBanner.interactable = false;
                        Debug.Log(" DisplayBannerAds3 " + needtoShowThirdPartyAds);
                        IronSource.Agent.displayBanner();
                    }


                });
            }
            else
                IronSource.Agent.hideBanner();
        }

        public void ShowInterstitial()
        {
            Debug.LogWarning("Showing Interstitial");

            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial();
            }
            else
                IronSource.Agent.loadInterstitial();
        }


        public void ShowRewardedVideo(Action action, bool _shouldAddID)
        {
            Dictionary<string, string> owParams = new Dictionary<string, string>();

            owParams.Add("userId", _shouldAddID ? "R_" + MegafansPrefs.UserId.ToString() : "");

            IronSource.Agent.setRewardedVideoServerParams(owParams);

            onReward = action;
            IronSource.Agent.showRewardedVideo();
        }

        public bool IsRewardedVideoAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        public void ShowOfferwall()
        {
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                MegafansUI.Instance.freeTokensUI.ShowFreeTokensPanel(true);
            }
            else
            {
                OpenIronOfferWall();
            }
        }

        internal void OpenIronOfferWall()
        {
            if (IronSource.Agent.isOfferwallAvailable())
            {
                Dictionary<string, string> owParams = new Dictionary<string, string>();
                owParams.Add("userId", "O_" + MegafansPrefs.UserId.ToString());
                IronSourceConfig.Instance.setOfferwallCustomParams(owParams);

                IronSource.Agent.showOfferwall();
            }
        }
        string baseUrl = "https://megafansapi.azurewebsites.net/";
        internal string freeTokensURL;

        IEnumerator GetRequest(string uri, int value, Action<bool> isShowingAction)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {

                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.Log(pages[page] + ": Error: " + webRequest.error);
                }
                else
                {
                    JSONNode tokenParams = JSON.Parse(webRequest.downloadHandler.text);
                    string success = tokenParams["success"];
                    string message = tokenParams["message"];
                    bool ironsourceBool = tokenParams["data"]["ironsource"];
                    string size = tokenParams["data"]["size"];
                    if (value == 0)
                        Refresh.Instance.bannercode = tokenParams["data"]["code"];
                    else
                        Refresh.Instance.interstialcode = tokenParams["data"]["code"];
                    string adUrl = tokenParams["data"]["imageUrl"];
                    Debug.Log(adUrl + " adurl is");
                    if (String.IsNullOrEmpty(adUrl))
                    {
                        Debug.Log(String.IsNullOrEmpty(adUrl) + "string is empty");
                        isShowingAction(true);
                        yield break;
                    }
                    else
                        isShowingAction(false);
                    if (adUrl.Contains(".gif"))
                    {
                        Debug.Log("i am gif");
                        if ((ironsourceBool == false && Refresh.Instance.adUrl != null) && value == 0)
                        {
                            Refresh.Instance.adUrl = adUrl;
                            Debug.Log("Hi, i am Banner Ad");
                            m_mutex = true;
                            IronSource.Agent.hideBanner();
                            StartCoroutine(ViewGifCoroutine());
                            redirectionButtonBanner.interactable = true;
                            adImage.enabled = true;
                            adImage1.enabled = false; 
                            /*if (LevelManager.Instance != null)
                            {
                                LevelManager.Instance.UpdateUIToFitBanner(true);
                            }*/
                            Refresh.Instance.redirectionButton.interactable = true;
                        }
                        else if ((ironsourceBool == false && Refresh.Instance.adUrl != null) && value == 1)
                        {
                            canvasObj.SetActive(true);
                            adImage.enabled = false;
                            adImage1.enabled = false;
                            IronSource.Agent.hideBanner();
                            Refresh.Instance.adUrl = adUrl;
                            Debug.Log("ironsource   " + ironsourceBool);
                            m_mutex = true;
                            Debug.Log("Hi, I am interstial ad.");
                            StartCoroutine(ViewGifCoroutineForVideo());
                            advideo.enabled = true;
                            adVideo1.enabled = false;
                            background.SetActive(true);
                            Refresh.Instance.redirectionButton.interactable = true;
                            Invoke("CrossButton", 10.0f);

                        }
                    }
                    else if (adUrl.Contains(".png") || adUrl.Contains(".jpg"))
                    {
                        if ((ironsourceBool == false && Refresh.Instance.adUrl != null) && value == 0)
                        {
                            StartCoroutine(GetTextureRequest(adUrl, (response) => {
                                adImage1.sprite = response;
                                adImage1.enabled = true;
                                redirectionButtonBanner.interactable = true;
                                Refresh.Instance.redirectionButton.interactable = true;
                                /*if (LevelManager.Instance != null)
                                {
                                    LevelManager.Instance.UpdateUIToFitBanner(true);
                                }*/
                            }));
                        }
                        else if ((ironsourceBool == false && Refresh.Instance.adUrl != null) && value == 1)
                        {
                            adImage.enabled = false;
                            adImage1.enabled = false;
                            StartCoroutine(GetTextureRequest(adUrl, (response) => {
                                IronSource.Agent.hideBanner();
                                canvasObj.SetActive(true);
                                adVideo1.sprite = response;
                                adVideo1.enabled = true;
                                advideo.enabled = false;
                                background.SetActive(true);
                                Refresh.Instance.redirectionButton.interactable = true;
                                Invoke("CrossButton", 10.0f);
                            }));
                        }
                    }

                }


            }
        }
        IEnumerator GetTextureRequest(string url, System.Action<Sprite> callback)
        {
            using (var www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    if (www.isDone)
                    {
                        var texture = DownloadHandlerTexture.GetContent(www);
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);

                        callback(sprite);
                    }
                }
            }
        }
        public void CrossButton()
        {

            crossButtonState.SetActive(true);
        }
        private IEnumerator ViewGifCoroutineForVideo()
        {
            if (m_uniGifVideo == null)
            {
                Debug.Log("Null value..111.");
            }
            yield return StartCoroutine(m_uniGifVideo.SetGifFromUrlCoroutine(Refresh.Instance.adUrl));
            m_mutex = false;
        }
        private IEnumerator ViewGifCoroutine()
        {
            Debug.Log(Refresh.Instance.adUrl);
            if (m_uniGifImage == null)
            {
                Debug.Log("Null value...");
            }
            yield return StartCoroutine(m_uniGifImage.SetGifFromUrlCoroutine(Refresh.Instance.adUrl));
            m_mutex = false;
        }
        public void ApiCall_Banner(Action<bool> isShowing)
        {
            adImage.enabled = false;
            IronSource.Agent.hideBanner();
            StartCoroutine(GetRequest(baseUrl + "direct_banner?appGameUid=" + Megafans.Instance.GameUID, 0, isShowing));
        }

        public void ApiCall_FullScreen(Action<bool> isShowing)
        {
            StartCoroutine(GetRequest(baseUrl + "direct_fullscreen?appGameUid=" + Megafans.Instance.GameUID, 1, isShowing));
        }
    }
}




