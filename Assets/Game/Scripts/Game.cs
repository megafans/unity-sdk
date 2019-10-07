using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK;
using MegafansSDK.Utils;

public class Game : MonoBehaviour, ILandingOptionsListener, IJoinGameCallback {

	[SerializeField] private GameObject gameCanvas;
	[SerializeField] private Text msgTxt;

	private int minNumber = 1;
	private int maxNumber = 1000;
	private string gameDescription = "";
	private bool isNormalGame = true;

	private int score = 0;
	private GameType gameType;

	void Awake() {
		gameDescription = "Press the button to roll a number from " + minNumber + " to " +
		maxNumber + ". Player with the highest number wins!";
	}

	void Start() {
		if (!Megafans.Instance.IsUserLoggedIn) {
			Megafans.Instance.ShowLandingScreen (this, this);
		}
		else {
            Megafans.Instance.ShowTournamentLobby (this, this);
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

	public void OnUserLoggedIn(string withUserId) {
		Debug.Log ("User logged in");
        Debug.Log("SHOW LOBBY");
        Megafans.Instance.ShowTournamentLobby (this, this);
	}

    public void OnUserRegistered() {
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