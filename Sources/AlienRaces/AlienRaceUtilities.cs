using RimWorld;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace AlienRace
{
	[StaticConstructorOnStartup]
	public static class AlienRaceUtilities
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly AlienRaceUtilities.<>c <>9 = new AlienRaceUtilities.<>c();

			public static Predicate<StartingColonistEntry> <>9__4_0;

			internal bool <InitializeAlienColonistOptions>b__4_0(StartingColonistEntry x)
			{
				return x.factionDef == Faction.OfPlayer.def;
			}
		}

		public static List<PawnKindDef> AllAlienColonistReferences = new List<PawnKindDef>();

		public static List<AlienKey> AllAlienColorKeys = new List<AlienKey>();

		public static bool ColorsInitiated = false;

		public static List<PawnKindDef_StartingColonist> StartingColonistAliens
		{
			get
			{
				return DefDatabase<PawnKindDef_StartingColonist>.AllDefsListForReading;
			}
		}

		public static void InitializeAlienColors()
		{
			bool flag = !AlienRaceUtilities.ColorsInitiated;
			if (flag)
			{
				try
				{
					foreach (ThingDef current in DefDatabase<ThingDef>.AllDefsListForReading)
					{
						bool flag2 = current.defName.Contains("Alien_");
						if (flag2)
						{
							Thingdef_AlienRace thingdef_AlienRace = current as Thingdef_AlienRace;
							List<Color> list = new List<Color>();
							List<Color> list2 = new List<Color>();
							ColorGenerator_Options colorGenerator_Options;
							bool flag3 = (colorGenerator_Options = (thingdef_AlienRace.alienhaircolorgen as ColorGenerator_Options)) != null;
							if (flag3)
							{
								bool flag4 = colorGenerator_Options.options != null;
								if (flag4)
								{
									foreach (ColorOption current2 in colorGenerator_Options.options)
									{
										list.Add(current2.only);
									}
								}
							}
							ColorGenerator_Options colorGenerator_Options2;
							bool flag5 = (colorGenerator_Options2 = (thingdef_AlienRace.alienskincolorgen as ColorGenerator_Options)) != null;
							if (flag5)
							{
								bool flag6 = colorGenerator_Options2.options != null;
								if (flag6)
								{
									foreach (ColorOption current3 in colorGenerator_Options2.options)
									{
										list2.Add(current3.only);
									}
								}
							}
							AlienRaceUtilities.AllAlienColorKeys.Add(new AlienKey(thingdef_AlienRace, list, list2));
						}
					}
					AlienRaceUtilities.ColorsInitiated = true;
				}
				catch
				{
				}
			}
		}

		public static void InitializeAlienColonistOptions()
		{
			foreach (PawnKindDef_StartingColonist current in AlienRaceUtilities.StartingColonistAliens)
			{
				List<StartingColonistEntry> arg_3D_0 = current.IsPossibleStartingColonistOf;
				Predicate<StartingColonistEntry> arg_3D_1;
				if ((arg_3D_1 = AlienRaceUtilities.<>c.<>9__4_0) == null)
				{
					arg_3D_1 = (AlienRaceUtilities.<>c.<>9__4_0 = new Predicate<StartingColonistEntry>(AlienRaceUtilities.<>c.<>9.<InitializeAlienColonistOptions>b__4_0));
				}
				bool flag = arg_3D_0.Any(arg_3D_1);
				if (flag)
				{
					bool flag2 = !AlienRaceUtilities.AllAlienColonistReferences.Contains(current);
					if (flag2)
					{
						AlienRaceUtilities.AllAlienColonistReferences.Add(current);
					}
				}
			}
		}

		public static AlienPawn GenerateNewStartingAlienColonist(PawnKindDef kinddef)
		{
			PawnGenerationRequest request = new PawnGenerationRequest(kinddef, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, null, true, false, false, false, true, false, 26f, false, true, true, null, null, null, null, null, null);
			Pawn pawn = null;
			try
			{
				pawn = PawnGenerator.GeneratePawn(request);
			}
			catch (Exception arg)
			{
				Log.Error("There was an exception thrown by the PawnGenerator during generating a starting pawn. Trying one more time...\nException: " + arg);
				pawn = PawnGenerator.GeneratePawn(request);
			}
			pawn.relations.everSeenByPlayer = true;
			PawnComponentsUtility.AddComponentsForSpawn(pawn);
			AlienPawn alienPawn = pawn as AlienPawn;
			alienPawn.SpawnSetupAlien();
			return alienPawn;
		}

		public static Pawn NewGeneratedStartingPawnModded()
		{
			PawnKindDef kind = Faction.OfPlayer.def.basicMemberKind;
			List<PawnKindDef_StartingColonist> startingColonistAliens = AlienRaceUtilities.StartingColonistAliens;
			bool flag = AlienRaceUtilities.StartingColonistAliens.Count > 0;
			if (flag)
			{
				for (int i = 0; i < startingColonistAliens.Count; i++)
				{
					for (int j = 0; j < startingColonistAliens[i].IsPossibleStartingColonistOf.Count; j++)
					{
						bool flag2 = Faction.OfPlayer.def == startingColonistAliens[i].IsPossibleStartingColonistOf[j].factionDef;
						if (flag2)
						{
							float num = 1f / (startingColonistAliens[i].IsPossibleStartingColonistOf[j].ProportionOfBasicMember + 1f);
							bool flag3 = Rand.Range(0f, 1f) < num;
							if (flag3)
							{
								kind = startingColonistAliens[i];
								break;
							}
						}
					}
				}
			}
			PawnGenerationRequest request = new PawnGenerationRequest(kind, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, null, true, false, false, false, true, false, 26f, false, true, true, null, null, null, null, null, null);
			Pawn pawn = null;
			try
			{
				pawn = PawnGenerator.GeneratePawn(request);
			}
			catch (Exception arg)
			{
				Log.Error("There was an exception thrown by the PawnGenerator during generating a starting pawn. Trying one more time...\nException: " + arg);
				pawn = PawnGenerator.GeneratePawn(request);
			}
			pawn.relations.everSeenByPlayer = true;
			PawnComponentsUtility.AddComponentsForSpawn(pawn);
			bool flag4 = !pawn.def.defName.Contains("Alien_");
			Pawn result;
			if (flag4)
			{
				result = pawn;
			}
			else
			{
				AlienPawn alienPawn = pawn as AlienPawn;
				alienPawn.SpawnSetupAlien();
				result = alienPawn;
			}
			return result;
		}

		public static Pawn NewGeneratedStartingPawn()
		{
			PawnGenerationRequest request = new PawnGenerationRequest(Faction.OfPlayer.def.basicMemberKind, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, null, true, false, false, false, true, false, 26f, false, true, true, null, null, null, null, null, null);
			Pawn pawn = null;
			try
			{
				pawn = PawnGenerator.GeneratePawn(request);
			}
			catch (Exception arg)
			{
				Log.Error("There was an exception thrown by the PawnGenerator during generating a starting pawn. Trying one more time...\nException: " + arg);
				pawn = PawnGenerator.GeneratePawn(request);
			}
			pawn.relations.everSeenByPlayer = true;
			PawnComponentsUtility.AddComponentsForSpawn(pawn);
			return pawn;
		}

		public static void DoRecruitAlien(Pawn recruiter, Pawn recruitee, float recruitChance, bool useAudiovisualEffects = true)
		{
			string text = recruitee.LabelIndefinite();
			bool flag = recruitee.guest != null;
			if (flag)
			{
				recruitee.guest.SetGuestStatus(null, false);
			}
			bool flag2 = recruitee.Name != null;
			bool flag3 = recruitee.Faction != recruiter.Faction;
			if (flag3)
			{
				bool flag4 = recruitee.kindDef.race.ToString().Contains("Alien");
				if (flag4)
				{
					Log.Message("RecruitingAlienPawn");
					PawnKindDef kindDef = recruitee.kindDef;
					AlienPawn alienPawn = recruitee as AlienPawn;
					alienPawn.SetFaction(recruiter.Faction, recruiter);
					alienPawn.kindDef = kindDef;
					Log.Message("Pawn Converted to Kind:  " + recruitee.kindDef.race.ToString());
				}
				else
				{
					recruitee.SetFaction(recruiter.Faction, recruiter);
				}
			}
			bool humanlike = recruitee.RaceProps.Humanlike;
			if (humanlike)
			{
				if (useAudiovisualEffects)
				{
					Find.LetterStack.ReceiveLetter("LetterLabelMessageRecruitSuccess".Translate(), "MessageRecruitSuccess".Translate(new object[]
					{
						recruiter,
						recruitee,
						recruitChance.ToStringPercent()
					}), LetterType.Good, recruitee, null);
				}
				TaleRecorder.RecordTale(TaleDefOf.Recruited, new object[]
				{
					recruiter,
					recruitee
				});
				recruiter.records.Increment(RecordDefOf.PrisonersRecruited);
				recruitee.needs.mood.thoughts.memories.TryGainMemoryThought(ThoughtDefOf.RecruitedMe, recruiter);
			}
			else
			{
				if (useAudiovisualEffects)
				{
					bool flag5 = !flag2;
					if (flag5)
					{
						Messages.Message("MessageTameAndNameSuccess".Translate(new object[]
						{
							recruiter.LabelShort,
							text,
							recruitChance.ToStringPercent(),
							recruitee.Name.ToStringFull
						}).AdjustedFor(recruitee), recruitee, MessageSound.Benefit);
					}
					else
					{
						Messages.Message("MessageTameSuccess".Translate(new object[]
						{
							recruiter.LabelShort,
							text,
							recruitChance.ToStringPercent()
						}), recruitee, MessageSound.Benefit);
					}
					MoteMaker.ThrowText((recruiter.DrawPos + recruitee.DrawPos) / 2f, recruitee.Map, "TextMote_TameSuccess".Translate(new object[]
					{
						recruitChance.ToStringPercent()
					}), 8f);
				}
				recruiter.records.Increment(RecordDefOf.AnimalsTamed);
				RelationsUtility.TryDevelopBondRelation(recruiter, recruitee, 0.05f);
				float num = Mathf.Lerp(0.02f, 1f, recruitee.RaceProps.wildness);
				bool flag6 = Rand.Value < num;
				if (flag6)
				{
					TaleRecorder.RecordTale(TaleDefOf.TamedAnimal, new object[]
					{
						recruiter,
						recruitee
					});
				}
			}
			bool flag7 = recruitee.caller != null;
			if (flag7)
			{
				recruitee.caller.DoCall();
			}
		}
	}
}
