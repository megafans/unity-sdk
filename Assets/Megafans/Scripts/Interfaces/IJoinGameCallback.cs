using System.Collections.Generic;

namespace MegafansSDK
{

    public interface IJoinGameCallback
    {

        /// <summary>
        /// Called when user has joined a match. Start the game here.
        /// </summary>
        /// <param name="gameType">Game type to be used by SaveScore function later. Store it for later use.</param>
        /// <param name="metaData">MetaData for tournament. Specifying level to play, time limit, etc.</param>
        void StartGame(GameType gameType, Dictionary<string, string> metaData);

        /// <summary>
        /// Called when the user wants to purchase a specific number of tokens. Start IAP flow here to allow
        /// the user to purchase tokens.
        /// </summary>
        /// <param name="numberOfTokens">Number of tokens to be purchased using IAP.</param>
        void PurchaseTokens(int numberOfTokens);

    }

}