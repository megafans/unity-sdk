using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using MegafansSDK;
using MegafansSDK.UI;

namespace MegafansSDK.Utils
{
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class InAppPurchaser : MonoBehaviour, IStoreListener
    {
        public static InAppPurchaser Instance;
        public static IStoreController megaFans_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider megaFans_StoreExtensionProvider; // The store-specific Purchasing subsystems.
        private string productIdForPurchase;

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        //public static string kProductID1000Tokens = "commegafansjetjackcp1";
        //public static string kProductID3000Tokens = "commegafansjetjackcp2";
        //public static string kProductID10000Tokens = "commegafansjetjackcp3";


        void Start()
        {
            Instance = this;
            // If we haven't set up the Unity Purchasing reference
            if (megaFans_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                Debug.Log(string.Format("Purchasing Ability is initialized"));
                return;
            }
            Debug.Log(string.Format("Purchasing Ability is NOT initialized"));
            // Create a builder, first passing in a suite of Unity provided stores.

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            Debug.Log(string.Format("Builder created - {0}", builder));
            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            builder.AddProduct(Megafans.Instance.ProductID200Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID200Tokens));
            builder.AddProduct(Megafans.Instance.ProductID1000Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID1000Tokens));
            builder.AddProduct(Megafans.Instance.ProductID3000Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID3000Tokens));
            builder.AddProduct(Megafans.Instance.ProductID10000Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID10000Tokens));

            //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
            //// And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            //// if the Product ID was configured differently between Apple and Google stores. Also note that
            //// one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            //// must only be referenced here. 
            //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            //    { kProductNameAppleSubscription, AppleAppStore.Name },
            //    { kProductNameGooglePlaySubscription, GooglePlay.Name },
            //});

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
            UnityPurchasing.Initialize(Instance, builder);
        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return megaFans_StoreController != null && megaFans_StoreExtensionProvider != null;
        }

        public ProductCollection GetProducts()
        {
            if (IsInitialized()) {
                return megaFans_StoreController.products;
            } else {
                InitializePurchasing();
                return null;
            }               
        }


        public void BuyTokens(int tokenQuanity)
        {
            // Buy the consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            switch (tokenQuanity)
            {
                case 200:
                    BuyProductID(Megafans.Instance.ProductID200Tokens);
                    return;
                case 1000:
                    BuyProductID(Megafans.Instance.ProductID1000Tokens);
                    return;
                case 3000:
                    BuyProductID(Megafans.Instance.ProductID3000Tokens);
                    return;
                case 10000:
                    BuyProductID(Megafans.Instance.ProductID10000Tokens);
                    return;
                default:
                    return;
            }
        }

        void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                MegafansUI.Instance.ShowLoadingBar();
                productIdForPurchase = null;
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = megaFans_StoreController.products.WithID(productId);
                Debug.Log(string.Format("GOT PRODUCT - {0}", product));
                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    megaFans_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                productIdForPurchase = productId;
                InitializePurchasing();
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = megaFans_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        //public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        //{
        //    // Purchasing has succeeded initializing. Collect our Purchasing references.
        //    Debug.Log("OnInitialized: PASS");

        //    // Overall Purchasing system, configured with products for this application.
        //    m_StoreController = controller;
        //    // Store specific subsystem, for accessing device-specific store features.
        //    m_StoreExtensionProvider = extensions;

        //    if (!string.IsNullOrEmpty(productIdForPurchase)) {
        //        BuyProductID(productIdForPurchase);
        //    }
        //}

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            megaFans_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            megaFans_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
            Debug.Log("Processing Purchase Result");
            MegafansUI.Instance.HideLoadingBar();

            int tokenQuantity = 0;
            string transactionId = "";
            if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID200Tokens, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                //ScoreManager.score += 100;
                tokenQuantity = 200;
                transactionId = args.purchasedProduct.transactionID;
            } else if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID1000Tokens, StringComparison.Ordinal)) {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                //ScoreManager.score += 100;
                tokenQuantity = 1000;
                transactionId = args.purchasedProduct.transactionID;
            } else if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID3000Tokens, StringComparison.Ordinal)) {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                tokenQuantity = 3000;
                transactionId = args.purchasedProduct.transactionID;
            } else if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID10000Tokens, StringComparison.Ordinal)) {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                tokenQuantity = 10000;
                transactionId = args.purchasedProduct.transactionID;
            }
            Debug.Log(string.Format("Receipt ='{0}'", args.purchasedProduct.receipt));
            if (transactionId != "" && tokenQuantity > 0) {
                Megafans.Instance.SaveTokens(transactionId, args.purchasedProduct.receipt, args.purchasedProduct.definition.id, tokenQuantity, () => {
                    Debug.Log("Tokens saved successfully.");
                    //MegafansUI.Instance.ShowPopup("Success", "Success purchasing tokens.");
                    MegafansUI.Instance.ShowBuyCoinsSuccessScreen(tokenQuantity);
                    megaFans_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                }, (error) => {
                    megaFans_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                    MegafansUI.Instance.ShowPopup("Error", "Error purchasing tokens.  You were not charged for this transaction.  Please try again.");
                    if (!string.IsNullOrEmpty(error)) {
                        Debug.LogError(error);
                    } else {
                        Debug.LogError("Unknown error while saving tokens to user");
                    }
                });
            }

            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            MegafansUI.Instance.HideLoadingBar();
            MegafansUI.Instance.ShowPopup("Error", "Error purchasing tokens.  You were not charged for this transaction.  Please try again.");
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
}