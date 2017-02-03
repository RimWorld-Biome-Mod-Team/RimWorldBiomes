using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace StorageSearch
{
	internal class _ITab_Bills : ITab
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly _ITab_Bills.<>c <>9 = new _ITab_Bills.<>c();

			public static Func<RecipeDef, BodyPartGroupDef> <>9__6_1;

			public static Func<RecipeDef, string> <>9__6_2;

			public static Func<RecipeDef, bool> <>9__6_3;

			public static Func<RecipeDef, bool> <>9__6_7;

			internal BodyPartGroupDef <FillTab>b__6_1(RecipeDef x)
			{
				if (x == null)
				{
					return null;
				}
				ThingCountClass expr_11 = x.products[0];
				if (expr_11 == null)
				{
					return null;
				}
				ThingDef expr_1C = expr_11.thingDef;
				if (expr_1C == null)
				{
					return null;
				}
				ApparelProperties expr_27 = expr_1C.apparel;
				if (expr_27 == null)
				{
					return null;
				}
				List<BodyPartGroupDef> expr_32 = expr_27.bodyPartGroups;
				if (expr_32 == null)
				{
					return null;
				}
				return expr_32[0];
			}

			internal string <FillTab>b__6_2(RecipeDef x)
			{
				if (x == null)
				{
					return null;
				}
				return x.LabelCap;
			}

			internal bool <FillTab>b__6_3(RecipeDef recipeDef)
			{
				return recipeDef != null && recipeDef.AvailableNow;
			}

			internal bool <FillTab>b__6_7(RecipeDef recipeDef)
			{
				return recipeDef.AvailableNow;
			}
		}

		private Bill mouseoverBill;

		private static readonly Vector2 WinSize = new Vector2(370f, 480f);

		private Vector2 scrollPosition;

		private float viewHeight = 1000f;

		protected Building_WorkTable SelTable
		{
			get
			{
				return (Building_WorkTable)base.SelThing;
			}
		}

		protected override void FillTab()
		{
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.BillsTab, KnowledgeAmount.FrameDisplayed);
			Rect rect = new Rect(0f, 0f, _ITab_Bills.WinSize.x, _ITab_Bills.WinSize.y).ContractedBy(10f);
			Func<List<FloatMenuOption>> recipeOptionsMaker = delegate
			{
				List<FloatMenuOption> list;
				if (this.SelTable.def.defName.Equals("HandTailoringBench") || this.SelTable.def.defName.Equals("ElectricTailoringBench"))
				{
					IEnumerable<RecipeDef> arg_6A_0 = this.SelTable.def.AllRecipes;
					Func<RecipeDef, BodyPartGroupDef> arg_6A_1;
					if ((arg_6A_1 = _ITab_Bills.<>c.<>9__6_1) == null)
					{
						arg_6A_1 = (_ITab_Bills.<>c.<>9__6_1 = new Func<RecipeDef, BodyPartGroupDef>(_ITab_Bills.<>c.<>9.<FillTab>b__6_1));
					}
					IOrderedEnumerable<RecipeDef> arg_8E_0 = arg_6A_0.OrderByDescending(arg_6A_1);
					Func<RecipeDef, string> arg_8E_1;
					if ((arg_8E_1 = _ITab_Bills.<>c.<>9__6_2) == null)
					{
						arg_8E_1 = (_ITab_Bills.<>c.<>9__6_2 = new Func<RecipeDef, string>(_ITab_Bills.<>c.<>9.<FillTab>b__6_2));
					}
					IEnumerable<RecipeDef> arg_B2_0 = arg_8E_0.ThenBy(arg_8E_1);
					Func<RecipeDef, bool> arg_B2_1;
					if ((arg_B2_1 = _ITab_Bills.<>c.<>9__6_3) == null)
					{
						arg_B2_1 = (_ITab_Bills.<>c.<>9__6_3 = new Func<RecipeDef, bool>(_ITab_Bills.<>c.<>9.<FillTab>b__6_3));
					}
					list = (from recipe in arg_B2_0.Where(arg_B2_1)
					select new FloatMenuOption(recipe.LabelCap, delegate
					{
						if (!this.SelTable.Map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
						{
							Bill.CreateNoPawnsWithSkillDialog(recipe);
						}
						Bill bill = recipe.MakeNewBill();
						this.SelTable.billStack.AddBill(bill);
						if (recipe.conceptLearned != null)
						{
							PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
						}
						if (TutorSystem.TutorialMode)
						{
							TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap);
						}
					}, MenuOptionPriority.Default, null, null, 0f, null, null)).ToList<FloatMenuOption>();
				}
				else
				{
					IEnumerable<RecipeDef> arg_FF_0 = this.SelTable.def.AllRecipes;
					Func<RecipeDef, bool> arg_FF_1;
					if ((arg_FF_1 = _ITab_Bills.<>c.<>9__6_7) == null)
					{
						arg_FF_1 = (_ITab_Bills.<>c.<>9__6_7 = new Func<RecipeDef, bool>(_ITab_Bills.<>c.<>9.<FillTab>b__6_7));
					}
					list = (from recipe in arg_FF_0.Where(arg_FF_1)
					select new FloatMenuOption(recipe.LabelCap, delegate
					{
						if (!this.SelTable.Map.mapPawns.FreeColonists.Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
						{
							Bill.CreateNoPawnsWithSkillDialog(recipe);
						}
						Bill bill = recipe.MakeNewBill();
						this.SelTable.billStack.AddBill(bill);
						if (recipe.conceptLearned != null)
						{
							PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
						}
						if (TutorSystem.TutorialMode)
						{
							TutorSystem.Notify_Event("AddBill-" + recipe.LabelCap);
						}
					}, MenuOptionPriority.Default, null, null, 0f, null, null)).ToList<FloatMenuOption>();
				}
				if (!list.Any<FloatMenuOption>())
				{
					list.Add(new FloatMenuOption("NoneBrackets".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				return list;
			};
			this.mouseoverBill = this.SelTable.billStack.DoListing(rect, recipeOptionsMaker, ref this.scrollPosition, ref this.viewHeight);
		}

		[Detour(typeof(ITab_Bills), bindingFlags = (BindingFlags.Instance | BindingFlags.Public))]
		public override void TabUpdate()
		{
			if (this.mouseoverBill != null)
			{
				this.mouseoverBill.TryDrawIngredientSearchRadiusOnMap(this.SelTable.Position);
				this.mouseoverBill = null;
			}
		}
	}
}
