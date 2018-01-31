using System;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace RimWorldBiomesCore
{
    public class ProjectileAura : Projectile
    {
        private int ticksToDetonation;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        public override void Tick()
        {
            base.Tick();
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
        }

        protected override void Impact(Thing hitThing)
        {
            if (this.def.projectile.explosionDelay == 0)
            {
                this.Explode();
                return;
            }
            this.landed = true;
            this.ticksToDetonation = this.def.projectile.explosionDelay;
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
        }

        protected virtual void Explode()
        {
            Map map = base.Map;
            this.Destroy(DestroyMode.Vanish);
            if (this.def.projectile.explosionEffect != null)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(base.Position, map, false), new TargetInfo(base.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = base.Position;
            Map map2 = map;
            float explosionRadius = this.def.projectile.explosionRadius;
            DamageDef damageDef = this.def.projectile.damageDef;
            Thing launcher = this.launcher;
            int damageAmountBase = this.def.projectile.damageAmountBase;
            SoundDef soundExplode = this.def.projectile.soundExplode;
            ThingDef equipmentDef = this.equipmentDef;
            ThingDef def = this.def;
            GenerateAura(explosionRadius, position, map);
            ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, launcher, damageAmountBase, soundExplode, equipmentDef, def, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDealMoreDamageAtCenter);
        }

        private void GenerateAura(float explosionRadius, IntVec3 pos, Map map){
            CompPropertiesProjectileAura props;
            if(base.GetComp<CompProjectileAura>() != null){
                props = base.GetComp<CompProjectileAura>().Props;
            }else{
                return;
            }
            if (props.aura != null)
            {
                for (int i = (int)(pos.x - explosionRadius); i <= pos.x + explosionRadius; i++)
                {
                    for (int j = (int)(pos.z - explosionRadius); j <= pos.z + explosionRadius; j++)
                    {
                        IntVec3 temp = new IntVec3(i, 0, j);
                        if ((Math.Abs(pos.x - i) * Math.Abs(pos.x - i) + Math.Abs(pos.z - j) * Math.Abs(pos.z - j) < explosionRadius * explosionRadius) && temp.InBounds(map))
                        {
                            ThingDef particle = ThingDef.Named(props.aura);
                            List<ThingDef> thingdefs = new List<ThingDef>();
                            foreach (Thing t in temp.GetThingList(map))
                            {
                                thingdefs.Add(t.def);
                            }
                            if (particle.GetCompProperties<CompProperties_AuraParticle>() != null && temp.GetFirstBuilding(map) == null && !thingdefs.Contains(ThingDef.Named(props.aura)))
                            {
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
