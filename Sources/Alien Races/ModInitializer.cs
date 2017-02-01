using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AlienRace
{
	public class ModInitializer : ITab
	{
		protected GameObject modInitializerControllerObject;

		public ModInitializer()
		{
			LongEventHandler.QueueLongEvent(delegate
			{
				this.modInitializerControllerObject = new GameObject("AlienRacer");
				this.modInitializerControllerObject.AddComponent<ModInitializerBehaviour>();
				this.modInitializerControllerObject.AddComponent<DoOnMainThread>();
				UnityEngine.Object.DontDestroyOnLoad(this.modInitializerControllerObject);
			}, "queueInject", false, null);
		}

		protected override void FillTab()
		{
		}
	}
}
