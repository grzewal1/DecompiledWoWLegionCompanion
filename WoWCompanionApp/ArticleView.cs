using System;
using UnityEngine;
using UnityEngine.UI;

namespace WoWCompanionApp
{
	public class ArticleView : MonoBehaviour
	{
		public Text title;

		public Text body;

		public RawImage image;

		public ArticleView()
		{
		}

		public void SetArticle(NewsArticle article)
		{
			this.title.text = article.Title;
			this.body.text = article.Body;
			this.image.texture = article.Image;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}