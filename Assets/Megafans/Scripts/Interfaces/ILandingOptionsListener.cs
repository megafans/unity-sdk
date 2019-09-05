
namespace MegafansSDK {

	public interface ILandingOptionsListener {

		/// <summary>
		/// Called when the user clicks Play Game button. Start the game here normally.
		/// </summary>
		void OnPlayGameClicked();

		/// <summary>
		/// Called when the user logs in to the MegaFans Platform.
		/// </summary>
		void OnUserLoggedIn(string userId);

		/// <summary>
		/// Called when the user registers with the MegaFans Platform.
		/// </summary>
		void OnUserRegistered();

	}

}