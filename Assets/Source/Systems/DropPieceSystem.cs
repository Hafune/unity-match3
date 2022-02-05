using System;
using UnityEngine;

namespace Source.Systems
{
    using Leopotam.Ecs;
    using Components;

    internal sealed class DropPieceSystem : IEcsRunSystem
    {
        //Auto inject
        // private readonly EcsWorld world = null!;

        //Auto inject
        private readonly EcsFilter<PieceComponent> justDroppedEntities = null!;
        private readonly EcsFilter<PositionComponent, PieceComponent, DropPieceEvent> droppedEntities = null!;
        private readonly EcsFilter<PositionComponent, PieceComponent>.Exclude<DropPieceEvent> entities = null!;

        public void Run()
        {
            foreach (var i in justDroppedEntities)
            {
                ref var entity = ref justDroppedEntities.GetEntity(i);
                ref var piece = ref entity.Get<PieceComponent>().piece;

                if (!piece.justPointerUp) continue;
                piece.blocked = true;
                piece.isDragged = false;
                piece.justPointerUp = false;
                entity.Get<DropPieceEvent>();
            }
                
                
            foreach (var i in droppedEntities)
            {
                ref var entity = ref droppedEntities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().vec;
                ref var piece = ref entity.Get<PieceComponent>().piece;
                ref var drag = ref piece.drag;
                var roundPosition = position + new Vector2((float) Math.Round(drag.x), (float) Math.Round(drag.y));

                var nullableEntity = switchIfExist(entity, roundPosition, position);
                if (nullableEntity == null) continue;

                entity.Get<RollbackComponent>().vec = position;
                entity.Get<RollbackComponent>().pair = (EcsEntity) nullableEntity;

                piece.drag = position + drag - roundPosition;
                position.Set(roundPosition.x, roundPosition.y);
            }
        }

        private EcsEntity? switchIfExist(EcsEntity pair, Vector2 point, Vector2 from)
        {
            foreach (var i in entities)
            {
                ref var entity = ref entities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().vec;
                ref var piece = ref entity.Get<PieceComponent>().piece;
                
                if (position != point) continue;
                if (piece.blocked) continue;

                piece.canvasRenderer.transform.SetAsLastSibling();

                entity.Get<RollbackComponent>().vec = position;
                entity.Get<RollbackComponent>().pair = pair;

                piece.drag = position - from;
                position.Set(from.x, from.y);
                piece.blocked = true;

                return entity;
            }

            return null;
        }
    }
}