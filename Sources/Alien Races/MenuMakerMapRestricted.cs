using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AlienRace
{
	public class MenuMakerMapRestricted
	{
		private static bool RaceRestricted(Pawn pawn, Apparel app)
		{
			bool flag = app.GetComp<CompRestritctedRace>() != null;
			bool result;
			if (flag)
			{
				CompRestritctedRace comp = app.GetComp<CompRestritctedRace>();
				bool flag2 = comp.Props.RestrictedToRace != null;
				if (flag2)
				{
					bool flag3 = pawn.kindDef.race.ToString() == comp.Props.RestrictedToRace;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = pawn.GetType() == typeof(AlienPawn);
						if (flag4)
						{
							AlienPawn alienPawn = pawn as AlienPawn;
							bool flag5 = alienPawn.kindDef.race.ToString() == comp.Props.RestrictedToRace;
							result = !flag5;
						}
						else
						{
							result = true;
						}
					}
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static void AddHumanlikeOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
		{
			IntVec3 c2 = IntVec3.FromVector3(clickPos);
			foreach (Thing current in c2.GetThingList(pawn.Map))
			{
				Thing t = current;
				bool flag = t.def.ingestible != null && pawn.RaceProps.CanEverEat(t) && t.IngestibleNow;
				if (flag)
				{
					bool flag2 = t.def.ingestible.ingestCommandString.NullOrEmpty();
					string text;
					if (flag2)
					{
						text = "ConsumeThing".Translate(new object[]
						{
							t.LabelShort
						});
					}
					else
					{
						text = string.Format(t.def.ingestible.ingestCommandString, t.LabelShort);
					}
					bool flag3 = t.def.IsPleasureDrug && pawn.story != null && pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire) < 0;
					FloatMenuOption item;
					if (flag3)
					{
						item = new FloatMenuOption(text + " (" + TraitDefOf.DrugDesire.DataAtDegree(-1).label + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else
					{
						bool flag4 = !pawn.CanReach(t, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
						if (flag4)
						{
							item = new FloatMenuOption(text + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
						}
						else
						{
							bool flag5 = !pawn.CanReserve(t, 1);
							if (flag5)
							{
								item = new FloatMenuOption(text + " (" + "ReservedBy".Translate(new object[]
								{
									pawn.Map.reservationManager.FirstReserverOf(t, pawn.Faction, true).LabelShort
								}) + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
							}
							else
							{
								item = new FloatMenuOption(text, delegate
								{
									t.SetForbidden(false, true);
									Job job = new Job(JobDefOf.Ingest, t);
									job.count = FoodUtility.WillIngestStackCountOf(pawn, t.def);
									pawn.jobs.TryTakeOrderedJob(job);
								}, MenuOptionPriority.Default, null, null, 0f, null, null);
							}
						}
					}
					opts.Add(item);
				}
			}
			bool flag6 = pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
			if (flag6)
			{
				foreach (LocalTargetInfo current2 in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
				{
					Pawn victim = (Pawn)current2.Thing;
					bool flag7 = !victim.InBed() && pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1);
					if (flag7)
					{
						bool flag8 = (victim.Faction == Faction.OfPlayer && victim.MentalStateDef == null) || (victim.Faction != Faction.OfPlayer && victim.MentalStateDef == null && !victim.IsPrisonerOfColony && (victim.Faction == null || !victim.Faction.HostileTo(Faction.OfPlayer)));
						if (flag8)
						{
							Pawn victim4 = victim;
							opts.Add(new FloatMenuOption("Rescue".Translate(new object[]
							{
								victim.LabelCap
							}), delegate
							{
								Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, false, false, false);
								bool flag34 = building_Bed == null;
								if (flag34)
								{
									bool animal = victim.RaceProps.Animal;
									string str2;
									if (animal)
									{
										str2 = "NoAnimalBed".Translate();
									}
									else
									{
										str2 = "NoNonPrisonerBed".Translate();
									}
									Messages.Message("CannotRescue".Translate() + ": " + str2, victim, MessageSound.RejectInput);
								}
								else
								{
									Job job = new Job(JobDefOf.Rescue, victim, building_Bed);
									job.count = 1;
									job.playerForced = true;
									pawn.jobs.TryTakeOrderedJob(job);
									PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Rescuing, KnowledgeAmount.Total);
								}
							}, MenuOptionPriority.Default, null, victim4, 0f, null, null));
						}
						bool flag9 = victim.MentalStateDef != null || (victim.RaceProps.Humanlike && victim.Faction != Faction.OfPlayer);
						if (flag9)
						{
							Pawn victim2 = victim;
							opts.Add(new FloatMenuOption("Capture".Translate(new object[]
							{
								victim.LabelCap
							}), delegate
							{
								Building_Bed building_Bed = RestUtility.FindBedFor(victim, pawn, true, false, false);
								bool flag34 = building_Bed == null;
								if (flag34)
								{
									Messages.Message("CannotCapture".Translate() + ": " + "NoPrisonerBed".Translate(), victim, MessageSound.RejectInput);
								}
								else
								{
									Job job = new Job(JobDefOf.Capture, victim, building_Bed);
									job.count = 1;
									job.playerForced = true;
									pawn.jobs.TryTakeOrderedJob(job);
									PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Capturing, KnowledgeAmount.Total);
								}
							}, MenuOptionPriority.Default, null, victim2, 0f, null, null));
						}
					}
				}
				foreach (LocalTargetInfo current3 in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
				{
					LocalTargetInfo localTargetInfo = current3;
					Pawn victim = (Pawn)localTargetInfo.Thing;
					bool flag10 = victim.Downed && pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1) && Building_CryptosleepCasket.FindCryptosleepCasketFor(victim, pawn) != null;
					if (flag10)
					{
						string label = "CarryToCryptosleepCasket".Translate(new object[]
						{
							localTargetInfo.Thing.LabelCap
						});
						JobDef jDef = JobDefOf.CarryToCryptosleepCasket;
						Action action = delegate
						{
							Building_CryptosleepCasket building_CryptosleepCasket = Building_CryptosleepCasket.FindCryptosleepCasketFor(victim, pawn);
							bool flag34 = building_CryptosleepCasket == null;
							if (flag34)
							{
								Messages.Message("CannotCarryToCryptosleepCasket".Translate() + ": " + "NoCryptosleepCasket".Translate(), victim, MessageSound.RejectInput);
							}
							else
							{
								Job job = new Job(jDef, victim, building_CryptosleepCasket);
								job.count = 1;
								job.playerForced = true;
								pawn.jobs.TryTakeOrderedJob(job);
							}
						};
						Pawn victim3 = victim;
						opts.Add(new FloatMenuOption(label, action, MenuOptionPriority.Default, null, victim3, 0f, null, null));
					}
				}
			}
			foreach (LocalTargetInfo current4 in GenUI.TargetsAt(clickPos, TargetingParameters.ForStrip(pawn), true))
			{
				LocalTargetInfo stripTarg = current4;
				bool flag11 = !pawn.CanReach(stripTarg, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn);
				FloatMenuOption item2;
				if (flag11)
				{
					item2 = new FloatMenuOption("CannotStrip".Translate(new object[]
					{
						stripTarg.Thing.LabelCap
					}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
				}
				else
				{
					bool flag12 = !pawn.CanReserveAndReach(stripTarg, PathEndMode.ClosestTouch, Danger.Deadly, 1);
					if (flag12)
					{
						item2 = new FloatMenuOption("CannotStrip".Translate(new object[]
						{
							stripTarg.Thing.LabelCap
						}) + " (" + "ReservedBy".Translate(new object[]
						{
							pawn.Map.reservationManager.FirstReserverOf(stripTarg, pawn.Faction, true).LabelShort
						}) + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else
					{
						item2 = new FloatMenuOption("Strip".Translate(new object[]
						{
							stripTarg.Thing.LabelCap
						}), delegate
						{
							stripTarg.Thing.SetForbidden(false, false);
							Job job = new Job(JobDefOf.Strip, stripTarg);
							job.playerForced = true;
							pawn.jobs.TryTakeOrderedJob(job);
						}, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
				}
				opts.Add(item2);
			}
			bool flag13 = pawn.equipment != null;
			if (flag13)
			{
				ThingWithComps equipment = null;
				List<Thing> thingList = c2.GetThingList(pawn.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					bool flag14 = thingList[i].TryGetComp<CompEquippable>() != null;
					if (flag14)
					{
						equipment = (ThingWithComps)thingList[i];
						break;
					}
				}
				bool flag15 = equipment != null;
				if (flag15)
				{
					string label2 = equipment.Label;
					bool flag16 = !pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn);
					FloatMenuOption item3;
					if (flag16)
					{
						item3 = new FloatMenuOption("CannotEquip".Translate(new object[]
						{
							label2
						}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else
					{
						bool flag17 = !pawn.CanReserve(equipment, 1);
						if (flag17)
						{
							item3 = new FloatMenuOption("CannotEquip".Translate(new object[]
							{
								label2
							}) + " (" + "ReservedBy".Translate(new object[]
							{
								pawn.Map.reservationManager.FirstReserverOf(equipment, pawn.Faction, true).LabelShort
							}) + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
						}
						else
						{
							bool flag18 = !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
							if (flag18)
							{
								item3 = new FloatMenuOption("CannotEquip".Translate(new object[]
								{
									label2
								}) + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
							}
							else
							{
								string text2 = "Equip".Translate(new object[]
								{
									label2
								});
								bool flag19 = equipment.def.IsRangedWeapon && pawn.story != null && pawn.story.traits.HasTrait(TraitDefOf.Brawler);
								if (flag19)
								{
									text2 = text2 + " " + "EquipWarningBrawler".Translate();
								}
								item3 = new FloatMenuOption(text2, delegate
								{
									equipment.SetForbidden(false, true);
									Job job = new Job(JobDefOf.Equip, equipment);
									job.playerForced = true;
									pawn.jobs.TryTakeOrderedJob(job);
									MoteMaker.MakeStaticMote(equipment.DrawPos, pawn.Map, ThingDefOf.Mote_FeedbackEquip, 1f);
									PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.EquippingWeapons, KnowledgeAmount.Total);
								}, MenuOptionPriority.Default, null, null, 0f, null, null);
							}
						}
					}
					opts.Add(item3);
				}
			}
			bool flag20 = pawn.apparel != null;
			if (flag20)
			{
				Apparel apparel = pawn.Map.thingGrid.ThingAt<Apparel>(c2);
				bool flag21 = apparel != null;
				if (flag21)
				{
					bool flag22 = !pawn.CanReach(apparel, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn);
					FloatMenuOption item4;
					if (flag22)
					{
						item4 = new FloatMenuOption("CannotWear".Translate(new object[]
						{
							apparel.Label
						}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else
					{
						bool flag23 = !pawn.CanReserve(apparel, 1);
						if (flag23)
						{
							Pawn pawn2 = pawn.Map.reservationManager.FirstReserverOf(apparel, pawn.Faction, true);
							item4 = new FloatMenuOption("CannotWear".Translate(new object[]
							{
								apparel.Label
							}) + " (" + "ReservedBy".Translate(new object[]
							{
								pawn2.LabelShort
							}) + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
						}
						else
						{
							bool flag24 = !ApparelUtility.HasPartsToWear(pawn, apparel.def);
							if (flag24)
							{
								item4 = new FloatMenuOption("CannotWear".Translate(new object[]
								{
									apparel.Label
								}) + " (" + "CannotWearBecauseOfMissingBodyParts".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
							}
							else
							{
								bool flag25 = MenuMakerMapRestricted.RaceRestricted(pawn, apparel);
								if (flag25)
								{
									item4 = new FloatMenuOption("CannotWear".Translate(new object[]
									{
										apparel.Label
									}) + " (" + "CannotWearBecauseOfWrongRace".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
								}
								else
								{
									item4 = new FloatMenuOption("ForceWear".Translate(new object[]
									{
										apparel.LabelShort
									}), delegate
									{
										apparel.SetForbidden(false, true);
										Job job = new Job(JobDefOf.Wear, apparel);
										job.playerForced = true;
										pawn.jobs.TryTakeOrderedJob(job);
									}, MenuOptionPriority.Default, null, null, 0f, null, null);
								}
							}
						}
					}
					opts.Add(item4);
				}
			}
			bool flag26 = pawn.equipment != null && pawn.equipment.Primary != null;
			if (flag26)
			{
				Thing thing = pawn.Map.thingGrid.ThingAt(c2, ThingDefOf.EquipmentRack);
				bool flag27 = thing != null;
				if (flag27)
				{
					bool flag28 = !pawn.CanReach(thing, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn);
					if (flag28)
					{
						opts.Add(new FloatMenuOption("CannotDeposit".Translate(new object[]
						{
							pawn.equipment.Primary.LabelCap,
							thing.def.label
						}) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
					else
					{
						using (IEnumerator<IntVec3> enumerator5 = GenAdj.CellsOccupiedBy(thing).GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								IntVec3 c = enumerator5.Current;
								bool flag29 = c.GetStorable(pawn.Map) == null && pawn.CanReserveAndReach(c, PathEndMode.ClosestTouch, Danger.Deadly, 1);
								if (flag29)
								{
									Action action2 = delegate
									{
										ThingWithComps t;
										bool flag34 = pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out t, pawn.Position, true);
										if (flag34)
										{
											t.SetForbidden(false, true);
											Job job = new Job(JobDefOf.HaulToCell, t, c);
											job.haulMode = HaulMode.ToCellStorage;
											job.count = 1;
											job.playerForced = true;
											pawn.jobs.TryTakeOrderedJob(job);
										}
									};
									opts.Add(new FloatMenuOption("Deposit".Translate(new object[]
									{
										pawn.equipment.Primary.LabelCap,
										thing.def.label
									}), action2, MenuOptionPriority.Default, null, null, 0f, null, null));
									break;
								}
							}
						}
					}
				}
				bool flag30 = pawn.equipment != null && GenUI.TargetsAt(clickPos, TargetingParameters.ForSelf(pawn), true).Any<LocalTargetInfo>();
				if (flag30)
				{
					Action action3 = delegate
					{
						ThingWithComps thingWithComps;
						pawn.equipment.TryDropEquipment(pawn.equipment.Primary, out thingWithComps, pawn.Position, true);
						pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.Wait, 20, false));
					};
					opts.Add(new FloatMenuOption("Drop".Translate(new object[]
					{
						pawn.equipment.Primary.Label
					}), action3, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
			}
			foreach (LocalTargetInfo current5 in GenUI.TargetsAt(clickPos, TargetingParameters.ForTrade(), true))
			{
				LocalTargetInfo dest = current5;
				bool flag31 = !pawn.CanReach(dest, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
				if (flag31)
				{
					opts.Add(new FloatMenuOption("CannotTrade".Translate() + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				else
				{
					bool flag32 = !pawn.CanReserve(dest.Thing, 1);
					if (flag32)
					{
						opts.Add(new FloatMenuOption("CannotTrade".Translate() + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
					else
					{
						Pawn pTarg = (Pawn)dest.Thing;
						Action action4 = delegate
						{
							Job job = new Job(JobDefOf.TradeWithPawn, pTarg);
							job.playerForced = true;
							pawn.jobs.TryTakeOrderedJob(job);
							PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.InteractingWithTraders, KnowledgeAmount.Total);
						};
						string str = string.Empty;
						bool flag33 = pTarg.Faction != null;
						if (flag33)
						{
							str = " (" + pTarg.Faction.Name + ")";
						}
						Thing thing2 = dest.Thing;
						opts.Add(new FloatMenuOption("TradeWith".Translate(new object[]
						{
							pTarg.LabelShort + ", " + pTarg.TraderKind.label
						}) + str, action4, MenuOptionPriority.Default, null, thing2, 0f, null, null));
					}
				}
			}
			foreach (Thing current6 in pawn.Map.thingGrid.ThingsAt(c2))
			{
				foreach (FloatMenuOption current7 in current6.GetFloatMenuOptions(pawn))
				{
					opts.Add(current7);
				}
			}
		}
	}
}
