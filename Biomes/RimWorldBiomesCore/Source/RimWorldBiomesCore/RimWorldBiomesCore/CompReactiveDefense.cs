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
namespace RimWorldBiomesCore
{
    public class CompReactiveDefense : ThingComp
    {
        private IDictionary<StatDef, float> stats = new Dictionary<StatDef, float>();
        bool initalized = false;
        bool hidden = false;
        bool stoppedAttacker = false;
        DamageInfo lastattack = new DamageInfo();
        bool newattack = false;
		public override void CompTick()
		{
            if(this.Props == null){
                return;
            }
            if(!initalized){
                foreach(StatModifier stat in parent.def.statBases){
                    stats.Add(stat.stat,stat.value);
                }
                initalized = true;
            }
            Defend(this.Props);
			base.CompTick();
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
                Attacked(pos,map,props);
            }
			if (props.defenseTrigger == CompProperties_ReactiveDefense.defTrigger.health)
			{
                    React(pos, map, props);
			}
			if (props.defenseTrigger == CompProperties_ReactiveDefense.defTrigger.proximity)
			{
                Proximity(pos, map, props);
			}
        }
        public void Attacked(IntVec3 pos, Map map, CompProperties_ReactiveDefense props){
            if(newattack){
                newattack = false;
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
                Hide(pos, map,  props);
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

        public void Hide(IntVec3 pos, Map map, CompProperties_ReactiveDefense props){
            if(props.statDefs == null || props.statDefs.Count == 0 || props.statValues == null || props.statValues.Count == 0 || props.statDefs.Count != props.statValues.Count){
                return;
            }
            Pawn pawn = (Pawn)parent;
            if(pawn.Dead){
                if(pawn.apparel.WornApparel.Count > 0){
                    List<Apparel> tap = pawn.apparel.WornApparel.ToList();
                    foreach(Apparel apparel in tap){
                        pawn.apparel.Remove(apparel);
                    }
                }
                return;
            }

            if(((Pawn)parent).health.hediffSet.GetPartHealth(((Pawn)parent).RaceProps.body.corePart) < ((Pawn)parent).RaceProps.body.corePart.def.hitPoints * props.hpThreshold * ((Pawn)parent).def.race.baseHealthScale){
               //Log.Error("a");
                if(!hidden){
                    if(Props.apparel != null){
                        Apparel apparel = (Apparel)ThingMaker.MakeThing(Props.apparel);
                        for (int i = 0; i < props.statDefs.Count; i++)
                        {
                            apparel.def.SetStatBaseValue(props.statDefs[i], props.statValues[i]);
                        }
                        if (ApparelUtility.HasPartsToWear(pawn, apparel.def))
                        {
                            if(pawn.apparel == null){
                                pawn.apparel = new Pawn_ApparelTracker(pawn);
                            }
                            pawn.apparel.Wear(apparel, false);
                        }


					}
                    if (Props.stopAttacker && !stoppedAttacker)
                    {
                        ((Pawn)lastattack.Instigator).jobs.StartJob(new Job(JobDefOf.WaitWander), JobCondition.InterruptForced);
                        stoppedAttacker = true;
                    }
                    ResolveHideGraphic();
                    hidden = true;
                    pawn.jobs.StartJob(new Job(JobDefOf.FleeAndCower, pawn.Position),JobCondition.InterruptForced);					
				} else {
					//prevent from moving
					pawn.pather.StopDead();
				}
			}
			else {
				if (hidden)
				{
					if (Props.apparel != null && pawn.apparel != null)
					{
						pawn.apparel.DestroyAll();
					}
					ResolveBaseGraphic();
					hidden = false;
					stoppedAttacker = false;


					//allow to move again
//					pawn.pather.ResetToCurrentPosition();
					pawn.pather.StartPath(pawn.Position, PathEndMode.OnCell);
				}
			}
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {   
            lastattack = dinfo;
            newattack = true;
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            if (parent as Pawn != null)
            {
                Pawn pawn = (Pawn)parent;
                if (pawn.Dead)
                {
                    if (Props.apparel != null && pawn.apparel != null)
                    {
                        pawn.apparel.DestroyAll();
                    }
                    ResolveBaseGraphic();
                    hidden = false;
                }
            }

        }

        public void ResolveHideGraphic()
        {
            if (Props.hideGraphic != null &&
                ((Pawn)parent).Drawer?.renderer?.graphics != null)
            {
                PawnGraphicSet pawnGraphicSet = ((Pawn)parent).Drawer?.renderer?.graphics;
                pawnGraphicSet = ((Pawn)parent).Drawer?.renderer?.graphics;
                pawnGraphicSet.ClearCache();
                pawnGraphicSet.nakedGraphic = Props.hideGraphic.Graphic;
            }
        }

        public void ResolveBaseGraphic()
        {
            if (Props.hideGraphic != null &&
                ((Pawn)parent).Drawer?.renderer?.graphics != null)
            {
                PawnGraphicSet pawnGraphicSet = ((Pawn)parent).Drawer?.renderer?.graphics;
                pawnGraphicSet.ClearCache();

                //Duplicated code from -> Verse.PawnGrapic -> ResolveAllGraphics
                var curKindLifeStage = ((Pawn)parent).ageTracker.CurKindLifeStage;
                if (((Pawn)parent).gender != Gender.Female || curKindLifeStage.femaleGraphicData == null)
                    pawnGraphicSet.nakedGraphic = curKindLifeStage.bodyGraphicData.Graphic;
                else
                    pawnGraphicSet.nakedGraphic = curKindLifeStage.femaleGraphicData.Graphic;
                pawnGraphicSet.rottingGraphic = pawnGraphicSet.nakedGraphic.GetColoredVersion(ShaderDatabase.CutoutSkin,
                    PawnGraphicSet.RottingColor, PawnGraphicSet.RottingColor);
                if (((Pawn)parent).RaceProps.packAnimal)
                    pawnGraphicSet.packGraphic = GraphicDatabase.Get<Graphic_Multi>(
                        pawnGraphicSet.nakedGraphic.path + "Pack", ShaderDatabase.Cutout,
                        pawnGraphicSet.nakedGraphic.drawSize, Color.white);
                if (curKindLifeStage.dessicatedBodyGraphicData != null)
                    pawnGraphicSet.dessicatedGraphic =
                                      curKindLifeStage.dessicatedBodyGraphicData.GraphicColoredFor(((Pawn)parent));
            }
        }
    }
}
