using System;
using RimWorld;
using RimWorld.Planet;
using Verse;
namespace RimWorldBiomesCaves
{
    public class BiomeWorker_Cavern : BiomeWorker
    {
            public override float GetScore(Tile tile)
        {
            if(tile.hilliness != Hilliness.Impassable){
                return -100f;
            }
            return 100f;
        }
    }
}
