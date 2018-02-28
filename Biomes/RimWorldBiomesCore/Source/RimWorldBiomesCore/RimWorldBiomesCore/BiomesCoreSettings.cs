using System;
using RimWorld;
using Verse;
using System.Xml;
namespace RimWorldBiomesCore
{
    public class BiomesCoreSettings : ModSettings
    {
        public bool enableWaterPathing = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.enableWaterPathing, "enableWaterPathing", true);
        }
    }

    public class CheckWaterPathingSetting : PatchOperation{
        protected override bool ApplyWorker(XmlDocument xml)
        {
            //Log.Error(BiomesCavesMod.settings.enableAnimalLight.ToString());
            return BiomesCoreMod.settings.enableWaterPathing;
        }
    }

}
