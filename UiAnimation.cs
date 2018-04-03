using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using WowStaticData;

public class UiAnimation : MonoBehaviour
{
	public Dictionary<string, UiAnimation.UiTexture> m_textures = new Dictionary<string, UiAnimation.UiTexture>();

	private List<UiAnimation.UiAnimGroup> m_groups = new List<UiAnimation.UiAnimGroup>();

	private UiAnimation.UiFrame m_frame;

	private UiAnimation.State m_state;

	private float m_fadeTime;

	private float m_fadeStart;

	public int m_ID;

	private float m_fadeAlphaScalar = 1f;

	public UiAnimation()
	{
	}

	public void Deserialize(string animName)
	{
		UiAnimation.UiTexture uiTexture;
		UiAnimation.UiAnchor array;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(UiAnimation.UiSourceAnimation));
		xmlSerializer.UnknownNode += new XmlNodeEventHandler(this.serializer_UnknownNode);
		xmlSerializer.UnknownAttribute += new XmlAttributeEventHandler(this.serializer_UnknownAttribute);
		TextAsset sourceData = UiAnimMgr.instance.GetSourceData(animName);
		if (sourceData == null)
		{
			Debug.Log(string.Concat("Could not find asset ", animName));
			return;
		}
		MemoryStream memoryStream = new MemoryStream(sourceData.bytes);
		UiAnimation.UiSourceAnimation uiSourceAnimation = xmlSerializer.Deserialize(memoryStream) as UiAnimation.UiSourceAnimation;
		memoryStream.Close();
		if (uiSourceAnimation == null)
		{
			Debug.Log("No ui animation.");
			return;
		}
		this.m_frame = uiSourceAnimation.frame;
		foreach (UiAnimation.UiSourceAnimGroup group in uiSourceAnimation.frame.animation.groups)
		{
			UiAnimation.UiAnimGroup uiAnimGroup = new UiAnimation.UiAnimGroup()
			{
				m_parentKey = group.parentKey,
				m_bounceBack = false
			};
			if (group.looping == null)
			{
				uiAnimGroup.m_looping = false;
				uiAnimGroup.m_bounce = false;
			}
			else if (group.looping == "REPEAT")
			{
				uiAnimGroup.m_looping = true;
				uiAnimGroup.m_bounce = false;
			}
			else if (group.looping == "BOUNCE")
			{
				uiAnimGroup.m_looping = true;
				uiAnimGroup.m_bounce = true;
			}
			foreach (UiAnimation.UiScale mScale in group.m_scales)
			{
				if (mScale.m_childKey == null)
				{
					continue;
				}
				mScale.SetSmoothing();
				uiAnimGroup.m_elements.Add(mScale);
			}
			foreach (UiAnimation.UiAlpha mAlpha in group.m_alphas)
			{
				if (mAlpha.m_childKey == null)
				{
					continue;
				}
				mAlpha.SetSmoothing();
				uiAnimGroup.m_elements.Add(mAlpha);
			}
			foreach (UiAnimation.UiRotation mRotation in group.m_rotations)
			{
				if (mRotation.m_childKey == null)
				{
					continue;
				}
				mRotation.SetSmoothing();
				uiAnimGroup.m_elements.Add(mRotation);
			}
			foreach (UiAnimation.UiTranslation mTranslation in group.m_translations)
			{
				if (mTranslation.m_childKey == null)
				{
					continue;
				}
				mTranslation.SetSmoothing();
				uiAnimGroup.m_elements.Add(mTranslation);
			}
			this.m_groups.Add(uiAnimGroup);
		}
		foreach (UiAnimation.UiLayer layer in uiSourceAnimation.frame.layers)
		{
			foreach (UiAnimation.UiSourceTexture texture in layer.textures)
			{
				if (texture.m_parentKey != null)
				{
					this.m_textures.TryGetValue(texture.m_parentKey, out uiTexture);
					if (uiTexture == null)
					{
						int d = 0;
						StaticDB.uiTextureAtlasMemberDB.EnumRecords((UiTextureAtlasMemberRec memberRec) => {
							if (memberRec.CommittedName == null || texture.m_atlas == null || !(memberRec.CommittedName.ToLower() == texture.m_atlas.ToLower()))
							{
								return true;
							}
							d = memberRec.ID;
							return false;
						});
						Sprite sprite = null;
						if (d > 0)
						{
							sprite = TextureAtlas.GetSprite(d);
						}
						else if (texture.m_resourceImage != null)
						{
							sprite = Resources.Load<Sprite>(texture.m_resourceImage);
						}
						if (sprite == null)
						{
							Debug.Log(string.Concat(new object[] { "Could not find sprite for textureAtlasMemberID ", d, " resourceImage ", texture.m_resourceImage, " in Ui Animation ", animName }));
						}
						else
						{
							UiAnimation.UiTexture mAtlas = new UiAnimation.UiTexture()
							{
								m_alpha = texture.m_alpha,
								m_alphaMode = texture.m_alphaMode
							};
							UiAnimation.UiTexture uiTexture1 = mAtlas;
							if (texture.m_anchors.Count <= 0)
							{
								array = null;
							}
							else
							{
								array = texture.m_anchors.ToArray()[0];
							}
							uiTexture1.m_anchor = array;
							mAtlas.m_atlas = texture.m_atlas;
							mAtlas.m_resourceImage = texture.m_resourceImage;
							mAtlas.m_width = texture.m_width;
							mAtlas.m_height = texture.m_height;
							mAtlas.m_hidden = texture.m_hidden;
							mAtlas.m_parentKey = texture.m_parentKey;
							mAtlas.m_sprite = sprite;
							this.m_textures.Add(texture.m_parentKey, mAtlas);
						}
					}
					else
					{
						Debug.Log(string.Concat("Found duplicate texture ", texture.m_parentKey));
					}
				}
			}
		}
		List<UiAnimation.UiAnimElement> uiAnimElements = new List<UiAnimation.UiAnimElement>();
		foreach (UiAnimation.UiAnimGroup mGroup in this.m_groups)
		{
			mGroup.m_maxTime = 0f;
			foreach (UiAnimation.UiAnimElement mElement in mGroup.m_elements)
			{
				UiAnimation.UiTexture uiTexture2 = null;
				this.m_textures.TryGetValue(mElement.m_childKey, out uiTexture2);
				if (uiTexture2 == null)
				{
					uiAnimElements.Add(mElement);
					Debug.Log(string.Concat("Removing element with childKey ", mElement.m_childKey, ", no associated texture was found."));
				}
				else
				{
					mElement.m_texture = uiTexture2;
					float totalTime = mElement.GetTotalTime();
					if (totalTime > mGroup.m_maxTime)
					{
						mGroup.m_maxTime = totalTime;
					}
				}
			}
			foreach (UiAnimation.UiAnimElement uiAnimElement in uiAnimElements)
			{
				mGroup.m_elements.Remove(uiAnimElement);
			}
		}
	}

	public float GetFrameHeight()
	{
		return this.m_frame.size.y;
	}

	public float GetFrameWidth()
	{
		return this.m_frame.size.x;
	}

	private bool IsTextureReferenced(string parentKey)
	{
		bool flag;
		List<UiAnimation.UiAnimGroup>.Enumerator enumerator = this.m_groups.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				List<UiAnimation.UiAnimElement>.Enumerator enumerator1 = enumerator.Current.m_elements.GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						if (enumerator1.Current.m_childKey != parentKey)
						{
							continue;
						}
						flag = true;
						return flag;
					}
				}
				finally
				{
					((IDisposable)enumerator1).Dispose();
				}
			}
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public void Play(float fadeTime = 0f)
	{
		switch (this.m_state)
		{
			case UiAnimation.State.Stopped:
			case UiAnimation.State.Stopping:
			{
				this.Reset();
				break;
			}
			case UiAnimation.State.Playing:
			{
				return;
			}
		}
		this.m_state = UiAnimation.State.Playing;
		this.m_fadeTime = fadeTime;
		if (fadeTime > 0f)
		{
			this.m_fadeAlphaScalar = 0f;
			this.m_fadeStart = Time.timeSinceLevelLoad;
			this.Update();
		}
	}

	public void Reset()
	{
		foreach (UiAnimation.UiAnimGroup mGroup in this.m_groups)
		{
			mGroup.Reset();
		}
	}

	private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
	{
	}

	private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
	{
	}

	public void Stop(float fadeTime = 0f)
	{
		if (fadeTime <= Mathf.Epsilon)
		{
			this.m_state = UiAnimation.State.Stopped;
			this.m_fadeTime = 0f;
			UiAnimMgr.instance.AnimComplete(this);
		}
		else
		{
			this.m_state = UiAnimation.State.Stopping;
			this.m_fadeTime = fadeTime;
			this.m_fadeStart = Time.timeSinceLevelLoad;
		}
	}

	private void Update()
	{
		if (this.m_state != UiAnimation.State.Playing && this.m_state != UiAnimation.State.Stopping)
		{
			return;
		}
		bool flag = true;
		foreach (UiAnimation.UiAnimGroup mGroup in this.m_groups)
		{
			if (mGroup.Update(this.m_state == UiAnimation.State.Stopping))
			{
				continue;
			}
			flag = false;
		}
		bool flag1 = false;
		if (this.m_state == UiAnimation.State.Playing && this.m_fadeTime > 0f)
		{
			float mFadeStart = (Time.timeSinceLevelLoad - this.m_fadeStart) / this.m_fadeTime;
			if (mFadeStart >= 1f)
			{
				this.m_fadeTime = 0f;
				mFadeStart = 1f;
			}
			flag1 = true;
			this.m_fadeAlphaScalar = mFadeStart;
		}
		if (this.m_state == UiAnimation.State.Stopping)
		{
			flag = false;
			float single = (Time.timeSinceLevelLoad - this.m_fadeStart) / this.m_fadeTime;
			if (single >= 1f)
			{
				single = 1f;
				flag = true;
			}
			single = 1f - single;
			this.m_fadeAlphaScalar = single;
			flag1 = true;
		}
		if (flag1)
		{
			foreach (UiAnimation.UiTexture value in this.m_textures.Values)
			{
				value.m_image.canvasRenderer.SetAlpha(value.m_alpha * this.m_fadeAlphaScalar);
			}
		}
		if (flag)
		{
			this.m_state = UiAnimation.State.Stopped;
			this.m_fadeTime = 0f;
			UiAnimMgr.instance.AnimComplete(this);
		}
	}

	private enum State
	{
		Stopped,
		Stopping,
		Paused,
		Playing
	}

	public class UiAlpha : UiAnimation.UiAnimElement
	{
		[XmlAttribute("fromAlpha")]
		public float m_fromAlpha;

		[XmlAttribute("toAlpha")]
		public float m_toAlpha;

		public UiAlpha()
		{
		}

		public override void Reset()
		{
		}

		public override void Update(float elapsedTime, float maxTime, bool reverse)
		{
			bool flag;
			float unitProgress = base.GetUnitProgress(elapsedTime, maxTime, reverse, out flag);
			if (!flag)
			{
				return;
			}
			float mFromAlpha = this.m_fromAlpha + (this.m_toAlpha - this.m_fromAlpha) * unitProgress;
			((UiAnimation.UiTexture)this.m_texture).m_image.canvasRenderer.SetAlpha(mFromAlpha);
		}
	}

	public class UiAnchor
	{
		[XmlAttribute("point")]
		public string point;

		[XmlAttribute("relativePoint")]
		public string relativePoint;

		[XmlAttribute("x")]
		public float x;

		[XmlAttribute("y")]
		public float y;

		public UiAnchor()
		{
		}
	}

	public class UiAnim
	{
		[XmlElement("AnimationGroup")]
		public List<UiAnimation.UiSourceAnimGroup> groups;

		public UiAnim()
		{
		}
	}

	public abstract class UiAnimElement
	{
		[XmlAttribute("childKey")]
		public string m_childKey;

		[XmlAttribute("smoothing")]
		public string m_smoothing;

		[XmlAttribute("duration")]
		public float m_duration;

		[XmlAttribute("startDelay")]
		public float m_startDelay;

		[XmlAttribute("order")]
		public int m_order;

		public object m_texture;

		public bool m_smoothIn;

		public bool m_smoothOut;

		protected UiAnimElement()
		{
		}

		public float GetTotalTime()
		{
			return this.m_startDelay + this.m_duration;
		}

		public float GetUnitProgress(float elapsedTime, float maxTime, bool reverse, out bool update)
		{
			float single;
			update = true;
			if (!reverse)
			{
				single = elapsedTime - this.m_startDelay;
				if (single < 0f)
				{
					update = false;
					return 0f;
				}
				if (single > this.m_duration)
				{
					update = false;
					return 1f;
				}
			}
			else
			{
				float single1 = maxTime - (this.m_startDelay + this.m_duration);
				single = elapsedTime - single1;
				if (single < 0f)
				{
					update = false;
					return 0f;
				}
				if (single > this.m_duration)
				{
					update = false;
					return 1f;
				}
			}
			if (single <= 0f)
			{
				return 0f;
			}
			if (single < Mathf.Epsilon)
			{
				return 1f;
			}
			float mDuration = single / this.m_duration;
			mDuration = Mathf.Clamp01(mDuration);
			if (!this.m_smoothIn && !this.m_smoothOut)
			{
				if (reverse)
				{
					mDuration = 1f - mDuration;
				}
				return mDuration;
			}
			if (this.m_smoothIn && mDuration <= 0.5f)
			{
				mDuration = 0.5f * (1f + Mathf.Sin((1f - 2f * mDuration) * -0.5f * 3.14159274f));
			}
			else if (this.m_smoothOut && mDuration > 0.5f)
			{
				mDuration = 0.5f + 0.5f * Mathf.Sin(2f * (mDuration - 0.5f) * 0.5f * 3.14159274f);
			}
			mDuration = Mathf.Clamp01(mDuration);
			if (reverse)
			{
				mDuration = 1f - mDuration;
			}
			return mDuration;
		}

		public abstract void Reset();

		public void SetSmoothing()
		{
			if (this.m_smoothing == "IN")
			{
				this.m_smoothIn = true;
				this.m_smoothOut = false;
			}
			else if (this.m_smoothing == "OUT")
			{
				this.m_smoothIn = false;
				this.m_smoothOut = true;
			}
			else if (this.m_smoothing != "IN_OUT")
			{
				this.m_smoothIn = false;
				this.m_smoothOut = false;
			}
			else
			{
				this.m_smoothIn = true;
				this.m_smoothOut = true;
			}
		}

		public abstract void Update(float elapsedTime, float maxTime, bool reverse);
	}

	public class UiAnimGroup
	{
		public string m_parentKey;

		public bool m_looping;

		public bool m_bounce;

		public bool m_bounceBack;

		public float m_startTime;

		public float m_maxTime;

		public List<UiAnimation.UiAnimElement> m_elements;

		public UiAnimGroup()
		{
		}

		public void Reset()
		{
			this.m_startTime = Time.timeSinceLevelLoad;
			this.m_bounceBack = false;
			foreach (UiAnimation.UiAnimElement mElement in this.m_elements)
			{
				mElement.Reset();
			}
		}

		public bool Update(bool stopping)
		{
			float mStartTime = Time.timeSinceLevelLoad - this.m_startTime;
			foreach (UiAnimation.UiAnimElement mElement in this.m_elements)
			{
				if (!stopping || !(mElement is UiAnimation.UiAlpha))
				{
					mElement.Update(mStartTime, this.m_maxTime, (!this.m_bounce ? false : this.m_bounceBack));
				}
			}
			if (mStartTime < this.m_maxTime || !this.m_looping)
			{
				return mStartTime >= this.m_maxTime;
			}
			this.m_startTime = Time.timeSinceLevelLoad;
			if (!this.m_bounce)
			{
				this.Reset();
			}
			else
			{
				this.m_bounceBack = !this.m_bounceBack;
			}
			return false;
		}
	}

	public class UiFrame
	{
		[XmlAttribute("hidden")]
		public bool hidden;

		[XmlAttribute("parent")]
		public string parent;

		[XmlAttribute("parentKey")]
		public string parentKey;

		[XmlAttribute("alpha")]
		public float alpha;

		[XmlElement("Size")]
		public UiAnimation.UiSize size;

		[XmlArray("Layers")]
		[XmlArrayItem("Layer")]
		public List<UiAnimation.UiLayer> layers;

		[XmlElement("Animations")]
		public UiAnimation.UiAnim animation;

		[XmlAttribute("name")]
		public string name;

		public UiFrame()
		{
		}
	}

	public class UiLayer
	{
		[XmlAttribute("level")]
		public string level;

		[XmlElement("Texture")]
		public List<UiAnimation.UiSourceTexture> textures;

		public UiLayer()
		{
		}
	}

	public class UiRotation : UiAnimation.UiAnimElement
	{
		[XmlAttribute("degrees")]
		public float m_degrees;

		public UiRotation()
		{
		}

		public override void Reset()
		{
		}

		public override void Update(float elapsedTime, float maxTime, bool reverse)
		{
			bool flag;
			float unitProgress = base.GetUnitProgress(elapsedTime, maxTime, reverse, out flag);
			if (!flag)
			{
				return;
			}
			UiAnimation.UiTexture mTexture = (UiAnimation.UiTexture)this.m_texture;
			Quaternion mImage = mTexture.m_image.transform.localRotation;
			Vector3 mDegrees = mImage.eulerAngles;
			mDegrees.z = this.m_degrees * unitProgress;
			mImage.eulerAngles = mDegrees;
			mTexture.m_image.transform.localRotation = mImage;
		}
	}

	public class UiScale : UiAnimation.UiAnimElement
	{
		[XmlAttribute("fromScaleX")]
		public float m_fromScaleX;

		[XmlAttribute("toScaleX")]
		public float m_toScaleX;

		[XmlAttribute("fromScaleY")]
		public float m_fromScaleY;

		[XmlAttribute("toScaleY")]
		public float m_toScaleY;

		public UiScale()
		{
		}

		public override void Reset()
		{
		}

		public override void Update(float elapsedTime, float maxTime, bool reverse)
		{
			bool flag;
			Vector3 mFromScaleX = new Vector3();
			float unitProgress = base.GetUnitProgress(elapsedTime, maxTime, reverse, out flag);
			if (!flag)
			{
				return;
			}
			mFromScaleX.x = this.m_fromScaleX + (this.m_toScaleX - this.m_fromScaleX) * unitProgress;
			mFromScaleX.y = this.m_fromScaleY + (this.m_toScaleY - this.m_fromScaleY) * unitProgress;
			mFromScaleX.z = 1f;
			((UiAnimation.UiTexture)this.m_texture).m_image.transform.localScale = mFromScaleX;
		}
	}

	public class UiSize
	{
		[XmlAttribute("x")]
		public float x;

		[XmlAttribute("y")]
		public float y;

		public UiSize()
		{
		}
	}

	[XmlRoot("Ui")]
	public class UiSourceAnimation
	{
		[XmlElement("Frame")]
		public UiAnimation.UiFrame frame;

		public UiSourceAnimation()
		{
		}
	}

	public class UiSourceAnimGroup
	{
		[XmlAttribute("parentKey")]
		public string parentKey;

		[XmlAttribute("looping")]
		public string looping;

		[XmlElement("Alpha")]
		public List<UiAnimation.UiAlpha> m_alphas;

		[XmlElement("Scale")]
		public List<UiAnimation.UiScale> m_scales;

		[XmlElement("Rotation")]
		public List<UiAnimation.UiRotation> m_rotations;

		[XmlElement("Translation")]
		public List<UiAnimation.UiTranslation> m_translations;

		public UiSourceAnimGroup()
		{
		}
	}

	public class UiSourceTexture
	{
		[XmlAttribute("parentKey")]
		public string m_parentKey;

		[XmlAttribute("hidden")]
		public bool m_hidden;

		[XmlAttribute("alpha")]
		public float m_alpha;

		[XmlAttribute("alphaMode")]
		public string m_alphaMode;

		[XmlAttribute("atlas")]
		public string m_atlas;

		[XmlAttribute("useAtlasSize")]
		public bool m_useAtlasSize;

		[XmlAttribute("resourceImage")]
		public string m_resourceImage;

		[XmlAttribute("w")]
		public string m_width;

		[XmlAttribute("h")]
		public string m_height;

		[XmlArray("Anchors")]
		[XmlArrayItem("Anchor")]
		public List<UiAnimation.UiAnchor> m_anchors;

		public UiSourceTexture()
		{
		}
	}

	public class UiTexture
	{
		public string m_parentKey;

		public bool m_hidden;

		public float m_alpha;

		public string m_alphaMode;

		public string m_atlas;

		public bool m_useAtlasSize;

		public string m_resourceImage;

		public string m_width;

		public string m_height;

		public UiAnimation.UiAnchor m_anchor;

		public int m_textureAtlasMemberID;

		public Sprite m_sprite;

		public Image m_image;

		public Vector2 m_localPosition;

		public UiTexture()
		{
		}
	}

	public class UiTranslation : UiAnimation.UiAnimElement
	{
		[XmlAttribute("offsetX")]
		public float m_offsetX;

		[XmlAttribute("offsetY")]
		public float m_offsetY;

		public UiTranslation()
		{
		}

		public override void Reset()
		{
			UiAnimation.UiTexture mTexture = (UiAnimation.UiTexture)this.m_texture;
			Vector2 mLocalPosition = mTexture.m_localPosition;
			mTexture.m_image.rectTransform.localPosition = mLocalPosition;
		}

		public override void Update(float elapsedTime, float maxTime, bool reverse)
		{
			bool flag;
			float unitProgress = base.GetUnitProgress(elapsedTime, maxTime, reverse, out flag);
			if (!flag)
			{
				return;
			}
			UiAnimation.UiTexture mTexture = (UiAnimation.UiTexture)this.m_texture;
			RectTransform mImage = mTexture.m_image.rectTransform;
			Vector2 mLocalPosition = mTexture.m_localPosition;
			mLocalPosition.x = mLocalPosition.x + this.m_offsetX * unitProgress;
			mLocalPosition.y = mLocalPosition.y + this.m_offsetY * unitProgress;
			mImage.localPosition = mLocalPosition;
		}
	}
}