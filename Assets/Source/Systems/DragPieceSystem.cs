using System;
using Leopotam.Ecs;
using Source.Components;
using UnityEngine;

namespace Source.Systems
{
    sealed class DragPieceSystem : IEcsRunSystem
    {
        private readonly EcsFilter<PieceComponent> justDroppedEntities = null!;

        public void Run()
        {
            foreach (var i in justDroppedEntities)
            {
                ref var entity = ref justDroppedEntities.GetEntity(i);
                ref var piece = ref entity.Get<PieceComponent>().piece;

                if (piece.isDragged)
                {
                    entity.Get<MoveComponent>();
                    var rVec = piece.realDragOffset;

                    var normVec = rVec.magnitude > 1 ? rVec.normalized : rVec;

                    if (Math.Abs(normVec.x) > Math.Abs(normVec.y)) normVec.y /= 4f;
                    else normVec.x /= 4f;

                    piece.dragOffset = Vector2.Lerp(piece.dragOffset, normVec, Time.deltaTime * 30);
                }

                if (!piece.justPointerUp) continue;

                piece.isDragged = false;
                piece.isBlocked = true;
                piece.justPointerUp = false;
                entity.Get<MoveComponent>();
                entity.Get<DropPieceEvent>();
            }
        }
    }
}