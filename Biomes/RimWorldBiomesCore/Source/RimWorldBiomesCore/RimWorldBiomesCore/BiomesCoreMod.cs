using System;
using RimWorld;
using Verse;
namespace RimWorldBiomesCore
{
    public class BiomesCoreMod : Mod
    {
        public static BiomesCoreSettings settings;

        public BiomesCoreMod(ModContentPack content) : base(content){
            settings = GetSettings<BiomesCoreSettings>();
        }

        public override string SettingsCategory() => "BiomesCoreSettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("EnableWaterPathingLabel".Translate() + ": ", ref settings.enableWaterPathing);
            listing_Standard.End();
            settings.Write();
        }
    }
}
