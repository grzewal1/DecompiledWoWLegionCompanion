using System;

namespace WoWCompanionApp
{
	public static class Scenes
	{
		private const string COMPANION_SCENE = "CompanionMainScene";

		private const string LEGION_SCENE = "LegionMainScene";

		public readonly static string TitleSceneName;

		public readonly static string MainSceneName;

		static Scenes()
		{
			Scenes.TitleSceneName = "TitleScene";
			Scenes.MainSceneName = "CompanionMainScene";
		}
	}
}