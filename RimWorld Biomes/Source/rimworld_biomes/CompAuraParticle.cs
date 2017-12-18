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
namespace rimworld_biomes
{
    public class CompAuraParticle : ThingComp
    {
        int count = GenTicks.TicksGame;
        public override void CompTick()
        {

            Map map = base.parent.Map;
            if (this.Props == null)
            {
                return;
            }
            //Log.Error("start: " + count.ToString());
            //Log.Error("End: " + (count + GenTicks.SecondsToTicks(Props.duration)));
            //Log.Error("current: " + GenTicks.TicksGame);

            if (GenTicks.TicksGame > count + GenTicks.SecondsToTicks(Props.duration))
            {
                base.parent.Destroy();
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
                            HealthUtility.AdjustSeverity(p, HediffDef.Named(this.Props.hediff), this.Props.severity);
                        }
                    }
                }
            }
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
