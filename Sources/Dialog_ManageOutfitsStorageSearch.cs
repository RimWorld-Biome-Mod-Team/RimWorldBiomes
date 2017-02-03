using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace StorageSearch
{
	public class Dialog_ManageOutfitsStorageSearch : Window
	{
		private const float TopAreaHeight = 40f;

		private const float TopButtonHeight = 35f;

		private const float TopButtonWidth = 150f;

		private static ThingFilter _apparelGlobalFilter;

		private static readonly Regex ValidNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");

		private Vector2 _scrollPosition;

		private Outfit _selOutfitInt;

		private string searchText = "";

		private bool isFocused;

		private Outfit SelectedOutfit
		{
			get
			{
				return this._selOutfitInt;
			}
			set
			{
				this.CheckSelectedOutfitHasName();
				this._selOutfitInt = value;
			}
		}

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(700f, 700f);
			}
		}

		public Dialog_ManageOutfitsStorageSearch(Outfit selectedOutfit)
		{
			this.forcePause = true;
			this.doCloseX = true;
			this.closeOnEscapeKey = true;
			this.doCloseButton = true;
			this.closeOnClickedOutside = true;
			this.absorbInputAroundWindow = true;
			if (Dialog_ManageOutfitsStorageSearch._apparelGlobalFilter == null)
			{
				Dialog_ManageOutfitsStorageSearch._apparelGlobalFilter = new ThingFilter();
				Dialog_ManageOutfitsStorageSearch._apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
			}
			this.SelectedOutfit = selectedOutfit;
		}

		private void CheckSelectedOutfitHasName()
		{
			if (this.SelectedOutfit != null && this.SelectedOutfit.label.NullOrEmpty())
			{
				this.SelectedOutfit.label = "Unnamed";
			}
		}

		[Detour(typeof(Dialog_ManageOutfits), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
		public override void DoWindowContents(Rect inRect)
		{
			float num = 0f;
			Rect rect = new Rect(0f, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.ButtonText(rect, "SelectOutfit".Translate(), true, false, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (Outfit current in Current.Game.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current;
					list.Add(new FloatMenuOption(localOut.label, delegate
					{
						this.SelectedOutfit = localOut;
					}, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
			num += 10f;
			Rect arg_10C_0 = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.ButtonText(arg_10C_0, "NewOutfit".Translate(), true, false, true))
			{
				this.SelectedOutfit = Current.Game.outfitDatabase.MakeNewOutfit();
			}
			num += 10f;
			Rect arg_15A_0 = new Rect(num, 0f, 150f, 35f);
			num += 150f;
			if (Widgets.ButtonText(arg_15A_0, "DeleteOutfit".Translate(), true, false, true))
			{
				List<FloatMenuOption> list2 = new List<FloatMenuOption>();
				foreach (Outfit current2 in Current.Game.outfitDatabase.AllOutfits)
				{
					Outfit localOut = current2;
					list2.Add(new FloatMenuOption(localOut.label, delegate
					{
						AcceptanceReport acceptanceReport = Current.Game.outfitDatabase.TryDelete(localOut);
						if (!acceptanceReport.Accepted)
						{
							Messages.Message(acceptanceReport.Reason, MessageSound.RejectInput);
							return;
						}
						if (localOut == this.SelectedOutfit)
						{
							this.SelectedOutfit = null;
						}
					}, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				Find.WindowStack.Add(new FloatMenu(list2));
			}
			Rect rect2 = new Rect(0f, 40f, 300f, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
			if (this.SelectedOutfit == null)
			{
				GUI.color = Color.grey;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect2, "NoOutfitSelected".Translate());
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
				return;
			}
			GUI.BeginGroup(rect2);
			Rect rect3 = new Rect(0f, 0f, 180f, 30f);
			Dialog_ManageOutfitsStorageSearch.DoNameInputRect(rect3, ref this.SelectedOutfit.label, 30);
			bool arg_403_0 = Widgets.ButtonImage(new Rect(rect2.width - 20f, 7.5f, 14f, 14f), Widgets.CheckboxOffTex);
			Rect arg_347_0 = new Rect(rect3.width + 10f, 0f, rect2.width - rect3.width - 10f, 29f);
			string text = (this.searchText != string.Empty || this.isFocused) ? this.searchText : "Search";
			bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
			bool flag2 = !Mouse.IsOver(arg_347_0) && Event.current.type == EventType.MouseDown;
			if (!this.isFocused)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.6f);
			}
			GUI.SetNextControlName("StorageSearchInput");
			string text2 = Widgets.TextField(arg_347_0, text);
			GUI.color = Color.white;
			if (this.isFocused)
			{
				this.searchText = text2;
			}
			if ((GUI.GetNameOfFocusedControl() == "StorageSearchInput" || this.isFocused) && (flag | flag2))
			{
				GUIUtility.keyboardControl = 0;
				this.isFocused = false;
			}
			else if (GUI.GetNameOfFocusedControl() == "StorageSearchInput" && !this.isFocused)
			{
				this.isFocused = true;
			}
			if (arg_403_0)
			{
				this.searchText = string.Empty;
			}
			UIHighlighter.HighlightOpportunity(rect, "StoragePriority");
			Rect arg_488_0 = new Rect(0f, 40f, rect2.width, rect2.height - 45f - 10f);
			if (Dialog_ManageOutfitsStorageSearch._apparelGlobalFilter == null)
			{
				Dialog_ManageOutfitsStorageSearch._apparelGlobalFilter = new ThingFilter();
				Dialog_ManageOutfitsStorageSearch._apparelGlobalFilter.SetAllow(ThingCategoryDefOf.Apparel, true);
			}
			ThingFilter apparelGlobalFilter = Dialog_ManageOutfitsStorageSearch._apparelGlobalFilter;
			HelperThingFilterUI.DoThingFilterConfigWindow(arg_488_0, ref this._scrollPosition, this.SelectedOutfit.filter, apparelGlobalFilter, 8, null, null, this.searchText);
			GUI.EndGroup();
			rect2 = new Rect(300f, 40f, inRect.width - 300f, inRect.height - 40f - this.CloseButSize.y).ContractedBy(10f);
			GUI.BeginGroup(rect2);
			new Rect(0f, 40f, rect2.width, rect2.height - 45f - 10f);
			GUI.EndGroup();
		}

		public override void PreClose()
		{
			base.PreClose();
			this.CheckSelectedOutfitHasName();
		}

		private static void DoNameInputRect(Rect rect, ref string name, int maxLength)
		{
			string text = Widgets.TextField(rect, name);
			if (text.Length <= maxLength && Dialog_ManageOutfitsStorageSearch.ValidNameRegex.IsMatch(text))
			{
				name = text;
			}
		}
	}
}
