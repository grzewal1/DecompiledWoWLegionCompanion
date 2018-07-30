using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoWCompanionApp
{
	public class NewsPanel : MonoBehaviour
	{
		public NewsListItem NewsListItemPrefab;

		public GameObject NewsListPanel;

		public GameObject NewsListContent;

		public ArticleView ArticleViewPanel;

		public GameObject ArticleViewContent;

		private NewsService newsService;

		private DateTime updatedTime = DateTime.MinValue;

		public NewsPanel()
		{
		}

		public void CloseArticleView()
		{
			this.ArticleViewPanel.gameObject.SetActive(false);
			this.NewsListPanel.SetActive(true);
		}

		public void ExpandArticleView(NewsArticle article)
		{
			this.NewsListPanel.SetActive(false);
			this.ArticleViewPanel.gameObject.SetActive(true);
			this.ArticleViewPanel.SetArticle(article);
		}

		private void Start()
		{
			this.newsService = base.GetComponent<NewsService>();
		}

		private void Update()
		{
			if (this.newsService != null && this.newsService.FetchTime > this.updatedTime)
			{
				NewsListItem[] componentsInChildren = this.NewsListContent.GetComponentsInChildren<NewsListItem>();
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
				}
				foreach (NewsArticle newsArticle in this.newsService.NewsArticles)
				{
					NewsListItem newsListItem = UnityEngine.Object.Instantiate<NewsListItem>(this.NewsListItemPrefab, this.NewsListContent.transform, false);
					newsListItem.SetNewsArticle(newsArticle);
					newsListItem.newsPanel = this;
				}
				this.updatedTime = this.newsService.FetchTime;
			}
			if (Input.GetKeyDown(KeyCode.Escape) && this.ArticleViewPanel.gameObject.activeSelf)
			{
				this.CloseArticleView();
			}
		}
	}
}