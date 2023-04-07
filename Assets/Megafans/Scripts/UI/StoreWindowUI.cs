#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class StoreWindowUI : MonoBehaviour
    {
        [SerializeField] GameObject Container;
        [SerializeField] private Text userTournamentTokensTxt;
        [SerializeField] private Text userClientTokensTxt;
        [SerializeField] private ListBox listBox;
        [SerializeField] private Text messageTxt;
        [SerializeField] private Sprite confirmationIcon;
        [SerializeField] private GameObject storeItemPrefab;
        [SerializeField] private GameObject storeItemBestDealPrefab;
        [SerializeField] private GameObject storeItemFree;

        void Awake()
        {
            if (Screen.orientation != ScreenOrientation.Portrait)
            {
                Container.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                Container.transform.position = new Vector3(Container.transform.position.x, Container.transform.position.y + 34, Container.transform.position.z);
            }
        }

        void Start()
        {
            //UnityEngine.Purchasing.ProductCollection allproducts = InAppPurchaser.Instance.GetProducts();
        }

        private void OnEnable()
        {
            userTournamentTokensTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userClientTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
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
                totalCredits += float.Parse(userTournamentTokensTxt.text);
            }

            userTournamentTokensTxt.text = totalCredits.ToString();
        }

        public void UpdateCreditUI()
        {
            userTournamentTokensTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userClientTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
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
            Application.OpenURL(MegafansSDK.AdsManagerAPI.AdsManager.instance.freeTokensURL);
        }

    }

}