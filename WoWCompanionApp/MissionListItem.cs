using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WowStaticData;

namespace WoWCompanionApp
{
	public class MissionListItem : MonoBehaviour
	{
		public Text missionNameText;

		public Text missionLevelText;

		public Text missionTimeRemainingText;

		public Text missionResultsText;

		public Text rareMissionText;

		public Image levelBG;

		public GameObject inProgressDarkener;

		public Image[] locationImages;

		public Image m_missionTypeImage;

		public int garrMissionID;

		public int locationIndex;

		public GameObject missionRewardGroup;

		public GameObject missionRewardDisplayPrefab;

		public bool isResultsItem;

		private TimeSpan missionDurationInSeconds;

		private DateTime missionStartedTime;

		public MissionListItem()
		{
		}

		public void Init(int missionRecID)
		{
			this.garrMissionID = missionRecID;
			GarrMissionRec record = StaticDB.garrMissionDB.GetRecord(this.garrMissionID);
			if (record == null)
			{
				return;
			}
			if (!PersistentMissionData.missionDictionary.ContainsKey(this.garrMissionID))
			{
				return;
			}
			this.missionDurationInSeconds = TimeSpan.FromSeconds((double)record.MissionDuration);
			WrapperGarrisonMission item = PersistentMissionData.missionDictionary[this.garrMissionID];
			this.missionStartedTime = item.StartTime;
			Duration duration = new Duration(record.MissionDuration, false);
			string str = (duration.Hours < 2 ? "<color=#ffffffff>" : "<color=#ff8600ff>");
			TimeSpan timeSpan = GarrisonStatus.CurrentTime() - this.missionStartedTime;
			TimeSpan timeSpan1 = this.missionDurationInSeconds - timeSpan;
			timeSpan1 = (timeSpan1.TotalSeconds <= 0 ? TimeSpan.Zero : timeSpan1);
			bool flag = (item.MissionState != 1 ? false : timeSpan1.TotalSeconds > 0);
			this.missionNameText.text = string.Concat(record.Name, (!flag ? string.Concat(" (", str, duration.DurationString, "</color>)") : string.Empty));
			this.missionLevelText.text = string.Concat(string.Empty, record.TargetLevel);
			this.inProgressDarkener.SetActive(flag);
			this.missionTimeRemainingText.gameObject.SetActive(flag);
			this.missionDurationInSeconds = TimeSpan.FromSeconds((double)record.MissionDuration);
			this.missionResultsText.gameObject.SetActive(false);
			this.isResultsItem = false;
			MissionRewardDisplay.InitMissionRewards(this.missionRewardDisplayPrefab, this.missionRewardGroup.transform, item.Rewards);
			for (int i = 0; i < (int)this.locationImages.Length; i++)
			{
				if (this.locationImages[i] != null)
				{
					this.locationImages[i].gameObject.SetActive(false);
				}
			}
			Image image = null;
			uint uiTextureKitID = record.UiTextureKitID;
			switch (uiTextureKitID)
			{
				case 101:
				{
					image = this.locationImages[1];
					this.locationIndex = 1;
					break;
				}
				case 102:
				{
					image = this.locationImages[10];
					this.locationIndex = 10;
					break;
				}
				case 103:
				{
					image = this.locationImages[3];
					this.locationIndex = 3;
					break;
				}
				case 104:
				{
					image = this.locationImages[4];
					this.locationIndex = 4;
					break;
				}
				case 105:
				{
					image = this.locationImages[9];
					this.locationIndex = 9;
					break;
				}
				case 106:
				{
					image = this.locationImages[7];
					this.locationIndex = 7;
					break;
				}
				case 107:
				{
					image = this.locationImages[8];
					this.locationIndex = 8;
					break;
				}
				default:
				{
					switch (uiTextureKitID)
					{
						case 203:
						{
							image = this.locationImages[2];
							this.locationIndex = 2;
							break;
						}
						case 204:
						{
							image = this.locationImages[6];
							this.locationIndex = 6;
							break;
						}
						case 205:
						{
							image = this.locationImages[5];
							this.locationIndex = 5;
							break;
						}
						default:
						{
							if (uiTextureKitID == 164)
							{
								image = this.locationImages[0];
								this.locationIndex = 0;
							}
							else if (uiTextureKitID == 165)
							{
								image = this.locationImages[11];
								this.locationIndex = 11;
							}
							else
							{
								this.locationIndex = 0;
							}
							break;
						}
					}
					break;
				}
			}
			if (image != null)
			{
				image.gameObject.SetActive(true);
			}
			GarrMissionTypeRec garrMissionTypeRec = StaticDB.garrMissionTypeDB.GetRecord((int)record.GarrMissionTypeID);
			this.m_missionTypeImage.overrideSprite = TextureAtlas.instance.GetAtlasSprite((int)garrMissionTypeRec.UiTextureAtlasMemberID);
			if ((record.Flags & 1) == 0)
			{
				this.rareMissionText.gameObject.SetActive(false);
			}
			else
			{
				this.rareMissionText.gameObject.SetActive(true);
				Color color = this.levelBG.color;
				color.r = 0f;
				color.g = 0.211f;
				color.b = 0.506f;
				this.levelBG.color = color;
			}
		}

		public void OnTap()
		{
			if (this.isResultsItem)
			{
				return;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (!this.missionTimeRemainingText.gameObject.activeSelf)
			{
				return;
			}
			TimeSpan timeSpan = GarrisonStatus.CurrentTime() - this.missionStartedTime;
			TimeSpan timeSpan1 = this.missionDurationInSeconds - timeSpan;
			timeSpan1 = (timeSpan1.TotalSeconds <= 0 ? TimeSpan.Zero : timeSpan1);
			this.missionTimeRemainingText.text = string.Concat(timeSpan1.GetDurationString(false), " <color=#ff0000ff>(In Progress)</color>");
		}
	}
}