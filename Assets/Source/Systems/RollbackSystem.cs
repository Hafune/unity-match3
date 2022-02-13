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

                var pair = findPair.find(rollback.pair, entities);

                if (pair == null) continue;

                ref var pairPosition = ref ((EcsEntity) pair).Get<PositionComponent>().position;
                rollback.backPosition = pairPosition;
                
                set.Add(entity);
            }
            
            foreach (var entity in set)
            {
                ref var position = ref entity.Get<PositionComponent>().position;
                ref var rollbackPosition = ref entity.Get<RollbackComponent>().backPosition;
                ref var pieceComponent = ref entity.Get<PieceComponent>();

                pieceComponent.piece.dragOffset = new Vector2(position.X - rollbackPosition.X,
                    position.Y - rollbackPosition.Y);
                
                pieceComponent.piece.isBlocked = true;
                
                position.X = rollbackPosition.X;
                position.Y = rollbackPosition.Y;

                entity.Get<MoveComponent>();

                myEngine.valuesBoard[position.X, position.Y] = entity;
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