using System;
using RimWorld;
using Verse;
namespace RimWorldBiomesCore
{
    public class CompAnimalGlower : ThingComp
    {
        //Credit to Erdelf and Jecrel for this code.
        //Code was obtained from StarWars-LightSabers Mod

        bool init = false;

        public override void CompTick(){
            if(!init){
                AnimalGlow ag;
                init = true;
                (this.parent as Pawn).AllComps.Add(ag = new AnimalGlow()
                {
                    parent = this.parent,
                    props = new CompProperties_Glower()
                    {
                        compClass = typeof(AnimalGlow),
                        glowRadius = Props.glowRadius,
                        glowColor = Props.glowColor,
                        overlightRadius = 5f
                    }
                });
                ag.PostSpawnSetup(false);
            }
        }

        public CompPropertiesAnimalGlower Props
        {
            get
            {
                return (CompPropertiesAnimalGlower)this.props;
            }
        }
    }
}
