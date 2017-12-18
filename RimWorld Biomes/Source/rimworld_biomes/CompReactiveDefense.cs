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
    public class CompReactiveDefense : ThingComp
    {
		public override void CompTick()
		{
            if(this.Props == null){
                return;
            }
			base.CompTick();
            Defend(this.Props);
		}
        public CompProperties_ReactiveDefense Props{
            get{
                return (CompProperties_ReactiveDefense)this.props;
            }
        }

        public void Defend(CompProperties_ReactiveDefense props){
            IntVec3 pos = base.parent.Position;
            Map map = base.parent.Map;
            if (props.defenseTrigger == CompProperties_ReactiveDefense.defTrigger.attacked){
                
            }
			if (props.defenseTrigger == CompProperties_ReactiveDefense.defTrigger.health)
			{

			}
			if (props.defenseTrigger == CompProperties_ReactiveDefense.defTrigger.proximity)
			{
                Proximity(pos, map, props);
			}
        }

        public void React(IntVec3 pos, Map map, CompProperties_ReactiveDefense props){
            if(props.defenseType == CompProperties_ReactiveDefense.defType.aura){
                Aura(pos, map, props);
            }
            if (props.defenseType == CompProperties_ReactiveDefense.defType.buff)
			{

			}
            if (props.defenseType == CompProperties_ReactiveDefense.defType.hide)
			{

			}
			if (props.defenseType == CompProperties_ReactiveDefense.defType.projectile)
			{

			}
        }

        public void Proximity(IntVec3 pos, Map map, CompProperties_ReactiveDefense props){
                for (int i = pos.x - props.proximity; i <= pos.x + props.proximity; i++){
					for (int j = pos.z - props.proximity; j <= pos.z + props.proximity; j++)
					{
                        IntVec3 temp = new IntVec3(i, 0, j);
                    if (temp.InBounds(map) && temp.GetFirstPawn(map) != null && temp != pos && !temp.GetFirstPawn(map).Dead && !temp.GetFirstPawn(map).Downed){
                            React(pos, map, props);
                        }
					}
                }


        }

        public void Aura(IntVec3 pos, Map map, CompProperties_ReactiveDefense props)
        {
            if (props.aura != null) {
                for (int i = pos.x - props.auraSize; i <= pos.x + props.auraSize; i++)
                {
                    for (int j = pos.z - props.auraSize; j <= pos.z + props.auraSize; j++)
                    {
                        IntVec3 temp = new IntVec3(i, 0, j);
                        if (temp != pos && temp.InBounds(map))
                        {
                            ThingDef particle = ThingDef.Named(props.aura);
                            List<ThingDef> thingdefs = new List<ThingDef>();
                            foreach(Thing t in temp.GetThingList(map)){
                                thingdefs.Add(t.def);
                            }
                            if (particle.GetCompProperties<CompProperties_AuraParticle>() != null && temp.GetFirstBuilding(map) == null && !thingdefs.Contains(ThingDef.Named(props.aura))){
                                particle.GetCompProperties<CompProperties_AuraParticle>().duration = props.duration;
								GenSpawn.Spawn(ThingDef.Named(props.aura), temp, map);
                            }


						}
					}
				}
            }
        }
    }
}
