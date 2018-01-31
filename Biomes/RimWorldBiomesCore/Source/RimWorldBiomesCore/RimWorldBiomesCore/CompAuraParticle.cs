using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;
using System.Reflection.Emit;
using RimWorld.Planet;
using System.Runtime.CompilerServices;
using RimWorld.BaseGen;
using System.Text;
using System;
using Verse.Sound;
using Verse.AI.Group;
using Verse.Noise;
namespace RimWorldBiomesCore
{
    public class CompAuraParticle : ThingComp
    {
        int count = GenTicks.TicksGame;
        bool hasWarned = false;
        int lastWarned = GenTicks.TicksGame;
        public override void CompTick()
        {
            if(hasWarned){
                if(lastWarned > GenTicks.TicksGame + GenTicks.SecondsToTicks(3)){
                    hasWarned = false;
                }
            }
            Map map = base.parent.Map;
            if (this.Props == null)
            {
                return;
            }
            base.CompTick();
            if (this.Props.hediff != null)
            {

                if (base.parent.Position.GetFirstPawn(map) != null)
                {
                    foreach (Pawn p in map.mapPawns.AllPawns)
                    {
                        if (p.Position == base.parent.Position)
                        {

                            if (!p.Downed)
                            {
                                HealthUtility.AdjustSeverity(p, HediffDef.Named(this.Props.hediff), this.Props.severity);
                            }
                            else{
                                HealthUtility.AdjustSeverity(p, HediffDef.Named(this.Props.hediff), this.Props.severity / 4);
                            }

                            //Log.Error("1");

                                if (Props.warn && Math.Abs(p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named(this.Props.hediff)).Severity - this.Props.severity) < Double.Epsilon && p.Faction != null && p.Faction.IsPlayer)
                                {
                                    Messages.Message(p.Name + " has walked into a " + this.parent.Label, p, MessageTypeDefOf.ThreatSmall);
                                }
                                //Log.Error("2");
                            if (Props.forceFlee && !p.Downed && (p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named(this.Props.hediff)).Severity > 0.7) || (p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named(this.Props.hediff)).Severity > 0.5 && !(p.Faction != null && p.Faction.IsPlayer && p.Drafted) && p.CurJob?.def != JobDefOf.Flee && !((this.Props.releasedBy == CompProperties_AuraParticle.parent.plant && Plant(p)) || (this.Props.releasedBy == CompProperties_AuraParticle.parent.animal && Animal(p)) || (this.Props.releasedBy == CompProperties_AuraParticle.parent.building && Building(p) || (this.Props.releasedBy == CompProperties_AuraParticle.parent.item && Item(p))))))
                                {
                                if (GenTicks.TicksGame - count > GenTicks.SecondsToTicks(0.5f))
                                {
                                    if (Props.warn && p.Faction != null && p.Faction.IsPlayer && !hasWarned)
                                    {
                                        Messages.Message(p.Name + " is fleeing from a " + this.parent.Label, p, MessageTypeDefOf.ThreatSmall);
                                        hasWarned = true;
                                        lastWarned = GenTicks.TicksGame;
                                    }
                                    IntVec3 dest = CellFinderLoose.GetFleeDest(p, new List<Thing> { this.parent }, 8f);
                                    if (dest == this.parent.Position)
                                    {
                                        dest = dest.RandomAdjacentCell8Way();
                                    }
                                    p.jobs?.TryTakeOrderedJob(new Job(JobDefOf.Flee, dest), JobTag.Escaping);
                                }
                                }
                                else
                                {
                                    //Log.Error("3");
                                if (Props.forceFlee && !p.Downed && p.Faction != null && p.Faction.IsPlayer && p.Drafted && p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named(this.Props.hediff)).Severity > 0.7)
                                    {
                                        if (GenTicks.TicksGame - count > GenTicks.SecondsToTicks(0.5f))
                                        {
                                        if (Props.warn && Props.forceFlee && p.Faction != null && p.Faction.IsPlayer && !hasWarned)
                                            {
                                                Messages.Message(p.Name + " is fleeing from a " + this.parent.Label, p, MessageTypeDefOf.ThreatSmall);
                                                hasWarned = true;
                                                lastWarned = GenTicks.TicksGame;
                                            }
                                            IntVec3 dest = CellFinderLoose.GetFleeDest(p, new List<Thing> { this.parent }, 8f);
                                            if (dest == this.parent.Position)
                                            {
                                                dest = dest.RandomAdjacentCell8Way();
                                            }
                                            p.jobs?.TryTakeOrderedJob(new Job(JobDefOf.Flee, dest), JobTag.Escaping);
                                        }
                                    }
                                }

                        }
                    }
                }
            }
            if (GenTicks.TicksGame > count + GenTicks.SecondsToTicks(Props.duration))
            {
                base.parent.Destroy();
            }
        }

        private bool Item(Pawn p){
            return false;
        }
        private bool Plant(Pawn p){
            return ((p.CurJob?.def == JobDefOf.Harvest || p.CurJob?.def == JobDefOf.CutPlant || p.CurJob?.def == JobDefOf.Ingest) && this.Props.parentThing != null && p.CurJob?.targetA.Thing.def.defName == this.Props.parentThing);
        }
		private bool Animal(Pawn p)
		{
			return ((p.CurJob?.def == JobDefOf.AttackMelee) && this.Props.parentThing != null && p.CurJob?.targetA.Thing.def.defName == this.Props.parentThing);
		}
        private bool Building(Pawn p){ 
            return ((p.CurJob?.def == JobDefOf.AttackMelee || p.CurJob?.def == JobDefOf.Deconstruct) && this.Props.parentThing != null && p.CurJob?.targetA.Thing.def.defName == this.Props.parentThing);
        }
        public CompProperties_AuraParticle Props
        {
            get
            {
                return (CompProperties_AuraParticle)this.props;
            }
        }
    }
}
