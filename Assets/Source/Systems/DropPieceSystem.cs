using System;
using System.Drawing;
using UnityEngine;
using Leopotam.Ecs;
using Scripts;
using Source.Components;
using Systems;

namespace Source.Systems
{
    internal sealed class DropPieceSystem : IEcsRunSystem
    {
        private readonly MyEngine myEngine;

        private readonly ChangePosition changePosition = new ChangePosition();
        private readonly EcsFilter<PositionComponent, PieceComponent, DropPieceEvent> droppedEntities = null!;
        private readonly EcsFilter<PositionComponent, PieceComponent>.Exclude<DropPieceEvent> entities = null!;

        public DropPieceSystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
        }

        public void Run()
        {
            foreach (var i in droppedEntities)
            {
                ref var entity = ref droppedEntities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().position;
                ref var piece = ref entity.Get<PieceComponent>().piece;
                ref var drag = ref piece.dragOffset;
                var targetPosition = new Point(position.X + (int) Math.Round(drag.x),
                    position.Y + (int) Math.Round(drag.y));

                var nullablePair = switchIfExist(pair: entity, targetPosition: targetPosition, from: position);
                if (nullablePair == null) continue;
                var pair = (EcsEntity) nullablePair;

                piece.canvasRenderer.transform.SetAsLastSibling();

                swap(entity: ref entity, pair: ref pair, targetPosition);
            }
        }

        private EcsEntity? switchIfExist(EcsEntity pair, Point targetPosition, Point from)
        {
            foreach (var i in entities)
            {
                ref var entity = ref entities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().position;
                ref var piece = ref entity.Get<PieceComponent>().piece;

                if (position != targetPosition) continue;
                if (piece.isBlocked) continue;
                if (piece.isDragged) continue;

                swap(entity: ref entity, pair: ref pair, newPosition: from);

                return entity;
            }
            return null;
        }

        private void swap(ref EcsEntity entity, ref EcsEntity pair, Point newPosition)
        {
            entity.Get<AwaitPairComponent>().pair = pair;

            changePosition.Change(ref entity, newPosition);
            myEngine.board[newPosition.X, newPosition.Y] = entity;
        }
    }
}