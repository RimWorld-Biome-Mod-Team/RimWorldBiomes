using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
namespace RimWorldBiomesCore
{
    public class Building_Cocoon : Building_Casket
    {
        public Pawn Victim
        {
            get
            {
                Pawn result = null;
                if (this.innerContainer.Count > 0)
                {
                    if (this.innerContainer[0] is Pawn p) result = p;
                    if (this.innerContainer[0] is Corpse y) result = y.InnerPawn;
                }
                return result;
            }
        }

        public bool isPathableBy(Pawn p)
        {
            bool result = false;
            using (PawnPath pawnPath = p.Map.pathFinder.FindPath(p.Position, this.Position, TraverseParms.For(p, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
            {
                if (!pawnPath.Found)
                {
                    return result;
                }
            }
            result = true;
            return result;
        }

        //Wild spiders || Faction spiders && player spiders must have access to cocoons
        public bool playerFactionExceptions(Spider y) => 
            (y?.Faction == null || 
            (Victim?.Faction != y?.Faction) && (y.Faction == Faction.OfPlayerSilentFail && this.PositionHeld.InAllowedArea(y)));

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        public override void DeSpawn()
        {
            base.DeSpawn();
        }

        // RimWorld.Building_Grave code repurposed for Cocoons
        private Graphic cachedGraphicFull;
        private Graphic cachedGraphicEmpty;
        public override Graphic Graphic
        {
            get
            {
                if (this.def.building.fullGraveGraphicData == null)
                {
                    return base.Graphic;
                }

                if (Victim == null)
                {
                    if (this.cachedGraphicEmpty == null)
                    {
                        this.cachedGraphicEmpty = GraphicDatabase.Get<Graphic_Single>(this.def.graphicData.texPath, ShaderDatabase.Cutout, this.def.graphicData.drawSize, this.DrawColor, this.DrawColorTwo, this.def.graphicData);
                    }
                    return this.cachedGraphicEmpty;
                }

                if (this.cachedGraphicFull == null)
                {
                    this.cachedGraphicFull = GraphicDatabase.Get<Graphic_Single>(this.def.building.fullGraveGraphicData.texPath, ShaderDatabase.Cutout, this.def.building.fullGraveGraphicData.drawSize, this.DrawColor, this.DrawColorTwo, this.def.building.fullGraveGraphicData);
                }
                return this.cachedGraphicFull;
            }
        }

        public bool isConsumable
        {
            get
            {
                return Victim != null &&
                        //Victim.IngestibleNow &&
                        this.Spawned &&
                        !this.Destroyed &&
                        !this.MapHeld.physicalInteractionReservationManager.IsReserved(this);
            }
        }

        public override string GetInspectString()
        {
            var str = this.innerContainer.ContentsString;
            var str2 = "None".Translate();
            var compDisappearsStr = this.GetComp<CompLifespan>()?.CompInspectStringExtra()?.TrimEndNewlines() ?? "";
            var result = new StringBuilder();

            result.AppendLine("CasketContains".Translate() + ": " + str.CapitalizeFirst());
            result.AppendLine(compDisappearsStr);

            return result.ToString().TrimEndNewlines();
        }

        private SoundDef sound = SoundDef.Named("HissSmall");

        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    //sound.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
                }
                return true;
            }
            return false;
        }

        public int lastEscapeAttempt = 0;

        public override void Tick()
        {
            base.Tick();
        }

        // RimWorld.Building_Casket
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (this.innerContainer.Count > 0 && (mode == DestroyMode.Deconstruct || mode == DestroyMode.KillFinalize))
            {
                if (mode != DestroyMode.Deconstruct)
                {
                    List<Pawn> list = new List<Pawn>();
                    foreach (Thing current in ((IEnumerable<Thing>)this.innerContainer))
                    {
                        Pawn pawn = current as Pawn;
                        if (pawn != null)
                        {
                            list.Add(pawn);
                        }
                    }
                    foreach (Pawn current2 in list)
                    {
                        HealthUtility.DamageUntilDowned(current2);
                    }
                }
                this.EjectContents();
            }
            this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            if (!this.Destroyed) base.Destroy(mode);
        }


        public override void EjectContents()
        {
            foreach (Thing current in ((IEnumerable<Thing>)this.innerContainer))
            {
                if (current is Pawn pawn)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawn.filth.GainFilth(ThingDefOf.FilthSlime);
                    //pawn.health.AddHediff(HediffDefOf.ToxicBuildup, null, null);
                    HealthUtility.AdjustSeverity(pawn, HediffDefOf.ToxicBuildup, 0.3f);
                }
            }
            if (this.MapHeld != null)
            {
                this.innerContainer.TryDropAll(this.PositionHeld, this.MapHeld, ThingPlaceMode.Near);
            }
            this.contentsKnown = true;
            if (!this.Destroyed) this.Destroy(DestroyMode.KillFinalize);
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
