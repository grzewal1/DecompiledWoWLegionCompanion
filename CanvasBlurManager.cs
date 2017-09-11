using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CanvasBlurManager : MonoBehaviour
{
	private int m_blurRefCount_MainCanvas;

	public Blur m_blurEffect_MainCanvas;

	private int m_blurRefCount_Level2Canvas;

	public Blur m_blurEffect_Level2Canvas;

	public CanvasBlurManager()
	{
	}

	public void AddBlurRef_Level2Canvas()
	{
		this.m_blurRefCount_Level2Canvas++;
		this.UpdateBlurEffect();
	}

	public void AddBlurRef_MainCanvas()
	{
		this.m_blurRefCount_MainCanvas++;
		this.UpdateBlurEffect();
	}

	private void Awake()
	{
		this.UpdateBlurEffect();
	}

	public void RemoveBlurRef_Level2Canvas()
	{
		this.m_blurRefCount_Level2Canvas--;
		this.UpdateBlurEffect();
	}

	public void RemoveBlurRef_MainCanvas()
	{
		this.m_blurRefCount_MainCanvas--;
		this.UpdateBlurEffect();
	}

	private void UpdateBlurEffect()
	{
		if (this.m_blurEffect_MainCanvas != null)
		{
			this.m_blurEffect_MainCanvas.enabled = this.m_blurRefCount_MainCanvas > 0;
		}
		if (this.m_blurEffect_Level2Canvas != null)
		{
			this.m_blurEffect_Level2Canvas.enabled = this.m_blurRefCount_Level2Canvas > 0;
		}
	}
}