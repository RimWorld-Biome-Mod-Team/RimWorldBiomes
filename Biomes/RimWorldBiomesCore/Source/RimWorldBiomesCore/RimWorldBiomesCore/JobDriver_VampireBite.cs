using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;
using RimWorld.Planet;
using System.Linq;
namespace RimWorldBiomesCore
{
    public class JobDriver_VampireBite : JobDriver
    {
        string currentActivity = "";
        public bool firstHit = true;
        public Pawn Prey
        {
            get
            {
                return (Pawn)base.job.GetTarget(TargetIndex.A).Thing;
            }
        }



        public override bool TryMakePreToilReservations()
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            //Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            //gotoToil.AddPreInitAction(new Action(delegate
            //{
            //    this.pawn.ClearAllReservations();
            //    this.pawn.Reserve(TargetA, this.job);
            //    //this.Map.physicalInteractionReservationManager.ReleaseAllForTarget(TargetA);
            //    //this.Map.physicalInteractionReservationManager.Reserve(this.GetActor(), TargetA);
            //    currentActivity = "Hunting";
            //}));
            //yield return gotoToil;
            Action onHitAction = delegate
            {
            Pawn prey = this.Prey;
            bool surpriseAttack = this.firstHit && !prey.IsColonist;
                Job tjob = prey.CurJob;
                if (this.pawn.needs.food.CurLevelPercentage < 1 && !prey.Dead && this.pawn.meleeVerbs.TryMeleeAttack(prey, this.job.verbToUse, surpriseAttack)){
                    float new_food = this.pawn.needs.food.CurLevelPercentage + 0.5f;
                    if (new_food > 1)
                        new_food = 1;
                    this.pawn.needs.food.CurLevelPercentage = new_food;
                    if (prey.Dead){
                        return;
                    }

                    prey.stances.stunner.StunFor((int)700);
                    IEnumerable<Hediff> visibleDiffs = prey.health.hediffSet.hediffs;
                    List<Hediff> toremove = new List<Hediff>();
                    foreach(Hediff h in visibleDiffs){
                        if((h.source == this.pawn.def || (h.def == HediffDefOf.MissingBodyPart && h.ageTicks < 100))  && h.def != HediffDefOf.BloodLoss){
                            toremove.Add(h);
                        }
                    }
                    foreach(Hediff h in toremove){
                        prey.health.RemoveHediff(h);
                    }
                    prey.jobs.StartJob(tjob,JobCondition.InterruptForced);
                }else{
                    if(Math.Abs(this.pawn.needs.food.CurLevelPercentage - 1) < float.Epsilon)
                    {
                        this.pawn.jobs.curDriver.ReadyForNextToil();
                    }
                }

            this.firstHit = false;
        };
               
                yield return Toils_Combat.FollowAndMeleeAttack(TargetIndex.A, onHitAction);

            
        }
    }
}
