using HaulingHysteresis;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace StorageSearch
{
	public class ITab_Storage_Detour
	{
		private static PropertyInfo SelStoreSettingsParent;

		private static FieldInfo ScrollPosition;

		private static readonly Vector2 WinSize = new Vector2(300f, 480f);

		private const float TopAreaHeight = 35f;

		private static string searchText = "";

		private static bool isFocused;

		public static void Init()
		{
			ITab_Storage_Detour.SelStoreSettingsParent = typeof(ITab_Storage).GetProperty("SelStoreSettingsParent", BindingFlags.Instance | BindingFlags.NonPublic);
			ITab_Storage_Detour.ScrollPosition = typeof(ITab_Storage).GetField("scrollPosition", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static void FillTab(ITab_Storage tab)
		{
			IStoreSettingsParent storeSettingsParent = (IStoreSettingsParent)ITab_Storage_Detour.SelStoreSettingsParent.GetValue(tab, null);
			Debug.Log(storeSettingsParent);
			StorageSettings settings = storeSettingsParent.GetStoreSettings();
			Rect position = new Rect(0f, 0f, ITab_Storage_Detour.WinSize.x, ITab_Storage_Detour.WinSize.y).ContractedBy(10f);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 0f, 160f, 29f);
			if (Widgets.ButtonText(rect, "Priority".Translate() + ": " + settings.Priority.Label(), true, false, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (object current in Enum.GetValues(typeof(StoragePriority)))
				{
					if ((StoragePriority)current > StoragePriority.Unstored)
					{
						StoragePriority localPr = (StoragePriority)current;
						list.Add(new FloatMenuOption(localPr.Label().CapitalizeFirst(), delegate
						{
							settings.Priority = localPr;
						}, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
			bool arg_2B9_0 = Widgets.ButtonImage(new Rect(position.width - 33f, 7.5f, 14f, 14f), Widgets.CheckboxOffTex);
			Rect arg_204_0 = new Rect(165f, 0f, position.width - 160f - 20f, 29f);
			string text = (ITab_Storage_Detour.searchText != string.Empty || ITab_Storage_Detour.isFocused) ? ITab_Storage_Detour.searchText : "SearchLabel".Translate();
			bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
			bool flag2 = !Mouse.IsOver(arg_204_0) && Event.current.type == EventType.MouseDown;
			if (!ITab_Storage_Detour.isFocused)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.6f);
			}
			GUI.SetNextControlName("StorageSearchInput");
			string text2 = Widgets.TextField(arg_204_0, text);
			GUI.color = Color.white;
			if (ITab_Storage_Detour.isFocused)
			{
				ITab_Storage_Detour.searchText = text2;
			}
			if ((GUI.GetNameOfFocusedControl() == "StorageSearchInput" || ITab_Storage_Detour.isFocused) && (flag | flag2))
			{
				GUIUtility.keyboardControl = 0;
				ITab_Storage_Detour.isFocused = false;
			}
			else if (GUI.GetNameOfFocusedControl() == "StorageSearchInput" && !ITab_Storage_Detour.isFocused)
			{
				ITab_Storage_Detour.isFocused = true;
			}
			if (arg_2B9_0)
			{
				ITab_Storage_Detour.searchText = string.Empty;
			}
			UIHighlighter.HighlightOpportunity(rect, "StoragePriority");
			ThingFilter parentFilter = null;
			if (storeSettingsParent.GetParentStoreSettings() != null)
			{
				parentFilter = storeSettingsParent.GetParentStoreSettings().filter;
			}
			Rect arg_334_0 = new Rect(0f, 35f, position.width, position.height - 70f);
			Vector2 vector = (Vector2)ITab_Storage_Detour.ScrollPosition.GetValue(tab);
			HelperThingFilterUI.DoThingFilterConfigWindow(arg_334_0, ref vector, settings.filter, parentFilter, 8, null, null, ITab_Storage_Detour.searchText);
			ITab_Storage_Detour.ScrollPosition.SetValue(tab, vector);
			Rect rect2 = new Rect(0f, position.height - 30f, position.width, 30f);
			StorageSettings_Hysteresis storageSettings_Hysteresis = StorageSettings_Mapping.Get(settings);
			if (storageSettings_Hysteresis == null)
			{
				storageSettings_Hysteresis = new StorageSettings_Hysteresis();
			}
			storageSettings_Hysteresis.FillPercent = Widgets.HorizontalSlider(rect2.LeftPart(0.8f), storageSettings_Hysteresis.FillPercent, 0f, 100f, false, "Refill cells less then", null, null, -1f);
			Widgets.Label(rect2.RightPart(0.2f), storageSettings_Hysteresis.FillPercent.ToString("N0") + "%");
			StorageSettings_Mapping.Set(settings, storageSettings_Hysteresis);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.StorageTab, KnowledgeAmount.FrameDisplayed);
			GUI.EndGroup();
		}
	}
}
