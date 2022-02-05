using System;
using Leopotam.Ecs;

namespace Source.Components
{
    using UnityEngine;

    public struct RollbackComponent
    {
        public Vector2 vec;
        public EcsEntity pair;
    }
}