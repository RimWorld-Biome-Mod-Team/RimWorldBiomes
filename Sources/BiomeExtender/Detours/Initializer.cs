using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace Detours
{
	[StaticConstructorOnStartup]
	internal class Initializer : ITab
	{
		protected static GameObject iconControllerObject;

		static Initializer()
		{
			Log.Message("Initialized Detour SK Core.");
			Initializer.iconControllerObject = new GameObject("SK_Initializer");
			Initializer.iconControllerObject.AddComponent<InitializerBehaviour>();
			Initializer.iconControllerObject.AddComponent<DoOnMainThread>();
			UnityEngine.Object.DontDestroyOnLoad(Initializer.iconControllerObject);
		}

		protected override void FillTab()
		{
		}
	}
}
