using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK;
using MegafansSDK.AdsManagerAPI;
using MegafansSDK.Utils;

public class Game : MonoBehaviour, ILandingOptionsListener, IJoinGameCallback {

	[SerializeField] private GameObject gameCanvas;
	[SerializeField] private Text msgTxt;
	[SerializeField] private GameObject m_MegaFans;
	[SerializeField] private GameObject m_MegaFansHelper;
	[SerializeField] private GameObject m_canvasRefresh;

	private int minNumber = 1;
	private int maxNumber = 1000;
	private string gameDescription = "";
	private bool isNormalGame = true;

	private int score = 0;
	private GameType gameType;

	void Awake() {
		if (MegafansHelper.m_Instance == null)
        {
			Instantiate(m_MegaFansHelper);
		}			

		if (MegafansSDK.Megafans.Instance == null)
		{
			GameObject go = (GameObject)Instantiate(m_MegaFans);
			GameObject AC = (GameObject)Instantiate(m_canvasRefresh);
			Refresh obj = AC.GetComponent<Refresh>();

			AdsManager adsManager = go.GetComponent<AdsManager>();
			adsManager.adImage1 = obj.adImage1;
			adsManager.adVideo1 = obj.adVideo1;
			adsManager.m_uniGifImage = obj.m_uniGifImage;
			adsManager.canvasObj = obj.canvasObj;
			adsManager.crossButtonState = obj.crossButtonState;
			adsManager.background = obj.background;
			adsManager.m_uniGifVideo = obj.m_uniGifImage;
			adsManager.advideo = obj.advideo;
			adsManager.adImage = obj.adImage;
			adsManager.redirectionButtonBanner = obj.redirectionButtonBanner;
		}
	}


	public void RollBtn_OnClick() {
		score = Random.Range (minNumber, maxNumber + 1);
		msgTxt.text = "You scored: " + score;
        string messageText = "You scored: " + score;

        if (!isNormalGame) {
            Megafans.Instance.SaveScore (score, messageText, gameType, () => {
				Debug.Log ("Score saved successfully.");
			}, (string error) => {
				Debug.LogError (error);
			});
		}
	}

	public void MegafansBtn_OnClick() {
		if (!Megafans.Instance.IsUserLoggedIn) {
			Megafans.Instance.ShowLandingScreen (this, this);
		}
		else {
			Megafans.Instance.ShowTournamentLobby (this, this);
		}
	}

	public void OnPlayGameClicked() {
		isNormalGame = true;
		StartGame ();
	}

	public void OnUserLoggedIn(string userId) { 
        Megafans.Instance.ShowTournamentLobby (this, this);
	}

    public void OnUserRegistered(string userId) {
		Debug.Log ("User registered");
	}

	public void StartGame(GameType gameType, Dictionary<string, string> metaData) {	
		this.gameType = gameType;

		isNormalGame = false;
		StartGame ();
	}

	public void PurchaseTokens(int numberOfTokens) {
		//Megafans.Instance.SaveTokens ("", numberOfTokens, () => {
		//	Debug.Log("Tokens saved successfully.");
		//},
			//(string error) => {
			//	Debug.LogError(error);
			//});
	}

	private void StartGame() {
		msgTxt.text = gameDescription;
		gameCanvas.SetActive (true);
	}

}