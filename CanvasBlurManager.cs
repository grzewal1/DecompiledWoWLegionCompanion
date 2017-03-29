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
		CanvasBlurManager mBlurRefCountLevel2Canvas = this;
		mBlurRefCountLevel2Canvas.m_blurRefCount_Level2Canvas = mBlurRefCountLevel2Canvas.m_blurRefCount_Level2Canvas + 1;
		this.UpdateBlurEffect();
	}

	public void AddBlurRef_MainCanvas()
	{
		CanvasBlurManager mBlurRefCountMainCanvas = this;
		mBlurRefCountMainCanvas.m_blurRefCount_MainCanvas = mBlurRefCountMainCanvas.m_blurRefCount_MainCanvas + 1;
		this.UpdateBlurEffect();
	}

	private void Awake()
	{
		this.UpdateBlurEffect();
	}

	public void RemoveBlurRef_Level2Canvas()
	{
		CanvasBlurManager mBlurRefCountLevel2Canvas = this;
		mBlurRefCountLevel2Canvas.m_blurRefCount_Level2Canvas = mBlurRefCountLevel2Canvas.m_blurRefCount_Level2Canvas - 1;
		this.UpdateBlurEffect();
	}

	public void RemoveBlurRef_MainCanvas()
	{
		CanvasBlurManager mBlurRefCountMainCanvas = this;
		mBlurRefCountMainCanvas.m_blurRefCount_MainCanvas = mBlurRefCountMainCanvas.m_blurRefCount_MainCanvas - 1;
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