#pragma warning disable 649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggleUI : MonoBehaviour {

	[Serializable]
	class Group {
		public GameObject[] items;
	}

	[SerializeField] Group[] groups;

	public void EnableGroup(int groupID) {
		if(groups == null || groups.Length == 0)
			return;

		for (int i = 0; i < groups.Length; i++) {
			if (groups[i] == null)
				continue;

			bool enable = i == groupID;
			foreach (var go in groups[i].items)
				go?.SetActive(enable);
		}
	}

}
