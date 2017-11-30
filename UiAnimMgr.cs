using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UiAnimMgr
{
	private static UiAnimMgr s_instance;

	private static bool s_initialized;

	private Dictionary<string, UiAnimMgr.AnimData> m_animData;

	private int m_idIndex;

	private GameObject m_parentObj;

	public static UiAnimMgr instance
	{
		get
		{
			if (UiAnimMgr.s_instance == null)
			{
				UiAnimMgr.s_instance = new UiAnimMgr();
				UiAnimMgr.s_instance.InitAnimMgr();
			}
			return UiAnimMgr.s_instance;
		}
	}

	public Material m_additiveMaterial
	{
		get;
		private set;
	}

	public Material m_blendMaterial
	{
		get;
		private set;
	}

	static UiAnimMgr()
	{
	}

	public UiAnimMgr()
	{
	}

	public void AnimComplete(UiAnimation script)
	{
		UiAnimMgr.AnimData animDatum;
		GameObject gameObject = script.gameObject;
		this.m_animData.TryGetValue(gameObject.name, out animDatum);
		if (animDatum == null)
		{
			Debug.Log(string.Concat("Error! UiAnimMgr could not find completed anim ", gameObject.name));
			return;
		}
		if (!animDatum.m_activeObjects.Remove(gameObject))
		{
			Debug.Log(string.Concat("Error! anim obj ", gameObject.name, "not in UiAnimMgr active list"));
		}
		animDatum.m_availableObjects.Push(gameObject);
		gameObject.SetActive(false);
		gameObject.transform.SetParent(this.m_parentObj.transform);
		gameObject.GetComponent<UiAnimation>().m_ID = 0;
	}

	private GameObject CreateAnimObj(string animName, bool createForInit = false)
	{
		UiAnimMgr.AnimData animDatum;
		string mAnchor;
		Dictionary<string, int> strs;
		int num;
		this.m_animData.TryGetValue(animName, out animDatum);
		if (animDatum == null)
		{
			return null;
		}
		GameObject gameObject = null;
		if (!createForInit && animDatum.m_availableObjects.Count > 0)
		{
			gameObject = animDatum.m_availableObjects.Pop();
		}
		if (gameObject != null)
		{
			if (!animDatum.m_activeObjects.Contains(gameObject))
			{
				animDatum.m_activeObjects.Add(gameObject);
			}
			else
			{
				Debug.Log("Error! new anim object already in active object list.");
			}
			gameObject.SetActive(true);
			UiAnimation component = gameObject.GetComponent<UiAnimation>();
			component.Reset();
			component.m_ID = this.GetNextID();
			return gameObject;
		}
		gameObject = new GameObject();
		if (createForInit)
		{
			animDatum.m_availableObjects.Push(gameObject);
		}
		else if (!animDatum.m_activeObjects.Contains(gameObject))
		{
			animDatum.m_activeObjects.Add(gameObject);
		}
		else
		{
			Debug.Log("Error! new anim object already in active object list.");
		}
		CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
		canvasGroup.blocksRaycasts = false;
		canvasGroup.interactable = false;
		gameObject.name = animName;
		UiAnimation nextID = gameObject.AddComponent<UiAnimation>();
		nextID.m_ID = this.GetNextID();
		nextID.Deserialize(animName);
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nextID.GetFrameWidth());
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, nextID.GetFrameHeight());
		foreach (UiAnimation.UiTexture value in nextID.m_textures.Values)
		{
			GameObject gameObject1 = new GameObject()
			{
				name = string.Concat(value.m_parentKey, "_Texture")
			};
			gameObject1.transform.SetParent(gameObject.transform, false);
			value.m_image = gameObject1.AddComponent<Image>();
			value.m_image.sprite = value.m_sprite;
			value.m_image.canvasRenderer.SetAlpha(value.m_alpha);
			if (value.m_alphaMode != "ADD")
			{
				value.m_image.material = new Material(UiAnimMgr.instance.m_blendMaterial);
			}
			else
			{
				value.m_image.material = new Material(UiAnimMgr.instance.m_additiveMaterial);
			}
			value.m_image.material.mainTexture = value.m_sprite.texture;
			RectTransform vector2 = gameObject1.GetComponent<RectTransform>();
			if (value.m_anchor != null && value.m_anchor.relativePoint != null)
			{
				mAnchor = value.m_anchor.relativePoint;
				if (mAnchor != null)
				{
					if (UiAnimMgr.<>f__switch$mapC == null)
					{
						strs = new Dictionary<string, int>(9)
						{
							{ "TOP", 0 },
							{ "BOTTOM", 1 },
							{ "LEFT", 2 },
							{ "RIGHT", 3 },
							{ "CENTER", 4 },
							{ "TOPLEFT", 5 },
							{ "TOPRIGHT", 6 },
							{ "BOTTOMLEFT", 7 },
							{ "BOTTOMRIGHT", 8 }
						};
						UiAnimMgr.<>f__switch$mapC = strs;
					}
					if (UiAnimMgr.<>f__switch$mapC.TryGetValue(mAnchor, out num))
					{
						switch (num)
						{
							case 0:
							{
								vector2.anchorMin = new Vector2(0.5f, 1f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 1:
							{
								vector2.anchorMin = new Vector2(0.5f, 0f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 2:
							{
								vector2.anchorMin = new Vector2(0f, 0.5f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 3:
							{
								vector2.anchorMin = new Vector2(1f, 0.5f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 4:
							{
								vector2.anchorMin = new Vector2(0.5f, 0.5f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 5:
							{
								vector2.anchorMin = new Vector2(0f, 1f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 6:
							{
								vector2.anchorMin = new Vector2(1f, 1f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 7:
							{
								vector2.anchorMin = new Vector2(0f, 0f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
							case 8:
							{
								vector2.anchorMin = new Vector2(1f, 0f);
								vector2.anchorMax = vector2.anchorMin;
								goto Label0;
							}
						}
					}
				}
			}
		Label0:
			Vector2 vector21 = new Vector2();
			if (value.m_anchor != null && value.m_anchor.point != null)
			{
				mAnchor = value.m_anchor.point;
				if (mAnchor != null)
				{
					if (UiAnimMgr.<>f__switch$mapD == null)
					{
						strs = new Dictionary<string, int>(9)
						{
							{ "TOP", 0 },
							{ "BOTTOM", 1 },
							{ "LEFT", 2 },
							{ "RIGHT", 3 },
							{ "CENTER", 4 },
							{ "TOPLEFT", 5 },
							{ "TOPRIGHT", 6 },
							{ "BOTTOMLEFT", 7 },
							{ "BOTTOMRIGHT", 8 }
						};
						UiAnimMgr.<>f__switch$mapD = strs;
					}
					if (UiAnimMgr.<>f__switch$mapD.TryGetValue(mAnchor, out num))
					{
						switch (num)
						{
							case 0:
							{
								Rect mImage = value.m_image.sprite.rect;
								vector21.Set(0f, -0.5f * mImage.height);
								goto Label1;
							}
							case 1:
							{
								Rect rect = value.m_image.sprite.rect;
								vector21.Set(0f, 0.5f * rect.height);
								goto Label1;
							}
							case 2:
							{
								Rect mImage1 = value.m_image.sprite.rect;
								vector21.Set(0.5f * mImage1.width, 0f);
								goto Label1;
							}
							case 3:
							{
								Rect rect1 = value.m_image.sprite.rect;
								vector21.Set(-0.5f * rect1.width, 0f);
								goto Label1;
							}
							case 4:
							{
								goto Label1;
							}
							case 5:
							{
								float single = 0.5f * value.m_image.sprite.rect.width;
								Rect mImage2 = value.m_image.sprite.rect;
								vector21.Set(single, -0.5f * mImage2.height);
								goto Label1;
							}
							case 6:
							{
								float single1 = -0.5f * value.m_image.sprite.rect.width;
								Rect rect2 = value.m_image.sprite.rect;
								vector21.Set(single1, -0.5f * rect2.height);
								goto Label1;
							}
							case 7:
							{
								float single2 = 0.5f * value.m_image.sprite.rect.width;
								Rect mImage3 = value.m_image.sprite.rect;
								vector21.Set(single2, 0.5f * mImage3.height);
								goto Label1;
							}
							case 8:
							{
								float single3 = -0.5f * value.m_image.sprite.rect.width;
								Rect rect3 = value.m_image.sprite.rect;
								vector21.Set(single3, 0.5f * rect3.height);
								goto Label1;
							}
						}
					}
				}
			}
		Label1:
			vector2.anchoredPosition = new Vector2(vector21.x + value.m_anchor.x, vector21.y + value.m_anchor.y);
			int num1 = 0;
			int num2 = 0;
			int.TryParse(value.m_width, out num1);
			int.TryParse(value.m_height, out num2);
			vector2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (num1 <= 0 ? value.m_image.sprite.rect.width : (float)num1));
			vector2.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (num2 <= 0 ? value.m_image.sprite.rect.height : (float)num2));
		}
		foreach (UiAnimation.UiTexture uiTexture in nextID.m_textures.Values)
		{
			uiTexture.m_localPosition = uiTexture.m_image.rectTransform.localPosition;
		}
		return gameObject;
	}

	private int GetNextID()
	{
		this.m_idIndex++;
		return this.m_idIndex;
	}

	public int GetNumActiveAnims()
	{
		int count = 0;
		foreach (KeyValuePair<string, UiAnimMgr.AnimData> mAnimDatum in this.m_animData)
		{
			count += mAnimDatum.Value.m_activeObjects.Count;
		}
		return count;
	}

	public int GetNumAvailableAnims()
	{
		int count = 0;
		foreach (KeyValuePair<string, UiAnimMgr.AnimData> mAnimDatum in this.m_animData)
		{
			count += mAnimDatum.Value.m_availableObjects.Count;
		}
		return count;
	}

	public TextAsset GetSourceData(string key)
	{
		UiAnimMgr.AnimData animDatum = null;
		this.m_animData.TryGetValue(key, out animDatum);
		if (animDatum == null)
		{
			return null;
		}
		return animDatum.m_sourceData;
	}

	private void InitAnimMgr()
	{
		unsafe
		{
			if (UiAnimMgr.s_initialized)
			{
				Debug.Log("Warning: AnimMgr already initialized.");
				return;
			}
			this.m_parentObj = new GameObject()
			{
				name = "UiAnimMgr Parent"
			};
			this.m_additiveMaterial = Resources.Load("Materials/UiAdditive") as Material;
			this.m_blendMaterial = Resources.Load("Materials/UiBlend") as Material;
			this.m_animData = new Dictionary<string, UiAnimMgr.AnimData>();
			TextAsset[] textAssetArray = Resources.LoadAll<TextAsset>("UiAnimations");
			for (uint i = 0; (ulong)i < (long)((int)textAssetArray.Length); i++)
			{
				UiAnimMgr.AnimData animDatum = new UiAnimMgr.AnimData()
				{
					m_sourceData = textAssetArray[i],
					m_animName = textAssetArray[i].name,
					m_activeObjects = new List<GameObject>(),
					m_availableObjects = new Stack<GameObject>()
				};
				this.m_animData.Add(textAssetArray[i].name, animDatum);
				GameObject gameObject = this.CreateAnimObj(textAssetArray[i].name, true);
				gameObject.SetActive(false);
				gameObject.transform.SetParent(this.m_parentObj.transform);
			}
			this.m_idIndex = 0;
			UiAnimMgr.s_initialized = true;
		}
	}

	public UiAnimMgr.UiAnimHandle PlayAnim(string animName, Transform parent, Vector3 localPos, float localScale, float fadeTime = 0f)
	{
		GameObject vector3 = this.CreateAnimObj(animName, false);
		if (vector3 == null)
		{
			return null;
		}
		vector3.transform.SetParent(parent, false);
		vector3.transform.localPosition = localPos;
		vector3.transform.localScale = new Vector3(localScale, localScale, localScale);
		UiAnimation component = vector3.GetComponent<UiAnimation>();
		component.Play(fadeTime);
		return new UiAnimMgr.UiAnimHandle(component);
	}

	private class AnimData
	{
		public TextAsset m_sourceData;

		public string m_animName;

		public List<GameObject> m_activeObjects;

		public Stack<GameObject> m_availableObjects;

		public AnimData()
		{
		}
	}

	public class UiAnimHandle
	{
		private UiAnimation m_anim;

		private int m_ID;

		public UiAnimHandle(UiAnimation anim)
		{
			this.m_anim = anim;
			this.m_ID = anim.m_ID;
		}

		public UiAnimation GetAnim()
		{
			if (this.m_anim == null)
			{
				return null;
			}
			if (this.m_anim.m_ID != this.m_ID)
			{
				return null;
			}
			return this.m_anim;
		}
	}
}