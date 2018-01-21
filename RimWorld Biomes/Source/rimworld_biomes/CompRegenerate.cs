using System;
using UnityEngine;
using Verse;
using RimWorld;
namespace rimworld_biomes
{
    public class CompRegenerate : ThingComp
    {
        bool restoring = false;
        int lastheal = 0;
        public CompProperties_Regenerate Props
        {
            get
            {
                return (CompProperties_Regenerate)this.props;
            }
        }

        public void DeadNotDowned()
        {
            //Gets pawn this ThingComp is attached to
            // Pawn pawn declares a variable of type Pawn that is named pawn
            //base.parent gets the parent object of type ThingWithComps while (Pawn) casts aka converts
            //that object into the type Pawn
            Pawn pawn = (Pawn)base.parent;

            //Calls the method Downed that is located inside the Pawn class
            //What this method does is return whether or not the pawn is currently downed
            if(pawn.Downed){
                //Kill the pawn
                GenExplosion.DoExplosion(pawn.Position, pawn.Map, 10, DamageDefOf.Bomb, pawn,50,,ThingDefOf.Explosion,null,,);
                pawn.Destroy();
            }

        }
        public override void CompTick()
        {
            Pawn pawn = (Pawn)base.parent;
            if(((Pawn)parent).health.hediffSet.GetPartHealth(((Pawn)parent).RaceProps.body.corePart) < ((Pawn)parent).RaceProps.body.corePart.def.hitPoints * Props.startThreshold * ((Pawn)parent).def.race.baseHealthScale){
                if(GenTicks.TicksGame > lastheal + GenTicks.SecondsToTicks(Props.delay)){
                    foreach(Hediff h in pawn.health.hediffSet.hediffs){
                        if(h.def == HediffDefOf.Cut || h.def == HediffDefOf.Stab || h.def == HediffDefOf.Bruise || h.def == HediffDefOf.Bite || h.def == HediffDefOf.Gunshot || h.def == HediffDefOf.Scratch || h.def == HediffDefOf.Shredded ||  h.def == HediffDef.Named("Crush") || h.def == HediffDef.Named("Crack") || (h.def == HediffDefOf.MissingBodyPart && Props.restoreBodyParts)){
                            h.Heal(Props.amount);
                        }
                    }
                    lastheal = GenTicks.TicksGame;
                }
                restoring = true;
            }
            if(restoring && ((Pawn)parent).health.hediffSet.GetPartHealth(((Pawn)parent).RaceProps.body.corePart) >= ((Pawn)parent).RaceProps.body.corePart.def.hitPoints * Props.endThreshold * ((Pawn)parent).def.race.baseHealthScale){
                restoring = false;
            }
            base.CompTick();

        }
    }
}
