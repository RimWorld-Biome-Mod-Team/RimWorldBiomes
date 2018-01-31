using System;
using Verse.AI;
using Verse;
namespace RimWorldBiomesCore
{
    public class JobGiver_WanderWaterAnywhere : JobGiver_WanderWater
    {
        public JobGiver_WanderWaterAnywhere()
        {
            this.wanderRadius = 7f;
            this.locomotionUrgency = LocomotionUrgency.Walk;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
        }
        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            return pawn.Position;
        }
    }
}
