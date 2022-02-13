using System.Drawing;
using Leopotam.Ecs;

namespace Source.Components
{
    public struct RollbackComponent
    {
        public Point backPosition;
        public EcsEntity pair;
    }
}