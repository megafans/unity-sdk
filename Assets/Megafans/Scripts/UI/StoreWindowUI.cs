#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class StoreWindowUI : MonoBehaviour
    {

        [SerializeField] private Text userTokensTxt;
        [SerializeField] private ListBox listBox;
        [SerializeField] private Text messageTxt;
        [SerializeField] private Sprite confirmationIcon;
        [SerializeField] private GameObject storeItemPrefab;
        [SerializeField] private GameObject storeItemBestDealPrefab;
        [SerializeField] private GameObject storeItemFree;

        void Awake()
        {

        }

        void Start()
        {
            //UnityEngine.Purchasing.ProductCollection allproducts = InAppPurchaser.Instance.GetProducts();
        }

        private void OnEnable()
        {
            userTokensTxt.text = "TET : " + MegafansPrefs.TournamentEntryTokens.ToString();
            //userTokensTxt.text = "" + MegafansPrefs.CurrentTokenBalance.ToString();
            listBox.ClearList();
            MegafansUI.Instance.ShowLoadingBar();
            StartCoroutine(SetProducts());
        }

        public IEnumerator SetProducts()
        {
            UnityEngine.Purchasing.ProductCollection allproducts = InAppPurchaser.Instance.GetProducts();
            if (allproducts != null)
            {
                GameObject bestDeal = Instantiate(storeItemBestDealPrefab);
                for (int i = 0; i < allproducts.all.Length; ++i)
                {
                    UnityEngine.Purchasing.Product currentProduct = allproducts.all[i];
                    if (currentProduct.definition.id == Megafans.Instance.ProductID10000Tokens)
                    {
                        StoreItem viewHandler = bestDeal.GetComponent<StoreItem>();
                        viewHandler.SetValues(10000, currentProduct.metadata.localizedPriceString, () =>
                        {
                            StoreItemBuyBtn_OnClick(10000);
                        });
                    }
                    else
                    {
                        GameObject item = Instantiate(storeItemPrefab);
                        StoreItem viewHandler = item.GetComponent<StoreItem>();
                        if (viewHandler != null)
                        {
                            if (currentProduct.definition.id == Megafans.Instance.ProductID200Tokens)
                            {
                                viewHandler.SetValues(200, currentProduct.metadata.localizedPriceString, () =>
                                {
                                    StoreItemBuyBtn_OnClick(200);
                                });
                            }
                            else if (currentProduct.definition.id == Megafans.Instance.ProductID1000Tokens)
                            {
                                viewHandler.SetValues(1000, currentProduct.metadata.localizedPriceString, () =>
                                {
                                    StoreItemBuyBtn_OnClick(1000);
                                });
                            }
                            else if (currentProduct.definition.id == Megafans.Instance.ProductID3000Tokens)
                            {
                                viewHandler.SetValues(3000, currentProduct.metadata.localizedPriceString, () =>
                                {
                                    StoreItemBuyBtn_OnClick(3000);
                                });
                            }
                            listBox.AddItem(item);
                        }
                    }
                }

                if (bestDeal != null)
                {
                    listBox.AddItem(bestDeal);
                }
                MegafansUI.Instance.HideLoadingBar();
            }
            else
            {
                MegafansUI.Instance.ShowLoadingBar();
                int countDown = 3;
                while (countDown > 0)
                {
                    Debug.Log("Countdown: " + countDown);
                    yield return new WaitForSeconds(1.0f);
                    countDown--;
                }
                StartCoroutine(SetProducts());
            }

            //TODO: Offer Wall Within iaps
            GameObject itemFree = Instantiate(storeItemFree);
            StoreItem viewHandlerFree = itemFree.GetComponent<StoreItem>();

            if (itemFree != null)
            {
                viewHandlerFree.SetValues(() => StoreItemFreeBtn_OnClick());
                listBox.AddItem(itemFree);
            }
        }

        public void Init(float credits, bool addToExisting)
        {
            float totalCredits = credits;
            if (addToExisting)
            {
                totalCredits += float.Parse(userTokensTxt.text);
            }

            userTokensTxt.text = totalCredits.ToString();
        }

        public void UpdateCreditUI()
        {
            userTokensTxt.text = "TET : " + MegafansPrefs.TournamentEntryTokens.ToString();
            //userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        public void BackBtn_OnClick()
        {
            MegafansUI.Instance.BackFromStoreWindow();
        }

        public void StoreItemBuyBtn_OnClick(int tokenQty)
        {
            InAppPurchaser.Instance.BuyTokens(tokenQty);
        }

        public void StoreItemFreeBtn_OnClick()
        {
            MegafansSDK.Megafans.Instance.m_AdsManager.ShowOfferwall();
        }

        //private void ShowConfirmationDialog(int itemIdx) {
        //	GameObject item = listBox.GetItem (itemIdx);
        //	if (item != null) {
        //		StoreItem itemHandler = item.GetComponent<StoreItem> ();
        //		if (itemHandler != null) {
        //			int tokensQty = itemHandler.Quantity;
        //			float price = itemHandler.Price;

        //			string msg = "You are purchasing <color=#f5a623ff>" + tokensQty + " Tokens</color> \nfor " +
        //			             "<b><color=black>$" + price + "</color></b>" +
        //			             "\n\nDo you want to continue the purchase?";

        //			MegafansUI.Instance.ShowAlertDialog (confirmationIcon, "Confirmation",
        //				msg, "Buy", "Cancel",
        //				() => {
        //					MegafansUI.Instance.HideAlertDialog();
        //                          Debug.Log(string.Format("InAppPurchaser Instance is null - {0}", InAppPurchaser.Instance == null));
        //                          InAppPurchaser.Instance.BuyTokens(tokensQty);						
        //				},
        //				() => {
        //					MegafansUI.Instance.HideAlertDialog();
        //				});
        //		}
        //	}
        //}

    }

}