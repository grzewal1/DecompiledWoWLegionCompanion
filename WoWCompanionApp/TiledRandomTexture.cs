using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class TiledRandomTexture : UIBehaviour
	{
		public RandomTexture randomTexturePrefab;

		public TiledRandomTexture.TilingDirection tilingDirection;

		public TiledRandomTexture.Rotation rotation;

		private List<RandomTexture> textures;

		private int updateCounter;

		public TiledRandomTexture()
		{
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			RectTransform rectTransform = base.transform as RectTransform;
			float single = (this.tilingDirection != TiledRandomTexture.TilingDirection.Horizontal ? rectTransform.rect.height : rectTransform.rect.width);
			float single1 = 0f;
			if (this.tilingDirection != TiledRandomTexture.TilingDirection.Horizontal)
			{
				single1 = (this.rotation == TiledRandomTexture.Rotation.Clockwise90 || this.rotation == TiledRandomTexture.Rotation.CounterClockwise90 ? this.randomTexturePrefab.Width : this.randomTexturePrefab.Height);
			}
			else
			{
				single1 = (this.rotation == TiledRandomTexture.Rotation.Clockwise90 || this.rotation == TiledRandomTexture.Rotation.CounterClockwise90 ? this.randomTexturePrefab.Height : this.randomTexturePrefab.Width);
			}
			int num = Mathf.CeilToInt(single / single1);
			this.textures = base.GetComponentsInChildren<RandomTexture>().ToList<RandomTexture>();
			for (int i = this.textures.Count - 1; i >= num && i >= 0; i--)
			{
				UnityEngine.Object.Destroy(this.textures[i].gameObject);
			}
			for (int j = this.textures.Count; j < num; j++)
			{
				UnityEngine.Object obj = UnityEngine.Object.Instantiate(this.randomTexturePrefab, base.transform, false);
				obj.name = string.Concat(this.randomTexturePrefab.GetType().Name, " ", j);
			}
			for (int k = 0; k < this.textures.Count; k++)
			{
				this.textures[k].RotateImage((float)this.rotation);
			}
		}

		protected override void Start()
		{
			base.Start();
			Vector2 vector2 = (base.transform as RectTransform).sizeDelta;
			Vector2 vector21 = (this.randomTexturePrefab.transform as RectTransform).sizeDelta;
			if (this.tilingDirection != TiledRandomTexture.TilingDirection.Horizontal)
			{
				if (this.rotation == TiledRandomTexture.Rotation.Clockwise90 || this.rotation == TiledRandomTexture.Rotation.CounterClockwise90)
				{
					vector2.x = vector21.y;
				}
				else
				{
					vector2.x = vector21.x;
				}
				LayoutGroup component = base.gameObject.GetComponent<LayoutGroup>();
				if (component == null)
				{
					component = base.gameObject.AddComponent<VerticalLayoutGroup>();
				}
				LayoutGroup layoutGroup = component;
				layoutGroup.childAlignment = TextAnchor.UpperCenter;
				layoutGroup.enabled = true;
			}
			else
			{
				if (this.rotation == TiledRandomTexture.Rotation.Clockwise90 || this.rotation == TiledRandomTexture.Rotation.CounterClockwise90)
				{
					vector2.y = vector21.x;
				}
				else
				{
					vector2.y = vector21.y;
				}
				LayoutGroup component1 = base.gameObject.GetComponent<LayoutGroup>();
				if (component1 == null)
				{
					component1 = base.gameObject.AddComponent<HorizontalLayoutGroup>();
				}
				LayoutGroup layoutGroup1 = component1;
				layoutGroup1.childAlignment = TextAnchor.MiddleLeft;
				layoutGroup1.enabled = true;
			}
			(base.transform as RectTransform).sizeDelta = vector2;
		}

		private void Update()
		{
			if (this.updateCounter < 5)
			{
				this.OnRectTransformDimensionsChange();
				this.updateCounter++;
			}
		}

		public enum Rotation
		{
			None = 0,
			Clockwise90 = 90,
			Rotate180 = 180,
			CounterClockwise90 = 270
		}

		public enum TilingDirection
		{
			Horizontal,
			Vertical
		}
	}
}