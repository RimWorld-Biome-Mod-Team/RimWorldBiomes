using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace StorageSearch
{
	public static class HelperThingFilterUI
	{
		private const float ExtraViewHeight = 90f;

		private const float RangeLabelTab = 10f;

		private const float RangeLabelHeight = 19f;

		private const float SliderHeight = 26f;

		private const float SliderTab = 20f;

		private static float viewHeight;

		public static void DoThingFilterConfigWindow(Rect rect, ref Vector2 scrollPosition, ThingFilter filter, ThingFilter parentFilter = null, int openMask = 1, IEnumerable<ThingDef> forceHiddenDefs = null, IEnumerable<SpecialThingFilterDef> forceHiddenFilters = null, string filterText = null)
		{
			Widgets.DrawMenuSection(rect, true);
			Text.Font = GameFont.Tiny;
			float num = rect.width - 2f;
			Rect rect2 = new Rect(rect.x + 1f, rect.y + 1f, num / 2f, 24f);
			if (Widgets.ButtonText(rect2, "ClearAll".Translate(), true, false, true))
			{
				filter.SetDisallowAll(forceHiddenDefs, forceHiddenFilters);
			}
			if (Widgets.ButtonText(new Rect(rect2.xMax + 1f, rect2.y, num / 2f, 24f), "AllowAll".Translate(), true, false, true))
			{
				filter.SetAllowAll(parentFilter);
			}
			Text.Font = GameFont.Small;
			rect.yMin = rect2.yMax;
			Rect viewRect = new Rect(0f, 0f, rect.width - 16f, HelperThingFilterUI.viewHeight);
			Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
			float num2 = 2f;
			HelperThingFilterUI.DrawHitPointsFilterConfig(ref num2, viewRect.width, filter);
			HelperThingFilterUI.DrawQualityFilterConfig(ref num2, viewRect.width, filter);
			float num3 = num2;
			Listing_TreeThingFilter listing_TreeThingFilter = new Listing_TreeThingFilter(new Rect(0f, num2, viewRect.width, 9999f), filter, parentFilter, forceHiddenDefs, forceHiddenFilters);
			TreeNode_ThingCategory treeNode_ThingCategory = ThingCategoryNodeDatabase.RootNode;
			if (parentFilter != null)
			{
				if (parentFilter.DisplayRootCategory == null)
				{
					parentFilter.RecalculateDisplayRootCategory();
				}
				treeNode_ThingCategory = parentFilter.DisplayRootCategory;
			}
			if (filterText != null && filterText.Length > 0)
			{
				TreeNode_ThingCategory treeNode_ThingCategory2 = new TreeNode_ThingCategory(new ThingCategoryDef());
				from td in treeNode_ThingCategory.catDef.DescendantThingDefs
				where td.label.ToLower().Contains(filterText.ToLower())
				select td;
				IEnumerable<ThingDef> arg_1D5_0 = treeNode_ThingCategory.catDef.DescendantThingDefs;
				Func<ThingDef, bool> <>9__1;
				Func<ThingDef, bool> arg_1D5_1;
				if ((arg_1D5_1 = <>9__1) == null)
				{
					arg_1D5_1 = (<>9__1 = ((ThingDef td) => td.label.ToLower().Contains(filterText.ToLower())));
				}
				foreach (ThingDef current in arg_1D5_0.Where(arg_1D5_1))
				{
					treeNode_ThingCategory2.catDef.childThingDefs.Add(current);
				}
				treeNode_ThingCategory = treeNode_ThingCategory2;
			}
			listing_TreeThingFilter.DoCategoryChildren(treeNode_ThingCategory, 0, openMask, true);
			listing_TreeThingFilter.End();
			if (Event.current.type == EventType.Layout)
			{
				HelperThingFilterUI.viewHeight = num3 + listing_TreeThingFilter.CurHeight + 90f;
			}
			Widgets.EndScrollView();
		}

		private static void DrawHitPointsFilterConfig(ref float y, float width, ThingFilter filter)
		{
			if (!filter.allowedHitPointsConfigurable)
			{
				return;
			}
			Rect arg_3B_0 = new Rect(20f, y, width - 20f, 26f);
			FloatRange allowedHitPointsPercents = filter.AllowedHitPointsPercents;
			Widgets.FloatRange(arg_3B_0, 1, ref allowedHitPointsPercents, 0f, 1f, "HitPoints", ToStringStyle.PercentZero);
			filter.AllowedHitPointsPercents = allowedHitPointsPercents;
			y += 26f;
			y += 5f;
			Text.Font = GameFont.Small;
		}

		private static void DrawQualityFilterConfig(ref float y, float width, ThingFilter filter)
		{
			if (!filter.allowedQualitiesConfigurable)
			{
				return;
			}
			Rect arg_2B_0 = new Rect(20f, y, width - 20f, 26f);
			QualityRange allowedQualityLevels = filter.AllowedQualityLevels;
			Widgets.QualityRange(arg_2B_0, 2, ref allowedQualityLevels);
			filter.AllowedQualityLevels = allowedQualityLevels;
			y += 26f;
			y += 5f;
			Text.Font = GameFont.Small;
		}
	}
}
