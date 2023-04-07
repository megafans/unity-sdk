    using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using MegafansSDK;
using MegafansSDK.UI;

namespace MegafansSDK.Utils
{
    public class InAppPurchaser : MonoBehaviour, IStoreListener
    {
        public static InAppPurchaser Instance;
        public static IStoreController megaFans_StoreController;
        private static IExtensionProvider megaFans_StoreExtensionProvider;
        private string productIdForPurchase;

        void Start()
        {
            Instance = this;

            if (megaFans_StoreController == null)
            {
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                Debug.Log(string.Format("Purchasing Ability is initialized"));
                return;
            }

            Debug.Log(string.Format("Purchasing Ability is NOT initialized"));

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            Debug.Log(string.Format("Builder created - {0}", builder));

            builder.AddProduct(Megafans.Instance.ProductID200Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID200Tokens));

            builder.AddProduct(Megafans.Instance.ProductID1000Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID1000Tokens));

            builder.AddProduct(Megafans.Instance.ProductID3000Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID3000Tokens));

            builder.AddProduct(Megafans.Instance.ProductID10000Tokens, ProductType.Consumable);
            Debug.Log(string.Format("Added product - {0}", Megafans.Instance.ProductID10000Tokens));

            UnityPurchasing.Initialize(this, builder);
        }


        private bool IsInitialized()
        {
            return megaFans_StoreController != null && megaFans_StoreExtensionProvider != null;
        }

        public ProductCollection GetProducts()
        {
            if (IsInitialized())
            {
                return megaFans_StoreController.products;
            }
            else
            {
                InitializePurchasing();
                return null;
            }
        }


        public void BuyTokens(int tokenQuanity)
        {
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

            if (IsInitialized())
            {
                MegafansUI.Instance.ShowLoadingBar();
                productIdForPurchase = null;

                Product product = megaFans_StoreController.products.WithID(productId);

                Debug.Log(string.Format("GOT PRODUCT - {0}", product));

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));

                    megaFans_StoreController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                productIdForPurchase = productId;
                InitializePurchasing();
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");

                var apple = megaFans_StoreExtensionProvider.GetExtension<IAppleExtensions>();

                apple.RestoreTransactions((result) =>
                {
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: PASS");

            megaFans_StoreController = controller;
            megaFans_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log("Processing Purchase Result");
            MegafansUI.Instance.HideLoadingBar();

            int tokenQuantity = 0;
            string transactionId = "";
            if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID200Tokens, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

                tokenQuantity = 200;
                transactionId = args.purchasedProduct.transactionID;
            }
            else if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID1000Tokens, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

                tokenQuantity = 1000;
                transactionId = args.purchasedProduct.transactionID;
            }
            else if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID3000Tokens, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                tokenQuantity = 3000;
                transactionId = args.purchasedProduct.transactionID;
            }
            else if (String.Equals(args.purchasedProduct.definition.id, Megafans.Instance.ProductID10000Tokens, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                tokenQuantity = 10000;
                transactionId = args.purchasedProduct.transactionID;
            }
            Debug.Log(string.Format("Receipt ='{0}'", args.purchasedProduct.receipt));

            if (transactionId != "" && tokenQuantity > 0)
            {
                Megafans.Instance.SaveTokens(transactionId, args.purchasedProduct.receipt, args.purchasedProduct.definition.id, tokenQuantity, () =>
                {
                    Debug.Log("Tokens saved successfully.");

                    MegafansUI.Instance.ShowBuyCoinsSuccessScreen(tokenQuantity);
                    megaFans_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                }, (error) =>
                {
                    megaFans_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                    MegafansUI.Instance.ShowPopup("Error", "Error purchasing tokens.  You were not charged for this transaction.  Please try again.");
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError(error);
                    }
                    else
                    {
                        Debug.LogError("Unknown error while saving tokens to user");
                    }
                });
            }

            return PurchaseProcessingResult.Pending;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            MegafansUI.Instance.HideLoadingBar();
            MegafansUI.Instance.ShowPopup("Error", "Error purchasing tokens.  You were not charged for this transaction.  Please try again.");
        }
    }
}