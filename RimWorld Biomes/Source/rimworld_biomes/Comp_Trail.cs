using System;
using Verse;
using RimWorld;
using System.Linq;

namespace rimworld_biomes
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
            if(Props.trail == null){
                return;
            }
            Thing trail = ThingMaker.MakeThing(Props.trail);
            if(!parent.Position.GetThingList(parent.Map).Contains(trail)){
                Filth filth = (Filth)(from t in parent.Position.GetThingList(parent.Map)
                                      where t.def == Props.trail
                                      select t).FirstOrDefault<Thing>();
                if (filth != null)
                {
                }
                else
                {
                    Filth filth2 = (Filth)ThingMaker.MakeThing(Props.trail, null);
                    filth2.thickness = Props.maxStacks;
                    //for (int i = 0; i < Props.maxStacks;i++){
                    //    filth.ThickenFilth();
                    //}
                    GenSpawn.Spawn(filth2, parent.Position, parent.Map);
                    filth2.ThickenFilth();

                }
            }
        }
    }
}
