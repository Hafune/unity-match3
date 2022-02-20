using System.Collections.Generic;
using Leopotam.Ecs;
using Scripts;
using Source.Components;
using Systems;
using UnityEngine;

namespace Source.Systems
{
    internal sealed class RollbackSystem : IEcsRunSystem
    {
        private readonly EcsFilter<
                PieceComponent,
                PositionComponent,
                RollbackComponent
            >
            entities = null!;

        private readonly MyEngine myEngine;

        private readonly FindPair findPair = new FindPair();
        private readonly ChangePosition changePosition = new ChangePosition();
        private readonly HashSet<EcsEntity> set = new HashSet<EcsEntity>();


        public RollbackSystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
        }

        public void Run()
        {
            foreach (var e in entities)
            {
                ref var entity = ref entities.GetEntity(e);
                ref var rollback = ref entity.Get<RollbackComponent>();

                var pair = findPair.find(ref rollback.pair, entities);
                if (pair == null) continue;

                ref var pairPosition = ref ((EcsEntity) pair).Get<PositionComponent>().position;
                rollback.backPosition = pairPosition;

                set.Add(entity);
            }

            foreach (var entity in set)
            {
                var e = entity;
                changePosition.Change(ref e, entity.Get<RollbackComponent>().backPosition);
                ref var position = ref entity.Get<PositionComponent>().position;

                myEngine.board[position.X, position.Y] = entity;
            }

            foreach (var e in entities)
            {
                ref var entity = ref entities.GetEntity(e);
                entity.Del<RollbackComponent>();
            }

            set.Clear();
        }
    }
}