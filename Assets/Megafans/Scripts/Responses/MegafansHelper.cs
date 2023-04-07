using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using MegafansSDK;
using MegafansSDK.UI;
using MegafansSDK.Utils;
using MegafansSDK.AdsManagerAPI;
using OneSignalSDK;


public class MegafansHelper : MonoBehaviour, ILandingOptionsListener, IJoinGameCallback
{
    public static MegafansHelper m_Instance;

    internal static bool m_HasUsedMultiplier = false;
    internal static bool m_ShoudlOpenMegFans = false;

    internal bool m_WasPlayingTournament = false;


    private string levelId = "";
    private int matchId = 0;
    private GameType gameType;

    private int bestScore = 0;
    public int BestScore
    {
        get
        {
            return bestScore;
        }

        set
        {
            bestScore = value;
        }
    }

    public GameType GameType
    {
        set
        {
            gameType = value;
        }

        get
        {
            return gameType;
        }
    }

    public bool isUIEnabled
    {
        get
        {
            return MegafansUI.Instance.isUIenabled;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        m_Instance = this;
    }

    #region ILandingOptionsListener implementation

    public void OnPlayGameClicked()
    {

    }

    public void OnUserLoggedIn(string userId)
    {
        Megafans.Instance.ShowTournamentLobby(this, this);

        var status = OneSignal.Default.NotificationPermission;
        if (status == NotificationPermission.NotDetermined)
        {
            PromptForPushNotificationsWithUserResponse();
        }

        OneSignal.Default.SetExternalUserId(userId);


        Megafan.NativeWrapper.MegafanNativeWrapper.RegisterUserWithUserId(userId, Megafans.Instance.GameUID, Application.productName);
    }
    async void PromptForPushNotificationsWithUserResponse()
    {

       var response = await OneSignal.Default.PromptForPushNotificationsWithUserResponse();

    }

    public void OnUserRegistered(string userId)
    {
      var status = OneSignal.Default.NotificationPermission;
        if (status == NotificationPermission.NotDetermined)
        {
            PromptForPushNotificationsWithUserResponse();
        }

        OneSignal.Default.SetExternalUserId(userId);

        Megafan.NativeWrapper.MegafanNativeWrapper.RegisterUserWithUserId(userId,
                                                                  Megafans.Instance.GameUID,
                                                                  Application.productName);
    }
    #endregion

    #region IJoinGameCallback implementation
    public void StartGame(GameType _gameType, Dictionary<string, string> metaData)
    {
        gameType = _gameType;

        BestScore = 0;
        int currentLevel;

        if (metaData.ContainsKey("level") && int.TryParse(metaData["level"], out currentLevel))
        {
            PlayerPrefs.SetInt("OpenLevel", currentLevel);
            PlayerPrefs.Save();

            GameObject.Find("CanvasGlobal").transform.Find("MenuOfferBooster").gameObject.SetActive(true);
        }
    }

    public void PurchaseTokens(int numberOfTokens)
    {
        Debug.Log("Did purchase tokens");
    }
    #endregion

    public void ShowMegafans()
    {
        Megafans.Instance.ShowMegafans(this, this);
    }

    public void SaveUserScore(int withScore, string metaData)
    {
        Megafans.Instance.SaveScore(withScore, metaData, this.gameType,
            () =>
            {
                Debug.Log("User score saved");
            }, (error) =>
            {
                Debug.Log("Error saving user score");
            });
    }

    public void ShowLandingScreen()
    {
        Megafans.Instance.ShowLandingScreen(this, this);
    }

    private void OneSignal_promptForPushNotificationsReponse(bool accepted)
    {
        Debug.Log("OneSignal_promptForPushNotificationsReponse: " + accepted);
    }

}
