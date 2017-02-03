using RimWorld;
using RimWorld.Planet;
using System;

namespace Caves
{
    public class Caves : BiomeWorker
    {
        private static Random random = new Random();

        public override float GetScore(Tile tile)
        {
            return (float)(10.0 * Caves.random.NextDouble());
        }
    }
}
