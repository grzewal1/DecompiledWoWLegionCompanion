using System;
using System.Collections;
using UnityEngine;
using WowJamMessages.MobileClientJSON;

public class FollowerInventoryListView : MonoBehaviour
{
	public GameObject m_headerPrefab;

	public GameObject m_followerInventoryListItemPrefab;

	public GameObject m_equipmentInventoryContent;

	private FollowerDetailView m_followerDetailView;

	private int m_abilityToReplace;

	public FollowerInventoryListView()
	{
	}

	private void HandleInventoryChanged()
	{
		if (this.m_followerDetailView != null)
		{
			this.Init(this.m_followerDetailView, this.m_abilityToReplace);
		}
	}

	public void Init(FollowerDetailView followerDetailView, int abilityToReplace)
	{
		this.m_followerDetailView = followerDetailView;
		this.m_abilityToReplace = abilityToReplace;
		FollowerInventoryListItem[] componentsInChildren = this.m_equipmentInventoryContent.GetComponentsInChildren<FollowerInventoryListItem>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
		int num = 0;
		IEnumerator enumerator = PersistentEquipmentData.equipmentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MobileFollowerEquipment current = (MobileFollowerEquipment)enumerator.Current;
				if (num == 0)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_headerPrefab);
					gameObject.transform.SetParent(this.m_equipmentInventoryContent.transform, false);
					gameObject.GetComponent<FollowerInventoryListItem>().SetHeaderText("Equipment");
				}
				GameObject gameObject1 = UnityEngine.Object.Instantiate<GameObject>(this.m_followerInventoryListItemPrefab);
				gameObject1.transform.SetParent(this.m_equipmentInventoryContent.transform, false);
				gameObject1.GetComponent<FollowerInventoryListItem>().SetEquipment(current, followerDetailView, abilityToReplace);
				num++;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable == null)
			{
			}
			disposable.Dispose();
		}
		if (num == 0)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.m_headerPrefab);
			gameObject2.transform.SetParent(this.m_equipmentInventoryContent.transform, false);
			gameObject2.GetComponent<FollowerInventoryListItem>().SetHeaderText(StaticDB.GetString("NO_EQUIPMENT", null));
		}
		int num1 = 0;
		IEnumerator enumerator1 = PersistentArmamentData.armamentDictionary.Values.GetEnumerator();
		try
		{
			while (enumerator1.MoveNext())
			{
				MobileFollowerArmamentExt mobileFollowerArmamentExt = (MobileFollowerArmamentExt)enumerator1.Current;
				if (num1 == 0)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.m_headerPrefab);
					gameObject3.transform.SetParent(this.m_equipmentInventoryContent.transform, false);
					gameObject3.GetComponent<FollowerInventoryListItem>().SetHeaderText("Armaments");
				}
				GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.m_followerInventoryListItemPrefab);
				gameObject4.transform.SetParent(this.m_equipmentInventoryContent.transform, false);
				gameObject4.GetComponent<FollowerInventoryListItem>().SetArmament(mobileFollowerArmamentExt, followerDetailView);
				num1++;
			}
		}
		finally
		{
			IDisposable disposable1 = enumerator1 as IDisposable;
			if (disposable1 == null)
			{
			}
			disposable1.Dispose();
		}
		if (num == 0)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(this.m_headerPrefab);
			gameObject5.transform.SetParent(this.m_equipmentInventoryContent.transform, false);
			gameObject5.GetComponent<FollowerInventoryListItem>().SetHeaderText(StaticDB.GetString("NO_ARMAMENTS", null));
		}
	}

	private void OnDisable()
	{
		Main.instance.ArmamentInventoryChangedAction -= new Action(this.HandleInventoryChanged);
		Main.instance.EquipmentInventoryChangedAction -= new Action(this.HandleInventoryChanged);
	}

	private void OnEnable()
	{
		Main.instance.ArmamentInventoryChangedAction += new Action(this.HandleInventoryChanged);
		Main.instance.EquipmentInventoryChangedAction += new Action(this.HandleInventoryChanged);
	}
}