using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using Verse;

namespace AlienRace
{
	public static class AlienPawnGenerator
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct PawnGenerationStatus
		{
			public Pawn Pawn
			{
				get;
				private set;
			}

			public List<Pawn> PawnsGeneratedInTheMeantime
			{
				get;
				private set;
			}

			public PawnGenerationStatus(Pawn pawn, List<Pawn> pawnsGeneratedInTheMeantime)
			{
				this.Pawn = pawn;
				this.PawnsGeneratedInTheMeantime = pawnsGeneratedInTheMeantime;
			}
		}

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly AlienPawnGenerator.<>c <>9 = new AlienPawnGenerator.<>c();

			public static Func<Thingdef_AlienRace, bool> <>9__5_0;

			public static Func<Pawn, float> <>9__5_2;

			public static Func<TraitDegreeData, float> <>9__18_0;

			public static Func<Backstory, bool> <>9__25_0;

			internal bool <GeneratePawn>b__5_0(Thingdef_AlienRace x)
			{
				return !x.defName.Contains("Base");
			}

			internal float <GeneratePawn>b__5_2(Pawn x)
			{
				return AlienPawnGenerator.WorldPawnSelectionWeight(x);
			}

			internal float <RandomTraitDegree>b__18_0(TraitDegreeData dd)
			{
				return dd.Commonality;
			}

			internal bool <FinalLevelOfSkill>b__25_0(Backstory bs)
			{
				return bs != null;
			}
		}

		public const int MaxStartMentalStateThreshold = 40;

		private static List<AlienPawnGenerator.PawnGenerationStatus> pawnsBeingGenerated = new List<AlienPawnGenerator.PawnGenerationStatus>();

		private static SimpleCurve DefaultAgeGenerationCurve = new SimpleCurve
		{
			new CurvePoint(0.05f, 0f),
			new CurvePoint(0.1f, 100f),
			new CurvePoint(0.675f, 100f),
			new CurvePoint(0.75f, 30f),
			new CurvePoint(0.875f, 18f),
			new CurvePoint(1f, 10f),
			new CurvePoint(1.125f, 3f),
			new CurvePoint(1.25f, 0f)
		};

		private static readonly SimpleCurve LevelRandomCurve = new SimpleCurve
		{
			new CurvePoint(0f, 0f),
			new CurvePoint(0.5f, 150f),
			new CurvePoint(4f, 150f),
			new CurvePoint(5f, 25f),
			new CurvePoint(10f, 5f),
			new CurvePoint(15f, 0f)
		};

		private static readonly SimpleCurve LevelFinalAdjustmentCurve = new SimpleCurve
		{
			new CurvePoint(0f, 0f),
			new CurvePoint(10f, 10f),
			new CurvePoint(20f, 16f),
			new CurvePoint(27f, 20f)
		};

		private static readonly SimpleCurve AgeSkillMaxFactorCurve = new SimpleCurve
		{
			new CurvePoint(0f, 0f),
			new CurvePoint(10f, 0.7f),
			new CurvePoint(35f, 1f),
			new CurvePoint(60f, 1.6f)
		};

		public static Pawn GeneratePawn(PawnKindDef kindDef, Faction faction = null)
		{
			return AlienPawnGenerator.GeneratePawn(new PawnGenerationRequest(kindDef, faction, PawnGenerationContext.NonPlayer, null, false, false, false, false, true, false, 1f, false, true, true, null, null, null, null, null, null));
		}

		public static Pawn GeneratePawn(PawnGenerationRequest request)
		{
			bool flag = request.KindDef != null && request.KindDef == PawnKindDefOf.SpaceRefugee && Rand.Value > 0.6f;
			if (flag)
			{
				PawnKindDef arg_79_0 = request.KindDef;
				IEnumerable<Thingdef_AlienRace> arg_6F_0 = DefDatabase<Thingdef_AlienRace>.AllDefs;
				Func<Thingdef_AlienRace, bool> arg_6F_1;
				if ((arg_6F_1 = AlienPawnGenerator.<>c.<>9__5_0) == null)
				{
					arg_6F_1 = (AlienPawnGenerator.<>c.<>9__5_0 = new Func<Thingdef_AlienRace, bool>(AlienPawnGenerator.<>c.<>9.<GeneratePawn>b__5_0));
				}
				arg_79_0.race = arg_6F_0.Where(arg_6F_1).RandomElement<Thingdef_AlienRace>();
			}
			request.EnsureNonNullFaction();
			Pawn pawn = null;
			bool flag2 = !request.Newborn && !request.ForceGenerateNewPawn && Rand.Value < AlienPawnGenerator.ChanceToRedressAnyWorldPawn();
			if (flag2)
			{
				IEnumerable<Pawn> enumerable = Find.WorldPawns.GetPawnsBySituation(WorldPawnSituation.Free);
				bool factionLeader = request.KindDef.factionLeader;
				if (factionLeader)
				{
					enumerable = enumerable.Concat(Find.WorldPawns.GetPawnsBySituation(WorldPawnSituation.FactionLeader));
				}
				enumerable = from x in enumerable
				where AlienPawnGenerator.IsValidCandidateToRedress(x, request)
				select x;
				IEnumerable<Pawn> arg_12E_0 = enumerable;
				Func<Pawn, float> arg_12E_1;
				if ((arg_12E_1 = AlienPawnGenerator.<>c.<>9__5_2) == null)
				{
					arg_12E_1 = (AlienPawnGenerator.<>c.<>9__5_2 = new Func<Pawn, float>(AlienPawnGenerator.<>c.<>9.<GeneratePawn>b__5_2));
				}
				bool flag3 = arg_12E_0.TryRandomElementByWeight(arg_12E_1, out pawn);
				if (flag3)
				{
					PawnGenerator.RedressPawn(pawn, request);
					Find.WorldPawns.RemovePawn(pawn);
				}
			}
			bool flag4 = pawn == null;
			Pawn result;
			if (flag4)
			{
				pawn = AlienPawnGenerator.GenerateNewNakedPawn(ref request);
				bool flag5 = pawn == null;
				if (flag5)
				{
					result = null;
					return result;
				}
				bool flag6 = !request.Newborn;
				if (flag6)
				{
					bool flag7 = pawn.story != null && pawn.story.bodyType == BodyType.Undefined;
					if (flag7)
					{
						bool flag8 = pawn.story.adulthood != null;
						if (flag8)
						{
							pawn.story.bodyType = pawn.story.adulthood.BodyTypeFor(pawn.gender);
						}
						bool flag9 = pawn.story.bodyType == BodyType.Undefined && pawn.story.adulthood != null;
						if (flag9)
						{
							pawn.story.bodyType = pawn.story.childhood.BodyTypeFor(pawn.gender);
						}
						pawn.story.bodyType = ((pawn.gender == Gender.Male) ? BodyType.Male : BodyType.Female);
						bool flag10 = pawn.story.bodyType == BodyType.Undefined;
						if (flag10)
						{
							pawn.story.bodyType = BodyType.Thin;
						}
					}
					AlienPawnGenerator.GenerateGearFor(pawn, request);
				}
			}
			bool flag11 = Find.Scenario != null;
			if (flag11)
			{
				Find.Scenario.Notify_PawnGenerated(pawn, request.Context);
			}
			bool flag12 = pawn.kindDef.race.ToString().Contains("Alien_") && !(pawn is AlienPawn);
			if (flag12)
			{
				pawn = AlienPawn.GeneratePawn(pawn);
			}
			result = ((pawn as AlienPawn) ?? pawn);
			return result;
		}

		private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
		{
			bool result;
			while (toCheck != null && toCheck != typeof(object))
			{
				Type type = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				bool flag = generic == type;
				if (flag)
				{
					result = false;
					return result;
				}
				toCheck = toCheck.BaseType;
			}
			result = true;
			return result;
		}

		public static void RedressPawn(Pawn pawn, PawnGenerationRequest request)
		{
			pawn.kindDef = request.KindDef;
			AlienPawnGenerator.GenerateGearFor(pawn, request);
		}

		public static bool IsBeingGenerated(Pawn pawn)
		{
			return AlienPawnGenerator.pawnsBeingGenerated.Any((AlienPawnGenerator.PawnGenerationStatus x) => x.Pawn == pawn);
		}

		private static bool IsValidCandidateToRedress(Pawn pawn, PawnGenerationRequest request)
		{
			bool flag = pawn.def != request.KindDef.race;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = pawn.Faction != request.Faction;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !request.AllowDead && (pawn.Dead || pawn.Destroyed);
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = !request.AllowDowned && pawn.Downed;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = pawn.health.hediffSet.BleedRateTotal > 0.001f;
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = !request.CanGeneratePawnRelations && pawn.RaceProps.IsFlesh && pawn.relations.RelatedToAnyoneOrAnyoneRelatedToMe;
								if (flag6)
								{
									result = false;
								}
								else
								{
									bool flag7 = !request.AllowGay && pawn.RaceProps.Humanlike && pawn.story.traits.HasTrait(TraitDefOf.Gay);
									if (flag7)
									{
										result = false;
									}
									else
									{
										bool flag8 = request.Validator != null && !request.Validator(pawn);
										if (flag8)
										{
											result = false;
										}
										else
										{
											bool flag9 = request.FixedBiologicalAge.HasValue && pawn.ageTracker.AgeBiologicalYearsFloat != request.FixedBiologicalAge;
											if (flag9)
											{
												result = false;
											}
											else
											{
												bool flag10 = request.FixedChronologicalAge.HasValue && (float)pawn.ageTracker.AgeChronologicalYears != request.FixedChronologicalAge;
												if (flag10)
												{
													result = false;
												}
												else
												{
													bool flag11 = request.FixedGender.HasValue && pawn.gender != request.FixedGender;
													if (flag11)
													{
														result = false;
													}
													else
													{
														bool flag12 = request.FixedLastName != null && ((NameTriple)pawn.Name).Last != request.FixedLastName;
														if (flag12)
														{
															result = false;
														}
														else
														{
															bool flag13 = request.FixedMelanin.HasValue && pawn.story != null && pawn.story.melanin != request.FixedMelanin;
															if (flag13)
															{
																result = false;
															}
															else
															{
																bool mustBeCapableOfViolence = request.MustBeCapableOfViolence;
																if (mustBeCapableOfViolence)
																{
																	bool flag14 = pawn.story != null && pawn.story.WorkTagIsDisabled(WorkTags.Violent);
																	if (flag14)
																	{
																		result = false;
																		return result;
																	}
																	bool flag15 = pawn.RaceProps.ToolUser && !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
																	if (flag15)
																	{
																		result = false;
																		return result;
																	}
																}
																result = true;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static Pawn GenerateNewNakedPawn(ref PawnGenerationRequest request)
		{
			Pawn pawn = null;
			string text = null;
			bool ignoreScenarioRequirements = false;
			for (int i = 0; i < 100; i++)
			{
				bool flag = i == 70;
				if (flag)
				{
					Log.Error(string.Concat(new object[]
					{
						"Could not generate a pawn after ",
						70,
						" tries. Last error: ",
						text,
						" Ignoring scenario requirements."
					}));
					ignoreScenarioRequirements = true;
				}
				PawnGenerationRequest pawnGenerationRequest = request;
				pawn = AlienPawnGenerator.DoGenerateNewNakedPawn(ref pawnGenerationRequest, out text, ignoreScenarioRequirements);
				bool flag2 = pawn != null;
				if (flag2)
				{
					request = pawnGenerationRequest;
					break;
				}
			}
			bool flag3 = pawn == null;
			Pawn result;
			if (flag3)
			{
				Log.Error(string.Concat(new object[]
				{
					"Pawn generation error: ",
					text,
					" Too many tries (",
					100,
					"), returning null. Generation request: ",
					request
				}));
				result = null;
			}
			else
			{
				result = pawn;
			}
			return result;
		}

		private static Pawn DoGenerateNewNakedPawn(ref PawnGenerationRequest request, out string error, bool ignoreScenarioRequirements)
		{
			error = null;
			Pawn pawn = (Pawn)ThingMaker.MakeThing(request.KindDef.race, null);
			AlienPawnGenerator.pawnsBeingGenerated.Add(new AlienPawnGenerator.PawnGenerationStatus(pawn, null));
			Pawn result;
			try
			{
				pawn.kindDef = request.KindDef;
				pawn.SetFactionDirect(request.Faction);
				PawnComponentsUtility.CreateInitialComponents(pawn);
				bool hasValue = request.FixedGender.HasValue;
				if (hasValue)
				{
					pawn.gender = request.FixedGender.Value;
				}
				else
				{
					bool hasGenders = pawn.RaceProps.hasGenders;
					if (hasGenders)
					{
						bool flag = Rand.Value < 0.5f;
						if (flag)
						{
							pawn.gender = Gender.Male;
						}
						else
						{
							pawn.gender = Gender.Female;
						}
					}
					else
					{
						pawn.gender = Gender.None;
					}
				}
				AlienPawnGenerator.GenerateRandomAge(pawn, request);
				pawn.needs.SetInitialLevels();
				bool flag2 = !request.Newborn && request.CanGeneratePawnRelations;
				if (flag2)
				{
					AlienPawnGenerator.GeneratePawnRelations(pawn, ref request);
				}
				bool humanlike = pawn.RaceProps.Humanlike;
				if (humanlike)
				{
					pawn.story.melanin = ((!request.FixedMelanin.HasValue) ? PawnSkinColors.RandomMelanin() : request.FixedMelanin.Value);
					pawn.story.crownType = ((Rand.Value >= 0.5f) ? CrownType.Narrow : CrownType.Average);
					pawn.story.hairColor = PawnHairColors.RandomHairColor(pawn.story.SkinColor, pawn.ageTracker.AgeBiologicalYears);
					PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo(pawn, request.FixedLastName);
					pawn.story.hairDef = PawnHairChooser.RandomHairDefFor(pawn, request.Faction.def);
					AlienPawnGenerator.GiveRandomTraits(pawn, request.AllowGay);
					bool flag3 = pawn.kindDef.race.ToString().Contains("Alien_");
					if (flag3)
					{
						pawn = AlienPawn.GeneratePawn(pawn);
					}
					AlienPawnGenerator.GenerateSkills(pawn);
				}
				bool flag4 = pawn.workSettings != null && request.Faction.IsPlayer;
				if (flag4)
				{
					pawn.workSettings.EnableAndInitialize();
				}
				bool flag5 = request.Faction != null && pawn.RaceProps.Animal;
				if (flag5)
				{
					pawn.GenerateNecessaryName();
				}
				bool flag6 = !request.AllowDead && (pawn.Dead || pawn.Destroyed);
				if (flag6)
				{
					AlienPawnGenerator.DiscardGeneratedPawn(pawn);
					error = "Generated dead pawn.";
					result = null;
				}
				else
				{
					bool flag7 = !request.AllowDowned && pawn.Downed;
					if (flag7)
					{
						AlienPawnGenerator.DiscardGeneratedPawn(pawn);
						error = "Generated downed pawn.";
						result = null;
					}
					else
					{
						bool flag8 = request.MustBeCapableOfViolence && pawn.story != null && pawn.story.WorkTagIsDisabled(WorkTags.Violent);
						if (flag8)
						{
							AlienPawnGenerator.DiscardGeneratedPawn(pawn);
							error = "Generated pawn incapable of violence.";
							result = null;
						}
						else
						{
							bool flag9 = !ignoreScenarioRequirements && request.Context == PawnGenerationContext.PlayerStarter && !Find.Scenario.AllowPlayerStartingPawn(pawn);
							if (flag9)
							{
								AlienPawnGenerator.DiscardGeneratedPawn(pawn);
								error = "Generated pawn doesn't meet scenario requirements.";
								result = null;
							}
							else
							{
								bool flag10 = request.Validator != null && !request.Validator(pawn);
								if (flag10)
								{
									AlienPawnGenerator.DiscardGeneratedPawn(pawn);
									error = "Generated pawn didn't pass validator check.";
									result = null;
								}
								else
								{
									for (int i = 0; i < AlienPawnGenerator.pawnsBeingGenerated.Count - 1; i++)
									{
										bool flag11 = AlienPawnGenerator.pawnsBeingGenerated[i].PawnsGeneratedInTheMeantime == null;
										if (flag11)
										{
											AlienPawnGenerator.pawnsBeingGenerated[i] = new AlienPawnGenerator.PawnGenerationStatus(AlienPawnGenerator.pawnsBeingGenerated[i].Pawn, new List<Pawn>());
										}
										AlienPawnGenerator.pawnsBeingGenerated[i].PawnsGeneratedInTheMeantime.Add(pawn);
									}
									result = pawn;
								}
							}
						}
					}
				}
			}
			finally
			{
				AlienPawnGenerator.pawnsBeingGenerated.RemoveLast<AlienPawnGenerator.PawnGenerationStatus>();
			}
			return result;
		}

		private static void GenerateSkills(Pawn pawn)
		{
			List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				SkillDef skillDef = allDefsListForReading[i];
				int num = AlienPawnGenerator.FinalLevelOfSkill(pawn, skillDef);
				SkillRecord skill = pawn.skills.GetSkill(skillDef);
				skill.Level = num;
				bool flag = !skill.TotallyDisabled;
				if (flag)
				{
					float num2 = (float)num * 0.11f;
					float value = Rand.Value;
					bool flag2 = value < num2;
					if (flag2)
					{
						bool flag3 = value < num2 * 0.2f;
						if (flag3)
						{
							skill.passion = Passion.Major;
						}
						else
						{
							skill.passion = Passion.Minor;
						}
					}
					skill.xpSinceLastLevel = Rand.Range(skill.XpRequiredForLevelUp * 0.1f, skill.XpRequiredForLevelUp * 0.9f);
				}
			}
		}

		private static void DiscardGeneratedPawn(Pawn pawn)
		{
			bool flag = Find.WorldPawns.Contains(pawn);
			if (flag)
			{
				Find.WorldPawns.RemovePawn(pawn);
			}
			Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
			List<Pawn> pawnsGeneratedInTheMeantime = AlienPawnGenerator.pawnsBeingGenerated.Last<AlienPawnGenerator.PawnGenerationStatus>().PawnsGeneratedInTheMeantime;
			bool flag2 = pawnsGeneratedInTheMeantime != null;
			if (flag2)
			{
				for (int i = 0; i < pawnsGeneratedInTheMeantime.Count; i++)
				{
					Pawn pawn2 = pawnsGeneratedInTheMeantime[i];
					bool flag3 = Find.WorldPawns.Contains(pawn2);
					if (flag3)
					{
						Find.WorldPawns.RemovePawn(pawn2);
					}
					Find.WorldPawns.PassToWorld(pawn2, PawnDiscardDecideMode.Discard);
					for (int j = 0; j < AlienPawnGenerator.pawnsBeingGenerated.Count; j++)
					{
						AlienPawnGenerator.pawnsBeingGenerated[j].PawnsGeneratedInTheMeantime.Remove(pawn2);
					}
				}
			}
		}

		private static float ChanceToRedressAnyWorldPawn()
		{
			int pawnsBySituationCount = Find.WorldPawns.GetPawnsBySituationCount(WorldPawnSituation.Free);
			return Mathf.Min(0.02f + 0.01f * ((float)pawnsBySituationCount / 25f), 0.8f);
		}

		private static float WorldPawnSelectionWeight(Pawn p)
		{
			bool flag = p.RaceProps.IsFlesh && !p.relations.everSeenByPlayer && p.relations.RelatedToAnyoneOrAnyoneRelatedToMe;
			float result;
			if (flag)
			{
				result = 0.1f;
			}
			else
			{
				result = 1f;
			}
			return result;
		}

		private static void GenerateGearFor(Pawn pawn, PawnGenerationRequest request)
		{
			ProfilerThreadCheck.BeginSample("GenerateGearFor");
			ProfilerThreadCheck.BeginSample("GenerateStartingApparelFor");
			PawnApparelGenerator.GenerateStartingApparelFor(pawn, request);
			ProfilerThreadCheck.EndSample();
			ProfilerThreadCheck.BeginSample("TryGenerateWeaponFor");
			PawnWeaponGenerator.TryGenerateWeaponFor(pawn);
			ProfilerThreadCheck.EndSample();
			ProfilerThreadCheck.BeginSample("GenerateInventoryFor");
			PawnInventoryGenerator.GenerateInventoryFor(pawn, request);
			ProfilerThreadCheck.EndSample();
			ProfilerThreadCheck.EndSample();
		}

		private static void GenerateRandomAge(Pawn pawn, PawnGenerationRequest request)
		{
			bool flag = request.FixedBiologicalAge.HasValue && request.FixedChronologicalAge.HasValue;
			if (flag)
			{
				float? fixedBiologicalAge = request.FixedBiologicalAge;
				bool hasValue = fixedBiologicalAge.HasValue;
				bool flag2;
				if (hasValue)
				{
					float? fixedChronologicalAge = request.FixedChronologicalAge;
					bool hasValue2 = fixedChronologicalAge.HasValue;
					if (hasValue2)
					{
						flag2 = (fixedBiologicalAge.Value > fixedChronologicalAge.Value);
						goto IL_6F;
					}
				}
				flag2 = false;
				IL_6F:
				bool flag3 = flag2;
				if (flag3)
				{
					Log.Warning(string.Concat(new object[]
					{
						"Tried to generate age for pawn ",
						pawn,
						", but pawn generation request demands biological age (",
						request.FixedBiologicalAge,
						") to be greater than chronological age (",
						request.FixedChronologicalAge,
						")."
					}));
				}
			}
			bool newborn = request.Newborn;
			if (newborn)
			{
				pawn.ageTracker.AgeBiologicalTicks = 0L;
			}
			else
			{
				bool hasValue3 = request.FixedBiologicalAge.HasValue;
				if (hasValue3)
				{
					pawn.ageTracker.AgeBiologicalTicks = (long)(request.FixedBiologicalAge.Value * 3600000f);
				}
				else
				{
					int num = 0;
					float num2;
					while (true)
					{
						bool flag4 = pawn.RaceProps.ageGenerationCurve != null;
						if (flag4)
						{
							num2 = (float)Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.ageGenerationCurve, 200));
						}
						else
						{
							bool isMechanoid = pawn.RaceProps.IsMechanoid;
							if (isMechanoid)
							{
								num2 = (float)Rand.Range(0, 2500);
							}
							else
							{
								num2 = Rand.ByCurve(AlienPawnGenerator.DefaultAgeGenerationCurve, 200) * pawn.RaceProps.lifeExpectancy;
							}
						}
						num++;
						bool flag5 = num > 300;
						if (flag5)
						{
							break;
						}
						bool flag6 = num2 <= (float)pawn.kindDef.maxGenerationAge && num2 >= (float)pawn.kindDef.minGenerationAge;
						if (flag6)
						{
							goto Block_12;
						}
					}
					Log.Error("Tried 300 times to generate age for " + pawn);
					Block_12:
					pawn.ageTracker.AgeBiologicalTicks = (long)(num2 * 3600000f) + (long)Rand.Range(0, 3600000);
				}
			}
			bool newborn2 = request.Newborn;
			if (newborn2)
			{
				pawn.ageTracker.AgeChronologicalTicks = 0L;
			}
			else
			{
				bool hasValue4 = request.FixedChronologicalAge.HasValue;
				if (hasValue4)
				{
					pawn.ageTracker.AgeChronologicalTicks = (long)(request.FixedChronologicalAge.Value * 3600000f);
				}
				else
				{
					bool flag7 = Rand.Value < pawn.kindDef.backstoryCryptosleepCommonality;
					int num3;
					if (flag7)
					{
						float value = Rand.Value;
						bool flag8 = value < 0.7f;
						if (flag8)
						{
							num3 = Rand.Range(0, 100);
						}
						else
						{
							bool flag9 = value < 0.95f;
							if (flag9)
							{
								num3 = Rand.Range(100, 1000);
							}
							else
							{
								int max = 5500 + GenDate.YearsPassed - 2026 - pawn.ageTracker.AgeBiologicalYears;
								num3 = Rand.Range(1000, max);
							}
						}
					}
					else
					{
						num3 = 0;
					}
					int ticksAbs = GenTicks.TicksAbs;
					long num4 = (long)ticksAbs - pawn.ageTracker.AgeBiologicalTicks;
					num4 -= (long)num3 * 3600000L;
					pawn.ageTracker.BirthAbsTicks = num4;
				}
			}
			bool flag10 = pawn.ageTracker.AgeBiologicalTicks > pawn.ageTracker.AgeChronologicalTicks;
			if (flag10)
			{
				pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
			}
		}

		public static int RandomTraitDegree(TraitDef traitDef)
		{
			bool flag = traitDef.degreeDatas.Count == 1;
			int degree;
			if (flag)
			{
				degree = traitDef.degreeDatas[0].degree;
			}
			else
			{
				IEnumerable<TraitDegreeData> arg_4D_0 = traitDef.degreeDatas;
				Func<TraitDegreeData, float> arg_4D_1;
				if ((arg_4D_1 = AlienPawnGenerator.<>c.<>9__18_0) == null)
				{
					arg_4D_1 = (AlienPawnGenerator.<>c.<>9__18_0 = new Func<TraitDegreeData, float>(AlienPawnGenerator.<>c.<>9.<RandomTraitDegree>b__18_0));
				}
				degree = arg_4D_0.RandomElementByWeight(arg_4D_1).degree;
			}
			return degree;
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
				Backstory expr_122 = pawn.story.adulthood;
				bool flag5 = ((expr_122 != null) ? expr_122.forcedTraits : null) != null;
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
					Trait trait = new Trait(TraitDefOf.Gay, AlienPawnGenerator.RandomTraitDegree(TraitDefOf.Gay), false);
					pawn.story.traits.GainTrait(trait);
				}
				Func<TraitDef, float> <>9__0;
				Predicate<TraitDef> <>9__2;
				while (pawn.story.traits.allTraits.Count < num)
				{
					AlienPawnGenerator.<>c__DisplayClass19_1 <>c__DisplayClass19_2 = new AlienPawnGenerator.<>c__DisplayClass19_1();
					AlienPawnGenerator.<>c__DisplayClass19_1 arg_2A1_0 = <>c__DisplayClass19_2;
					IEnumerable<TraitDef> arg_29C_0 = DefDatabase<TraitDef>.AllDefsListForReading;
					Func<TraitDef, float> arg_29C_1;
					if ((arg_29C_1 = <>9__0) == null)
					{
						arg_29C_1 = (<>9__0 = ((TraitDef tr) => tr.GetGenderSpecificCommonality(pawn)));
					}
					arg_2A1_0.newTraitDef = arg_29C_0.RandomElementByWeight(arg_29C_1);
					bool flag9 = !pawn.story.traits.HasTrait(<>c__DisplayClass19_2.newTraitDef);
					if (flag9)
					{
						bool flag10 = <>c__DisplayClass19_2.newTraitDef == TraitDefOf.Gay;
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
						bool arg_38E_0;
						if (!pawn.story.traits.allTraits.Any((Trait tr) => <>c__DisplayClass19_2.newTraitDef.ConflictsWith(tr)))
						{
							if (<>c__DisplayClass19_2.newTraitDef.conflictingTraits != null)
							{
								List<TraitDef> arg_380_0 = <>c__DisplayClass19_2.newTraitDef.conflictingTraits;
								Predicate<TraitDef> arg_380_1;
								if ((arg_380_1 = <>9__2) == null)
								{
									arg_380_1 = (<>9__2 = ((TraitDef tr) => pawn.story.traits.HasTrait(tr)));
								}
								arg_38E_0 = !arg_380_0.Any(arg_380_1);
							}
							else
							{
								arg_38E_0 = true;
							}
						}
						else
						{
							arg_38E_0 = false;
						}
						bool flag13 = arg_38E_0;
						if (flag13)
						{
							bool flag14 = <>c__DisplayClass19_2.newTraitDef.requiredWorkTypes == null || !pawn.story.OneOfWorkTypesIsDisabled(<>c__DisplayClass19_2.newTraitDef.requiredWorkTypes);
							if (flag14)
							{
								bool flag15 = !pawn.story.WorkTagIsDisabled(<>c__DisplayClass19_2.newTraitDef.requiredWorkTags);
								if (flag15)
								{
									int degree = AlienPawnGenerator.RandomTraitDegree(<>c__DisplayClass19_2.newTraitDef);
									bool flag16 = !pawn.story.childhood.DisallowsTrait(<>c__DisplayClass19_2.newTraitDef, degree) && (pawn.story.adulthood == null || !pawn.story.adulthood.DisallowsTrait(<>c__DisplayClass19_2.newTraitDef, degree));
									if (flag16)
									{
										Trait trait2 = new Trait(<>c__DisplayClass19_2.newTraitDef, degree, false);
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

		public static void PostProcessGeneratedGear(Thing gear, Pawn pawn)
		{
			CompQuality compQuality = gear.TryGetComp<CompQuality>();
			bool flag = compQuality != null;
			if (flag)
			{
				compQuality.SetQuality(QualityUtility.RandomGeneratedGearQuality(pawn.kindDef), ArtGenerationContext.Outsider);
			}
			bool useHitPoints = gear.def.useHitPoints;
			if (useHitPoints)
			{
				float randomInRange = pawn.kindDef.gearHealthRange.RandomInRange;
				bool flag2 = randomInRange < 1f;
				if (flag2)
				{
					int num = Mathf.RoundToInt(randomInRange * (float)gear.MaxHitPoints);
					num = Mathf.Max(1, num);
					gear.HitPoints = num;
				}
			}
		}

		private static void GeneratePawnRelations(Pawn pawn, ref PawnGenerationRequest request)
		{
			bool flag = !pawn.RaceProps.Humanlike;
			if (!flag)
			{
				List<KeyValuePair<Pawn, PawnRelationDef>> list = new List<KeyValuePair<Pawn, PawnRelationDef>>();
				List<PawnRelationDef> allDefsListForReading = DefDatabase<PawnRelationDef>.AllDefsListForReading;
				IEnumerable<Pawn> enumerable = from x in Find.WorldPawns.AllPawnsAliveOrDead
				where x.def == pawn.def
				select x;
				foreach (Pawn current in enumerable)
				{
					bool discarded = current.Discarded;
					if (discarded)
					{
						Log.Warning(string.Concat(new object[]
						{
							"Warning during generating pawn relations for ",
							pawn,
							": Pawn ",
							current,
							" is discarded, yet he was yielded by PawnUtility. Discarding a pawn means that he is no longer managed by anything."
						}));
					}
					else
					{
						for (int i = 0; i < allDefsListForReading.Count; i++)
						{
							bool flag2 = allDefsListForReading[i].generationChanceFactor > 0f;
							if (flag2)
							{
								list.Add(new KeyValuePair<Pawn, PawnRelationDef>(current, allDefsListForReading[i]));
							}
						}
					}
				}
				PawnGenerationRequest localReq = request;
				KeyValuePair<Pawn, PawnRelationDef> keyValuePair = list.RandomElementByWeightWithDefault(delegate(KeyValuePair<Pawn, PawnRelationDef> x)
				{
					bool flag5 = !x.Value.familyByBloodRelation;
					float result;
					if (flag5)
					{
						result = 0f;
					}
					else
					{
						result = x.Value.generationChanceFactor * x.Value.Worker.GenerationChance(pawn, x.Key, localReq);
					}
					return result;
				}, 82f);
				bool flag3 = keyValuePair.Key != null;
				if (flag3)
				{
					keyValuePair.Value.Worker.CreateRelation(pawn, keyValuePair.Key, ref request);
				}
				KeyValuePair<Pawn, PawnRelationDef> keyValuePair2 = list.RandomElementByWeightWithDefault(delegate(KeyValuePair<Pawn, PawnRelationDef> x)
				{
					bool familyByBloodRelation = x.Value.familyByBloodRelation;
					float result;
					if (familyByBloodRelation)
					{
						result = 0f;
					}
					else
					{
						result = x.Value.generationChanceFactor * x.Value.Worker.GenerationChance(pawn, x.Key, localReq);
					}
					return result;
				}, 82f);
				bool flag4 = keyValuePair2.Key != null;
				if (flag4)
				{
					keyValuePair2.Value.Worker.CreateRelation(pawn, keyValuePair2.Key, ref request);
				}
			}
		}

		private static int FinalLevelOfSkill(Pawn pawn, SkillDef sk)
		{
			bool usuallyDefinedInBackstories = sk.usuallyDefinedInBackstories;
			float num;
			if (usuallyDefinedInBackstories)
			{
				num = (float)Rand.RangeInclusive(0, 4);
			}
			else
			{
				num = Rand.ByCurve(AlienPawnGenerator.LevelRandomCurve, 100);
			}
			IEnumerable<Backstory> arg_52_0 = pawn.story.AllBackstories;
			Func<Backstory, bool> arg_52_1;
			if ((arg_52_1 = AlienPawnGenerator.<>c.<>9__25_0) == null)
			{
				arg_52_1 = (AlienPawnGenerator.<>c.<>9__25_0 = new Func<Backstory, bool>(AlienPawnGenerator.<>c.<>9.<FinalLevelOfSkill>b__25_0));
			}
			foreach (Backstory current in arg_52_0.Where(arg_52_1))
			{
				foreach (KeyValuePair<SkillDef, int> current2 in current.skillGainsResolved)
				{
					bool flag = current2.Key == sk;
					if (flag)
					{
						num += (float)current2.Value * Rand.Range(1f, 1.4f);
					}
				}
			}
			for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
			{
				int num2 = 0;
				bool flag2 = pawn.story.traits.allTraits[i].CurrentData.skillGains.TryGetValue(sk, out num2);
				if (flag2)
				{
					num += (float)num2;
				}
			}
			float num3 = Rand.Range(1f, AlienPawnGenerator.AgeSkillMaxFactorCurve.Evaluate((float)pawn.ageTracker.AgeBiologicalYears));
			num *= num3;
			num = AlienPawnGenerator.LevelFinalAdjustmentCurve.Evaluate(num);
			return Mathf.Clamp(Mathf.RoundToInt(num), 0, 20);
		}
	}
}
