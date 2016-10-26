using System;
using UnityEngine;
using UnityEngine.UI;

public class MissionStartedEffect : MonoBehaviour
{
	public CanvasGroup m_toastCanvasGroup;

	public RectTransform m_rootRT;

	public float m_raisedHeight;

	public float m_loweredHeight;

	public Text m_missionStartedLabel;

	public CanvasGroup m_glowCanvasGroup;

	public Image m_glowImage;

	private void Awake()
	{
		this.m_toastCanvasGroup.set_alpha(0f);
		this.m_toastCanvasGroup.set_blocksRaycasts(false);
		this.m_toastCanvasGroup.set_interactable(false);
		this.m_glowCanvasGroup.set_alpha(0f);
		this.m_glowCanvasGroup.set_blocksRaycasts(false);
		this.m_glowCanvasGroup.set_interactable(false);
		this.m_missionStartedLabel.set_font(GeneralHelpers.LoadFancyFont());
		this.m_missionStartedLabel.set_text(StaticDB.GetString("MISSION_STARTED", "Mission Started PH"));
	}

	private void Start()
	{
		this.m_rootRT.set_localScale(Vector3.get_one());
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"Phase1",
			"delay",
			0.6f,
			"from",
			0f,
			"to",
			1f,
			"easeType",
			"easeOutCubic",
			"time",
			0.75f,
			"onupdate",
			"Phase1Update",
			"oncomplete",
			"Phase1Complete"
		}));
	}

	private void Phase1Update(float val)
	{
		this.m_rootRT.set_anchoredPosition(new Vector2(this.m_rootRT.get_anchoredPosition().x, this.m_loweredHeight + val * (this.m_raisedHeight - this.m_loweredHeight)));
		this.m_toastCanvasGroup.set_alpha(Mathf.Min(val / 0.6f, 1f));
	}

	private void Phase1Complete()
	{
		this.m_rootRT.set_anchoredPosition(new Vector2(this.m_rootRT.get_anchoredPosition().x, this.m_raisedHeight));
		this.m_toastCanvasGroup.set_alpha(1f);
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"Phase2",
			"delay",
			0.5f,
			"from",
			1f,
			"to",
			0f,
			"easeType",
			"easeOutCubic",
			"time",
			0.5f,
			"onupdate",
			"Phase2Update",
			"oncomplete",
			"Phase2Complete"
		}));
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"Phase3",
			"delay",
			0.5f,
			"from",
			0f,
			"to",
			3.14159274f,
			"easeType",
			iTween.EaseType.easeInOutCubic,
			"time",
			1f,
			"onupdate",
			"Phase3Update",
			"oncomplete",
			"Phase3Complete"
		}));
	}

	private void Phase2Update(float val)
	{
		this.m_rootRT.set_anchoredPosition(new Vector2(this.m_rootRT.get_anchoredPosition().x, this.m_loweredHeight + val * (this.m_raisedHeight - this.m_loweredHeight)));
		this.m_toastCanvasGroup.set_alpha(val);
		this.m_rootRT.set_localScale((0.5f + val * 0.5f) * Vector3.get_one());
	}

	private void Phase2Complete()
	{
		this.m_rootRT.set_anchoredPosition(new Vector2(this.m_rootRT.get_anchoredPosition().x, this.m_loweredHeight));
		this.m_toastCanvasGroup.set_alpha(0f);
		this.m_rootRT.set_localScale(0.5f * Vector3.get_one());
	}

	private void Phase3Update(float val)
	{
		this.m_glowCanvasGroup.set_alpha(Mathf.Sin(val));
	}

	private void Phase3Complete()
	{
		Object.DestroyObject(base.get_gameObject());
	}
}
