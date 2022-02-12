using Leopotam.Ecs;
using UnityEngine;

namespace Source.Components
{
    public struct RollbackComponent
    {
        public Vector2 backPosition;
        public EcsEntity pair;
    }
}