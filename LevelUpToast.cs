using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpToast : MonoBehaviour
{
	public CanvasGroup m_baseCanvasGroup;

	public CanvasGroup m_glowCanvasGroup;

	public CanvasGroup m_glintCanvasGroup;

	public RectTransform m_glintRT;

	public Text m_level;

	public Text m_levelUpLabel;

	public float m_initialFadeInDuration;

	public float m_delayUntilPhase2Duration;

	public float m_glowFadeOutDuration;

	public float m_delayUntilPhase3Duration;

	public float m_toastFadeOutDuration;

	public float m_glintInitialDelayDuration;

	public float m_glintDuration;

	private void Awake()
	{
		this.m_baseCanvasGroup.set_alpha(0f);
		this.m_baseCanvasGroup.set_interactable(false);
		this.m_baseCanvasGroup.set_blocksRaycasts(false);
		this.m_glowCanvasGroup.set_alpha(0f);
		this.m_glowCanvasGroup.set_interactable(false);
		this.m_glowCanvasGroup.set_blocksRaycasts(false);
		this.m_glintCanvasGroup.set_alpha(0f);
		this.m_glintCanvasGroup.set_interactable(false);
		this.m_glintCanvasGroup.set_blocksRaycasts(false);
		this.m_level.set_font(GeneralHelpers.LoadStandardFont());
		this.m_level.set_text("XXX");
		this.m_levelUpLabel.set_font(GeneralHelpers.LoadStandardFont());
		this.m_levelUpLabel.set_text(StaticDB.GetString("LEVEL_UP", "Level Up! (PH)"));
	}

	public void Show(int newLevel)
	{
		this.m_level.set_text(string.Empty + newLevel);
		Main.instance.m_UISound.Play_PlayerLevelUp();
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"Phase1",
			"delay",
			0f,
			"from",
			0f,
			"to",
			1f,
			"easeType",
			"easeInCubic",
			"time",
			this.m_initialFadeInDuration,
			"onupdate",
			"Phase1Update",
			"oncomplete",
			"Phase1Complete"
		}));
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"GlintPhase1",
			"delay",
			this.m_glintInitialDelayDuration,
			"from",
			0f,
			"to",
			1f,
			"easeType",
			iTween.EaseType.easeInOutCubic,
			"time",
			this.m_glintDuration,
			"onupdate",
			"GlintPhase1Update",
			"oncomplete",
			"GlintPhase1Complete"
		}));
	}

	private void Phase1Update(float val)
	{
		this.m_baseCanvasGroup.set_alpha(val);
		this.m_glowCanvasGroup.set_alpha(val);
	}

	private void Phase1Complete()
	{
		this.m_baseCanvasGroup.set_alpha(1f);
		this.m_glowCanvasGroup.set_alpha(1f);
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"Phase2",
			"delay",
			this.m_delayUntilPhase2Duration,
			"from",
			1f,
			"to",
			0f,
			"easeType",
			"easeOutCubic",
			"time",
			this.m_glowFadeOutDuration,
			"onupdate",
			"Phase2Update",
			"oncomplete",
			"Phase2Complete"
		}));
	}

	private void Phase2Update(float val)
	{
		this.m_glowCanvasGroup.set_alpha(val);
	}

	private void Phase2Complete()
	{
		this.m_glowCanvasGroup.set_alpha(0f);
		iTween.ValueTo(base.get_gameObject(), iTween.Hash(new object[]
		{
			"name",
			"Phase3",
			"delay",
			this.m_delayUntilPhase3Duration,
			"from",
			1f,
			"to",
			0f,
			"easeType",
			"easeInCubic",
			"time",
			this.m_toastFadeOutDuration,
			"onupdate",
			"Phase3Update",
			"oncomplete",
			"Phase3Complete"
		}));
	}

	private void Phase3Update(float val)
	{
		this.m_baseCanvasGroup.set_alpha(val);
	}

	private void Phase3Complete()
	{
		this.m_baseCanvasGroup.set_alpha(0f);
	}

	private void GlintPhase1Update(float val)
	{
		Vector2 anchoredPosition = this.m_glintRT.get_anchoredPosition();
		anchoredPosition.x = 10f + 240f * val;
		this.m_glintRT.set_anchoredPosition(anchoredPosition);
		float num = 0.5f;
		float num2 = 0.6f;
		if (val < num)
		{
			this.m_glintCanvasGroup.set_alpha(val / num);
		}
		else if (val < num2)
		{
			this.m_glintCanvasGroup.set_alpha(1f);
		}
		else
		{
			this.m_glintCanvasGroup.set_alpha(1f - (val - num2) / (1f - num2));
		}
	}

	private void GlintPhase1Complete()
	{
		this.m_glintCanvasGroup.set_alpha(0f);
	}
}
