using System;
using UnityEngine;

public class MissionDialog : MonoBehaviour
{
	public MissionDetailView m_missionDetailView;

	public FollowerListView m_followerListView;

	private void OnEnable()
	{
		AdventureMapPanel expr_05 = AdventureMapPanel.instance;
		expr_05.MissionSelectedFromListAction = (Action<int>)Delegate.Combine(expr_05.MissionSelectedFromListAction, new Action<int>(this.HandleMissionSelected));
	}

	private void OnDisable()
	{
		AdventureMapPanel expr_05 = AdventureMapPanel.instance;
		expr_05.MissionSelectedFromListAction = (Action<int>)Delegate.Remove(expr_05.MissionSelectedFromListAction, new Action<int>(this.HandleMissionSelected));
	}

	private void HandleMissionSelected(int garrMissionID)
	{
		if (garrMissionID == 0)
		{
			this.m_missionDetailView.get_gameObject().SetActive(false);
			return;
		}
		this.m_missionDetailView.get_gameObject().SetActive(true);
		this.m_missionDetailView.HandleMissionSelected(garrMissionID);
		this.m_followerListView.m_missionDetailView = this.m_missionDetailView;
		this.m_followerListView.InitFollowerList();
		this.m_followerListView.HandleMissionChanged(garrMissionID);
	}
}
