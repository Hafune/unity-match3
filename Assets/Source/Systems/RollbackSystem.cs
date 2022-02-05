using System;
using UnityEngine;

namespace Source.Systems
{
    using Leopotam.Ecs;
    using Components;

    internal sealed class RollbackSystem : IEcsRunSystem
    {
        private readonly EcsFilter<
                PieceComponent,
                PositionComponent,
                RollbackComponent,
                ReadyToRollBackComponent
            >
            entities = null!;

        public void Run()
        {
            if (entities.GetEntitiesCount() < 2) return;

            foreach (var e in entities)
            {
                ref var entity = ref entities.GetEntity(e);
                ref var rollback = ref entity.Get<RollbackComponent>();

                for (var i = 0; i < entities.GetEntitiesCount(); i++)
                {
                    if (rollback.pair != entities.GetEntity(i)) continue;
                    
                    ref var piece = ref entity.Get<PieceComponent>().piece;
                    ref var position = ref entity.Get<PositionComponent>().vec;
                        
                    piece.drag = position - rollback.vec;
                    position.Set(rollback.vec.x, rollback.vec.y);
                    
                    entity.Del<RollbackComponent>();
                    entity.Del<ReadyToRollBackComponent>();
                }
            }
        }
    }
}