using System;
using RimWorld;
using Verse;
using Harmony;
namespace RimWorldBiomesCore
{
    public class AnimalGlow : CompGlower
    {
        //Credit to Erdelf and Jecrel for this code.
        //Code was obtained from StarWars-LightSabers Mod

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            try
            {
                Traverse.Create(this).Field("glowOnInt").SetValue(true);
                this.parent.MapHeld.glowGrid.RegisterGlower(this);
            }
            catch { }
        }

      
        public void GlowTick(object state)
        {
            try
            {
                this.parent.Map.glowGrid.MarkGlowGridDirty(this.parent.Position);
            }
            catch { }
            {
                //We're interested in this, but not the end users.
                //Log.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }


        public override void CompTick()
        {
            try
            {
                this.parent.Map.glowGrid.MarkGlowGridDirty(this.parent.Position);
            }
            catch { }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            try
            {
                this.parent.Map.glowGrid.DeRegisterGlower(this);
                base.PostDestroy(mode, previousMap);
            }
            catch { }
        }
    }
}
