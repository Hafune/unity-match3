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

        private readonly MyEngine myEngine;

        public RollbackSystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
        }

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

                    ref var piece = ref entity.Get<PieceComponent>();
                    ref var position = ref entity.Get<PositionComponent>().vec;
                    
                    piece.piece.drag = position - rollback.vec;
                    position.Set(rollback.vec.x, rollback.vec.y);

                    entity.Del<RollbackComponent>();
                    entity.Del<ReadyToRollBackComponent>();

                    myEngine.valuesBoard[(int) position.x, (int) position.y] = piece.value;
                }
            }
        }
    }
}