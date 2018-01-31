using System;
using Verse;
using RimWorld;
using System.Linq;

namespace RimWorldBiomesCore
{
    public class Comp_Trail : ThingComp
    {
        public override void CompTick()
        {
            if (this.Props == null || ((Pawn)parent).Dead)
            {
                return;
            }
            Trail();
            base.CompTick();
        }
        public CompProperties_Trail Props
        {
            get
            {
                return (CompProperties_Trail)this.props;
            }
        }

        public void Trail(){
            if(!parent.Spawned){
                return;
            }
            if(Props.trail == null){
                return;
            }
            Thing trail = ThingMaker.MakeThing(Props.trail);
            if(parent.Position.GetThingList(parent.Map) != null && !parent.Position.GetThingList(parent.Map).Contains(trail) && (from t in parent.Position.GetThingList(parent.Map)
                                                                          where t.def == Props.trail
                                                                         select t).FirstOrDefault<Thing>() == null){
                    GenSpawn.Spawn(trail, parent.Position, parent.Map);
            }
        }
    }
}
