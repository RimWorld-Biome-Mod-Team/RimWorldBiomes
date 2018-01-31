using System;
using UnityEngine;
using Verse;
namespace RimWorldBiomesCore
{
    
    public class Comp_TempThing : ThingComp
    {
        bool first = false;
        float start = 0;
        public override void CompTick()
        {
            if(!first){
                first = true;
                start = GenTicks.TicksGame;
            }
            base.CompTick();
            if(start + GenTicks.SecondsToTicks(Props.lifetime) < GenTicks.TicksGame){
                parent.Destroy();
            }
        }
        public CompProperties_TempThing Props
        {
            get
            {
                return (CompProperties_TempThing)this.props;
            }
        }
    }
}
