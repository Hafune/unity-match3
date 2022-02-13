using Leopotam.Ecs;
using Source.Components;

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
                
                // ref var lastDrag = ref piece.lastDragOffset;
                // ref var drag = ref piece.dragOffset;
                
                // var nDir = drag.normalized;
                // var aDir = new Vector2(Math.Abs(drag.x), Math.Abs(drag.y));

                // if (piece.isDragged && drag.magnitude >= 1f)
                // {
                //     entity.Get<MoveComponent>();
                //     piece.dragOffset = aDir.x > aDir.y
                //         ? new Vector2(nDir.x > 0 ? 1f : -1f, 0f)
                //         : new Vector2(0f, nDir.y > 0 ? 1f : -1f);
                // } 
                
                if (piece.isDragged) entity.Get<MoveComponent>();

                if (!piece.justPointerUp) continue;
                
                piece.isBlocked = true;
                piece.justPointerUp = false;
                entity.Get<DropPieceEvent>();
            }
        }
    }
}