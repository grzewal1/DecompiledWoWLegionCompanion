using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	[ExecuteInEditMode]
	public class RandomTexture : MonoBehaviour
	{
		public Sprite[] sprites;

		private Image image;

		public float Height
		{
			get
			{
				return (this.sprites == null || (int)this.sprites.Length <= 0 || !(this.sprites[0] != null) ? (base.transform as RectTransform).rect.height : (float)this.sprites[0].texture.height);
			}
		}

		public float Width
		{
			get
			{
				return (this.sprites == null || (int)this.sprites.Length <= 0 || !(this.sprites[0] != null) ? (base.transform as RectTransform).rect.width : (float)this.sprites[0].texture.width);
			}
		}

		public RandomTexture()
		{
		}

		public void ChangeTexture()
		{
			if (this.sprites == null || (int)this.sprites.Length == 0)
			{
				return;
			}
			this.image.sprite = this.sprites[UnityEngine.Random.Range(0, (int)this.sprites.Length - 1)];
			this.image.preserveAspect = true;
			RectTransform vector2 = base.transform as RectTransform;
			vector2.sizeDelta = new Vector2((float)this.image.sprite.texture.width, (float)this.image.sprite.texture.height);
		}

		public void RotateImage(float degrees)
		{
			if (this.image == null)
			{
				this.image = base.GetComponentInChildren<Image>();
			}
			RectTransform rectTransform = this.image.transform as RectTransform;
			rectTransform.rotation = Quaternion.identity;
			rectTransform.Rotate(Vector3.forward, degrees);
		}

		private void Start()
		{
			this.image = base.GetComponentInChildren<Image>();
			this.ChangeTexture();
		}

		private void Update()
		{
		}
	}
}