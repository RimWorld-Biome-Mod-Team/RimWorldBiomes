using System;
using Verse;
using Verse.AI;
using RimWorld;
namespace RimWorldBiomesCore
{
    public class JobGiver_CocoonCorpse : ThinkNode_JobGiver
    {

        private const float SearchRadius = 8f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            IntVec3 c = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 5f, null, Danger.Some);
            return new Job(JobDefOf.LayEgg, c);
        }
    }
}
