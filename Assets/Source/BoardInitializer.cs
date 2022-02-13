using Leopotam.Ecs;
using Scripts;
using Object = UnityEngine.Object;

namespace Systems
{
    public class BoardInitializer : Object
    {
        public readonly int width = 9;
        public readonly int height = 14;

        public BoardInitializer(MyEngine myEngine)
        {
            myEngine.valuesBoard = new EcsEntity?[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var _ = new SpawnPiece(myEngine, x, y);
                }
            }
        }
    }
}