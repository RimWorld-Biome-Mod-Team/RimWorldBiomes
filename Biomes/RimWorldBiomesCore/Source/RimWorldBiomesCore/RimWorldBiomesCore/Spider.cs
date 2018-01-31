using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
namespace RimWorldBiomesCore
{
    public class Spider : Pawn
    {


        public override void Tick()
        {
            base.Tick();
            Map map = base.Map;
            Corpse corpse;
            if(GenTicks.TicksGame % GenTicks.SecondsToTicks(5) == 0){
                if (this.Spawned && !this.Downed && !this.Dead && FindNearestCorpse(this, map, out corpse) && !IsBusy)
                {
                    this.jobs.StartJob(new Job(RWBDefOf.RWBSpinCocoon, corpse), JobCondition.InterruptForced);
                }
            }
        }

        private bool FindNearestCorpse(Pawn pawn, Map map, out Corpse corpse){
            float maxDist = 5f;
            Corpse closest = null;
            foreach(Thing t in map.spawnedThings.ToList()){
                if(t.def.IsCorpse && pawn.CanReach(t.Position,PathEndMode.OnCell,Danger.Some, false,TraverseMode.ByPawn) && Mathf.Sqrt(Mathf.Pow(t.Position.x - pawn.Position.x,2)+ Mathf.Pow(t.Position.z - pawn.Position.z, 2)) < maxDist){
                    closest = (Verse.Corpse)t;
                }
            }
            if(closest != null){
                corpse = closest;
                return true;
            }
            corpse = null;
            return false;
        }

        private bool IsBusy{
            get
            {
                return this?.CurJob?.def == JobDefOf.PredatorHunt ||
                       this?.CurJob?.def == JobDefOf.Ingest ||
                       this?.CurJob?.def == JobDefOf.LayDown ||
                       this?.CurJob?.def == JobDefOf.Mate ||
                       this?.CurJob?.def == JobDefOf.LayEgg ||
                       this?.CurJob?.def == JobDefOf.AttackMelee ||
                       this?.CurJob?.def == RWBDefOf.RWBSpinCocoon;
            }
        }
    }
}
