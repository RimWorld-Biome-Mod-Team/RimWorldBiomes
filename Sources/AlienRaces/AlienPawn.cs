using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace AlienRace
{
	[StaticConstructorOnStartup]
	public class AlienPawn : Pawn
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly AlienPawn.<>c <>9 = new AlienPawn.<>c();

			public static Func<KeyValuePair<string, Backstory>, Backstory> <>9__29_1;

			internal Backstory <SetBackstoryInSlot>b__29_1(KeyValuePair<string, Backstory> kvp)
			{
				return kvp.Value;
			}
		}

		public string TexPathHair;

		public Color HColor;

		public Color alienskincolor;

		public string headtexpath;

		public string nakedbodytexpath = "Things/Pawn/Humanlike/Bodies/";

		public string dessicatedgraphicpath;

		public string skullgraphicpath;

		private Graphic nakedGraphic;

		private Graphic rottingGraphic;

		private Graphic desiccatedGraphic;

		private Graphic headGraphic;

		private Graphic desiccatedHeadGraphic;

		private Graphic skullGraphic;

		private Graphic hairGraphic;

		public Vector2 DrawSize;

		private List<AlienTraitEntry> fTraits;

		public bool FirstSpawn = false;

		public static int MainThreadId;

		private bool isHeadless;

		public List<Color> PossibleHairColors;

		public List<Color> PossibleSkinColors;

		public bool SpawnedByPC = false;

		public static AlienPawn GeneratePawn(Thing thing)
		{
			AlienPawn alienPawn = (AlienPawn)Convert.ChangeType(thing, typeof(AlienPawn));
			alienPawn.ReadXML(alienPawn);
			return alienPawn;
		}

		public static AlienPawn GeneratePawn(Pawn pawn)
		{
			AlienPawn result;
			try
			{
				AlienPawn alienPawn = (AlienPawn)Convert.ChangeType(pawn, typeof(AlienPawn));
				alienPawn.ReadXML(alienPawn);
				result = alienPawn;
				return result;
			}
			catch (Exception ex)
			{
				Log.Message(string.Concat(new string[]
				{
					ex.Message,
					"\n",
					ex.TargetSite.Name,
					"\n",
					ex.StackTrace
				}));
			}
			result = (pawn as AlienPawn);
			return result;
		}

		private bool CheckStartingColonists(AlienPawn newguy)
		{
			return Find.TickManager.TicksGame < 600;
		}

		public void ReadXML(AlienPawn newguy)
		{
			bool flag = newguy.def.defName.Contains("Alien_");
			if (flag)
			{
				Thingdef_AlienRace thingdef_AlienRace = this.def as Thingdef_AlienRace;
				bool flag2 = thingdef_AlienRace == null;
				if (flag2)
				{
					Log.Message("Could not read Alien ThingDef.");
				}
				bool flag3 = !this.FirstSpawn;
				if (flag3)
				{
					bool flag4 = Find.TickManager.TicksGame > 10 && !this.SpawnedByPC;
					if (flag4)
					{
						bool customGenderDistribution = thingdef_AlienRace.CustomGenderDistribution;
						if (customGenderDistribution)
						{
							bool flag5 = Rand.Value < thingdef_AlienRace.MaleGenderProbability;
							if (flag5)
							{
								this.gender = Gender.Male;
							}
							else
							{
								this.gender = Gender.Female;
							}
						}
					}
					Log.Message("1");
					bool customSkinColors = thingdef_AlienRace.CustomSkinColors;
					if (customSkinColors)
					{
						this.alienskincolor = thingdef_AlienRace.alienskincolorgen.NewRandomizedColor();
					}
					else
					{
						this.alienskincolor = this.story.SkinColor;
					}
					Log.Message("2");
					bool flag6 = thingdef_AlienRace.NakedBodyGraphicLocation.NullOrEmpty();
					if (flag6)
					{
						this.nakedbodytexpath = "Things/Pawn/Humanlike/Bodies/";
					}
					else
					{
						this.nakedbodytexpath = thingdef_AlienRace.NakedBodyGraphicLocation;
					}
					Log.Message("3");
					bool flag7 = thingdef_AlienRace.DesiccatedGraphicLocation.NullOrEmpty();
					if (flag7)
					{
						this.dessicatedgraphicpath = "Things/Pawn/Humanlike/HumanoidDessicated";
					}
					else
					{
						this.dessicatedgraphicpath = thingdef_AlienRace.DesiccatedGraphicLocation;
					}
					Log.Message("4");
					bool flag8 = thingdef_AlienRace.SkullGraphicLocation.NullOrEmpty();
					if (flag8)
					{
						this.skullgraphicpath = "Things/Pawn/Humanlike/Heads/None_Average_Skull";
					}
					else
					{
						this.skullgraphicpath = thingdef_AlienRace.SkullGraphicLocation;
					}
					Log.Message("5");
					bool flag9 = this.story != null;
					if (flag9)
					{
						Log.Message("5.1");
						switch (thingdef_AlienRace.HasHair)
						{
						case AlienHairTypes.Vanilla:
							this.TexPathHair = this.story.hairDef.texPath;
							this.HColor = this.story.hairColor;
							break;
						case AlienHairTypes.None:
							this.HColor = Color.white;
							this.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved", true);
							this.TexPathHair = this.story.hairDef.texPath;
							break;
						case AlienHairTypes.Custom:
						{
							this.TexPathHair = this.story.hairDef.texPath;
							bool flag10 = thingdef_AlienRace.alienhaircolorgen == null;
							if (flag10)
							{
								this.HColor = PawnHairColorsAlien.RandomHairColor(this.alienskincolor, this.ageTracker.AgeBiologicalYears, thingdef_AlienRace.GetsGreyAt);
							}
							else
							{
								this.HColor = thingdef_AlienRace.alienhaircolorgen.NewRandomizedColor();
							}
							break;
						}
						default:
							this.TexPathHair = this.story.hairDef.texPath;
							this.HColor = this.story.hairColor;
							break;
						}
						Log.Message("5.2");
						bool flag11 = !thingdef_AlienRace.Headless;
						if (flag11)
						{
							Log.Message("5.2.1");
							bool flag12 = thingdef_AlienRace.NakedHeadGraphicLocation.NullOrEmpty();
							if (flag12)
							{
								Log.Message("5.2.1.1");
								this.headtexpath = this.story.HeadGraphicPath;
							}
							else
							{
								Log.Message("5.2.1.2");
								Log.Message((thingdef_AlienRace != null).ToString());
								Log.Message((thingdef_AlienRace.alienpartgenerator != null).ToString());
								Log.Message((thingdef_AlienRace.alienpartgenerator.RandomAlienHead(thingdef_AlienRace.NakedHeadGraphicLocation, this.gender) != null).ToString());
								this.headtexpath = thingdef_AlienRace.alienpartgenerator.RandomAlienHead(thingdef_AlienRace.NakedHeadGraphicLocation, this.gender);
								Log.Message("5.2.1.3");
								typeof(Pawn_StoryTracker).GetField("headGraphicPath", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this.story, this.headtexpath);
							}
							Log.Message("5.2.2");
						}
						else
						{
							Log.Message("5.2.3");
							this.isHeadless = true;
						}
						Log.Message("5.3");
						Vector2 arg_407_0 = thingdef_AlienRace.CustomDrawSize;
						bool flag13 = false;
						if (flag13)
						{
							this.DrawSize = Vector2.one;
						}
						else
						{
							this.DrawSize = thingdef_AlienRace.CustomDrawSize;
						}
						Log.Message("5.4");
						bool pawnsSpecificBackstories = thingdef_AlienRace.PawnsSpecificBackstories;
						if (pawnsSpecificBackstories)
						{
							AlienPawn.UpdateBackstories(newguy, newguy.kindDef);
						}
					}
					Log.Message("6");
					bool flag14 = thingdef_AlienRace.ForcedRaceTraitEntries != null && !this.SpawnedByPC;
					if (flag14)
					{
						this.fTraits = thingdef_AlienRace.ForcedRaceTraitEntries;
						List<TraitDef> allDefsListForReading = DefDatabase<TraitDef>.AllDefsListForReading;
						bool flag15 = this.story.childhood.forcedTraits == null;
						if (flag15)
						{
							this.story.childhood.forcedTraits = new List<TraitEntry>();
						}
						for (int i = 0; i < this.fTraits.Count; i++)
						{
							bool flag16 = Rand.RangeInclusive(0, 100) < this.fTraits[i].chance;
							if (flag16)
							{
								foreach (TraitDef current in allDefsListForReading)
								{
									bool flag17 = current.defName == this.fTraits[i].defname;
									if (flag17)
									{
										this.story.childhood.forcedTraits.Add(new TraitEntry(current, this.fTraits[i].degree));
									}
								}
							}
						}
					}
					this.def = thingdef_AlienRace;
					bool flag18 = this.TryGetComp<CompImmuneToAge>() != null;
					if (flag18)
					{
						this.health.hediffSet.Clear();
						PawnTechHediffsGenerator.GeneratePartsAndImplantsFor(this);
					}
				}
				bool flag19 = this.fTraits != null && !this.FirstSpawn;
				if (flag19)
				{
					this.UpdateForcedTraits(newguy);
				}
				this.FirstSpawn = true;
				AlienPawn.MainThreadId = Thread.CurrentThread.ManagedThreadId;
			}
			bool flag20 = this.nakedbodytexpath.Length < 2;
			if (flag20)
			{
				this.nakedbodytexpath = "Things/Pawn/Humanlike/Bodies/";
			}
			bool flag21 = this.headtexpath.Length < 2;
			if (flag21)
			{
				this.headtexpath = this.story.HeadGraphicPath;
			}
		}

		public void SpawnSetupAlien()
		{
			DoOnMainThread.ExecuteOnMainThread.Enqueue(delegate
			{
				this.nakedGraphic = GraphicGetterAlienBody.GetNakedBodyGraphicAlien(this.story.bodyType, ShaderDatabase.Cutout, this.alienskincolor, this.nakedbodytexpath, this.DrawSize);
				this.rottingGraphic = GraphicGetterAlienBody.GetNakedBodyGraphicAlien(this.story.bodyType, ShaderDatabase.CutoutSkin, PawnGraphicSet.RottingColor, this.nakedbodytexpath, this.DrawSize);
				bool flag = !this.isHeadless;
				if (flag)
				{
					this.headGraphic = GraphicDatabase.Get<Graphic_Multi>(this.headtexpath, ShaderDatabase.Cutout, this.DrawSize, this.alienskincolor);
				}
				else
				{
					this.headGraphic = null;
				}
				this.desiccatedGraphic = GraphicDatabase.Get<Graphic_Multi>(this.dessicatedgraphicpath, ShaderDatabase.Cutout, this.DrawSize, PawnGraphicSet.RottingColor);
				this.desiccatedHeadGraphic = GraphicDatabase.Get<Graphic_Multi>(this.headtexpath, ShaderDatabase.Cutout, this.DrawSize, PawnGraphicSet.RottingColor);
				this.skullGraphic = GraphicDatabase.Get<Graphic_Multi>(this.skullgraphicpath, ShaderDatabase.Cutout, this.DrawSize, Color.white);
				this.hairGraphic = GraphicDatabase.Get<Graphic_Multi>(this.TexPathHair, ShaderDatabase.Cutout, this.DrawSize, this.HColor);
				this.UpdateGraphics();
			});
		}

		public void UpdateGraphics()
		{
			base.Drawer.renderer.graphics.headGraphic = this.headGraphic;
			base.Drawer.renderer.graphics.nakedGraphic = this.nakedGraphic;
			base.Drawer.renderer.graphics.hairGraphic = this.hairGraphic;
			base.Drawer.renderer.graphics.rottingGraphic = this.rottingGraphic;
			base.Drawer.renderer.graphics.desiccatedHeadGraphic = this.desiccatedHeadGraphic;
			base.Drawer.renderer.graphics.skullGraphic = this.skullGraphic;
			foreach (Apparel current in this.apparel.WornApparel)
			{
			}
			base.Drawer.renderer.graphics.ResolveApparelGraphics();
		}

		private static void UpdateBackstories(Pawn pawn, PawnKindDef pawntype)
		{
			AlienPawn.SetBackstoryInSlot(pawn, BackstorySlot.Childhood, ref pawn.story.childhood, pawntype);
			AlienPawn.SetBackstoryInSlot(pawn, BackstorySlot.Adulthood, ref pawn.story.adulthood, pawntype);
		}

		private static void SetBackstoryInSlot(Pawn pawn, BackstorySlot slot, ref Backstory backstory, PawnKindDef pawntype)
		{
			IEnumerable<KeyValuePair<string, Backstory>> arg_51_0 = from kvp in BackstoryDatabase.allBackstories
			where kvp.Value.shuffleable && kvp.Value.spawnCategories.Contains(pawntype.backstoryCategory) && kvp.Value.slot == slot && (slot != BackstorySlot.Adulthood || !kvp.Value.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.childhood.workDisables))
			select kvp;
			Func<KeyValuePair<string, Backstory>, Backstory> arg_51_1;
			if ((arg_51_1 = AlienPawn.<>c.<>9__29_1) == null)
			{
				arg_51_1 = (AlienPawn.<>c.<>9__29_1 = new Func<KeyValuePair<string, Backstory>, Backstory>(AlienPawn.<>c.<>9.<SetBackstoryInSlot>b__29_1));
			}
			bool flag = !arg_51_0.Select(arg_51_1).TryRandomElement(out backstory);
			if (flag)
			{
				backstory = (from kvp in BackstoryDatabase.allBackstories
				where kvp.Value.slot == slot
				select kvp).RandomElement<KeyValuePair<string, Backstory>>().Value;
			}
		}

		private void UpdateForcedTraits(AlienPawn apawn)
		{
			apawn.story.traits.allTraits.Clear();
			AlienPawn.GiveRandomTraits(apawn, true);
		}

		private static void GiveRandomTraits(Pawn pawn, bool allowGay)
		{
			bool flag = pawn.story == null;
			if (!flag)
			{
				bool flag2 = pawn.story.childhood.forcedTraits != null;
				if (flag2)
				{
					List<TraitEntry> forcedTraits = pawn.story.childhood.forcedTraits;
					for (int i = 0; i < forcedTraits.Count; i++)
					{
						TraitEntry traitEntry = forcedTraits[i];
						bool flag3 = traitEntry.def == null;
						if (flag3)
						{
							Log.Error("Null forced trait def on " + pawn.story.childhood);
						}
						else
						{
							bool flag4 = !pawn.story.traits.HasTrait(traitEntry.def);
							if (flag4)
							{
								pawn.story.traits.GainTrait(new Trait(traitEntry.def, traitEntry.degree, false));
							}
						}
					}
				}
				bool flag5 = pawn.story.adulthood.forcedTraits != null;
				if (flag5)
				{
					List<TraitEntry> forcedTraits2 = pawn.story.adulthood.forcedTraits;
					for (int j = 0; j < forcedTraits2.Count; j++)
					{
						TraitEntry traitEntry2 = forcedTraits2[j];
						bool flag6 = traitEntry2.def == null;
						if (flag6)
						{
							Log.Error("Null forced trait def on " + pawn.story.adulthood);
						}
						else
						{
							bool flag7 = !pawn.story.traits.HasTrait(traitEntry2.def);
							if (flag7)
							{
								pawn.story.traits.GainTrait(new Trait(traitEntry2.def, traitEntry2.degree, false));
							}
						}
					}
				}
				int num = Rand.RangeInclusive(2, 3);
				bool flag8 = allowGay && (LovePartnerRelationUtility.HasAnyLovePartnerOfTheSameGender(pawn) || LovePartnerRelationUtility.HasAnyExLovePartnerOfTheSameGender(pawn));
				if (flag8)
				{
					Trait trait = new Trait(TraitDefOf.Gay, PawnGenerator.RandomTraitDegree(TraitDefOf.Gay), false);
					pawn.story.traits.GainTrait(trait);
				}
				Func<TraitDef, float> <>9__0;
				Predicate<TraitDef> <>9__2;
				while (pawn.story.traits.allTraits.Count < num)
				{
					AlienPawn.<>c__DisplayClass31_1 <>c__DisplayClass31_2 = new AlienPawn.<>c__DisplayClass31_1();
					AlienPawn.<>c__DisplayClass31_1 arg_29A_0 = <>c__DisplayClass31_2;
					IEnumerable<TraitDef> arg_295_0 = DefDatabase<TraitDef>.AllDefsListForReading;
					Func<TraitDef, float> arg_295_1;
					if ((arg_295_1 = <>9__0) == null)
					{
						arg_295_1 = (<>9__0 = ((TraitDef tr) => tr.GetGenderSpecificCommonality(pawn)));
					}
					arg_29A_0.newTraitDef = arg_295_0.RandomElementByWeight(arg_295_1);
					bool flag9 = !pawn.story.traits.HasTrait(<>c__DisplayClass31_2.newTraitDef);
					if (flag9)
					{
						bool flag10 = <>c__DisplayClass31_2.newTraitDef == TraitDefOf.Gay;
						if (flag10)
						{
							bool flag11 = !allowGay;
							if (flag11)
							{
								continue;
							}
							bool flag12 = LovePartnerRelationUtility.HasAnyLovePartnerOfTheOppositeGender(pawn) || LovePartnerRelationUtility.HasAnyExLovePartnerOfTheOppositeGender(pawn);
							if (flag12)
							{
								continue;
							}
						}
						bool arg_387_0;
						if (!pawn.story.traits.allTraits.Any((Trait tr) => <>c__DisplayClass31_2.newTraitDef.ConflictsWith(tr)))
						{
							if (<>c__DisplayClass31_2.newTraitDef.conflictingTraits != null)
							{
								List<TraitDef> arg_379_0 = <>c__DisplayClass31_2.newTraitDef.conflictingTraits;
								Predicate<TraitDef> arg_379_1;
								if ((arg_379_1 = <>9__2) == null)
								{
									arg_379_1 = (<>9__2 = ((TraitDef tr) => pawn.story.traits.HasTrait(tr)));
								}
								arg_387_0 = !arg_379_0.Any(arg_379_1);
							}
							else
							{
								arg_387_0 = true;
							}
						}
						else
						{
							arg_387_0 = false;
						}
						bool flag13 = arg_387_0;
						if (flag13)
						{
							bool flag14 = <>c__DisplayClass31_2.newTraitDef.requiredWorkTypes == null || !pawn.story.OneOfWorkTypesIsDisabled(<>c__DisplayClass31_2.newTraitDef.requiredWorkTypes);
							if (flag14)
							{
								bool flag15 = !pawn.story.WorkTagIsDisabled(<>c__DisplayClass31_2.newTraitDef.requiredWorkTags);
								if (flag15)
								{
									int degree = PawnGenerator.RandomTraitDegree(<>c__DisplayClass31_2.newTraitDef);
									bool flag16 = !pawn.story.childhood.DisallowsTrait(<>c__DisplayClass31_2.newTraitDef, degree) && !pawn.story.adulthood.DisallowsTrait(<>c__DisplayClass31_2.newTraitDef, degree);
									if (flag16)
									{
										Trait trait2 = new Trait(<>c__DisplayClass31_2.newTraitDef, degree, false);
										bool flag17 = pawn.mindState == null || pawn.mindState.mentalBreaker == null || pawn.mindState.mentalBreaker.BreakThresholdExtreme + trait2.OffsetOfStat(StatDefOf.MentalBreakThreshold) <= 40f;
										if (flag17)
										{
											pawn.story.traits.GainTrait(trait2);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public override void SetFaction(Faction newFaction, Pawn recruiter = null)
		{
			PawnKindDef kindDef = this.kindDef;
			bool flag = newFaction == base.Faction;
			if (flag)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Used ChangePawnFactionTo to change ",
					this,
					" to same faction ",
					newFaction
				}));
			}
			else
			{
				bool flag2 = this.guest != null;
				if (flag2)
				{
					this.guest.SetGuestStatus(null, false);
				}
				base.Map.mapPawns.DeRegisterPawn(this);
				base.Map.pawnDestinationManager.RemovePawnFromSystem(this);
				base.Map.designationManager.RemoveAllDesignationsOn(this, false);
				bool flag3 = newFaction == Faction.OfPlayer || base.Faction == Faction.OfPlayer;
				if (flag3)
				{
					Find.ColonistBar.MarkColonistsDirty();
				}
				Lord lord = this.GetLord();
				bool flag4 = lord != null;
				if (flag4)
				{
					lord.Notify_PawnLost(this, PawnLostCondition.ChangedFaction);
				}
				base.SetFaction(newFaction, null);
				PawnComponentsUtility.AddAndRemoveDynamicComponents(this, false);
				bool flag5 = base.Faction != null && base.Faction.IsPlayer;
				if (flag5)
				{
					bool flag6 = this.workSettings != null;
					if (flag6)
					{
						this.workSettings.EnableAndInitialize();
					}
					Find.Storyteller.intenderPopulation.Notify_PopulationGained();
				}
				bool drafted = base.Drafted;
				if (drafted)
				{
					this.drafter.Drafted = false;
				}
				base.Map.reachability.ClearCache();
				this.health.surgeryBills.Clear();
				bool spawned = base.Spawned;
				if (spawned)
				{
					base.Map.mapPawns.RegisterPawn(this);
				}
				base.GenerateNecessaryName();
				bool flag7 = this.playerSettings != null;
				if (flag7)
				{
					this.playerSettings.medCare = ((!base.RaceProps.Humanlike) ? (this.playerSettings.medCare = MedicalCareCategory.NoMeds) : MedicalCareCategory.Best);
				}
				base.ClearMind(true);
				bool flag8 = !base.Dead && this.needs.mood != null;
				if (flag8)
				{
					this.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
				}
				base.Map.attackTargetsCache.UpdateTarget(this);
				Find.GameEnder.CheckGameOver();
				this.kindDef = kindDef;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.LookDef<ThingDef>(ref this.def, "def");
			Scribe_Values.LookValue<bool>(ref this.FirstSpawn, "FirstSpawn", false, false);
			Scribe_Values.LookValue<string>(ref this.headtexpath, "headtexpath", null, false);
			Scribe_Values.LookValue<string>(ref this.TexPathHair, "TexPathHair", null, false);
			Scribe_Values.LookValue<string>(ref this.nakedbodytexpath, "nakedbodytexpath", null, false);
			Scribe_Values.LookValue<string>(ref this.dessicatedgraphicpath, "dessicatedgraphicpath", null, false);
			Scribe_Values.LookValue<string>(ref this.skullgraphicpath, "skullgraphicpath", null, false);
			Scribe_Values.LookValue<Color>(ref this.alienskincolor, "alienskincolor", default(Color), false);
			Scribe_Values.LookValue<Color>(ref this.HColor, "HColor", default(Color), false);
		}
	}
}
