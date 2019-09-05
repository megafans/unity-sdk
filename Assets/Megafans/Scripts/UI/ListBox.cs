using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MegafansSDK.UI {
	
	public class ListBox : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {
		
		[SerializeField] private GameObject listItemsHolder;
		[SerializeField] private GameObject header;
		[SerializeField] private GameObject footer;
        [SerializeField] public ScrollRect scrollRect;

        float[] points;
        [Tooltip("how many screens or pages are there within the content (steps)")]
        public int screens = 1;
        float stepSize;

        ScrollRect scroll;
        bool LerpH;
        float targetH;
        [Tooltip("Snap horizontally")]
        public bool snapInH = true;

        bool LerpV;
        float targetV;
        [Tooltip("Snap vertically")]
        public bool snapInV = true;

        public int currentIndex = 0;


        public GameObject Header {
			get {
				return header;
			}
		}

		public void AddItem(GameObject item) {
			item.transform.SetParent (listItemsHolder.transform, false);
			if (header != null) {
				header.transform.SetAsFirstSibling ();
			}

			if (footer != null) {
				footer.transform.SetAsLastSibling ();
			}
		}

		public GameObject GetItem(int index) {
			if (index < 0 || index >= listItemsHolder.transform.childCount) {
				return null;
			}

			return listItemsHolder.transform.GetChild (index).gameObject;
		}

		public void ClearList() {
			for (int i = 0; i < listItemsHolder.transform.childCount; i++) {
				if (header != null && i == 0) {
					continue;
				}

				if (footer != null && i == listItemsHolder.transform.childCount - 1) {
					continue;
				}

				Destroy (listItemsHolder.transform.GetChild (i).gameObject);
			}
		}

        public void OnPointerClick(PointerEventData eventData)
        {
        Debug.Log("Button clicked = " + eventData);
        //if (canClick == true)
        //{
        //    // add code for click here
        //}
        //canClick = true;
        }

        public void DidSelectItem() {
            Debug.Log("Button clicked = " + currentIndex);
        }

        // Use this for initialization
        public void SetUpForScreenCount(int screenCount)
        {
            screens = screenCount;
            scroll = gameObject.GetComponent<ScrollRect>();
            scroll.inertia = false;
            if (screens > 0)
            {
                points = new float[screens];
                stepSize = 1 / (float)(screens - 1);

                for (int i = 0; i < screens; i++)
                {
                    points[i] = i * stepSize;
                }
            }
            else
            {
                points = new float[0];
            }
        }

        void Update()
        {
            if (LerpH)
            {
                scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, targetH, 50 * scroll.elasticity * Time.deltaTime);
                if (Mathf.Approximately(scroll.horizontalNormalizedPosition, targetH)) {
                    LerpH = false;
                } 
            }
            if (LerpV)
            {
                scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, targetV, 50 * scroll.elasticity * Time.deltaTime);
                if (Mathf.Approximately(scroll.verticalNormalizedPosition, targetV)) LerpV = false;
            }
        }

        public void DragEnd()
        {
            if (scroll.horizontal && snapInH)
            {
                currentIndex = FindNearest(scroll.horizontalNormalizedPosition, points);
                targetH = points[currentIndex];
                GameObject tournamentLobbyUI = this.transform.parent.gameObject;
                ExecuteEvents.Execute<TournamentCardItemCustomMessageTarget>(tournamentLobbyUI, null, (x, y) => x.ScrollViewDidFinishScrollingOnIndex(currentIndex));
                LerpH = true;
            }
            if (scroll.vertical && snapInV)
            {
                targetH = points[FindNearest(scroll.verticalNormalizedPosition, points)];
                LerpH = true;
            }
        }

        public void OnDrag()
        {
            LerpH = false;
            LerpV = false;
        }

        int FindNearest(float f, float[] array)
        {
            float distance = Mathf.Infinity;
            int output = 0;
            for (int index = 0; index < array.Length; index++)
            {
                if (Mathf.Abs(array[index] - f) < distance)
                {
                    distance = Mathf.Abs(array[index] - f);
                    output = index;
                }
            }
            return output;
        }

        public void next() {
            if (currentIndex < points.Length - 1) {
                currentIndex += 1;
                float scrollPoint = points[currentIndex];
                targetH = scrollPoint;
                LerpH = true;
                GameObject tournamentLobbyUI = this.transform.parent.gameObject;
                ExecuteEvents.Execute<TournamentCardItemCustomMessageTarget>(tournamentLobbyUI, null, (x, y) => x.ScrollViewDidFinishScrollingOnIndex(currentIndex));
            }
            //scroll.horizontalNormalizedPosition = targetH;
            //if (currentIndex != null) {
            //    scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, targetH, 50 * scroll.elasticity * Time.deltaTime);
            //} else {
            //    scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, targetH, 50 * scroll.elasticity * Time.deltaTime);
            //}
        }

        public void back() {
            if (currentIndex > 0)
            {
                currentIndex -= 1;
                float scrollPoint = points[currentIndex];
                targetH = scrollPoint;
                LerpH = true;
                GameObject tournamentLobbyUI = this.transform.parent.gameObject;
                ExecuteEvents.Execute<TournamentCardItemCustomMessageTarget>(tournamentLobbyUI, null, (x, y) => x.ScrollViewDidFinishScrollingOnIndex(currentIndex));
            }
        }
    }
}