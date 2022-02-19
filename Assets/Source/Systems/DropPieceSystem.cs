using System;
using System.Drawing;
using UnityEngine;
using Leopotam.Ecs;
using Scripts;
using Source.Components;

namespace Source.Systems
{
    internal sealed class DropPieceSystem : IEcsRunSystem
    {
        private readonly MyEngine myEngine;

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
                ref var pieceComponent = ref entity.Get<PieceComponent>();
                ref var drag = ref pieceComponent.piece.dragOffset;
                var targetPosition = new Point(position.X + (int) Math.Round(drag.x),
                    position.Y + (int) Math.Round(drag.y));

                var pair = switchIfExist(pair: entity, targetPosition: targetPosition, from: position);
                if (pair == null) continue;

                entity.Get<AwaitPairComponent>().pair = (EcsEntity) pair;

                pieceComponent.piece.dragOffset =
                    new Vector2(position.X + drag.x - targetPosition.X,
                        position.Y + drag.y - targetPosition.Y);

                position.X = targetPosition.X;
                position.Y = targetPosition.Y;
                entity.Get<MoveComponent>();

                myEngine.valuesBoard[position.X, position.Y] = entity;
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

                piece.canvasRenderer.transform.SetAsLastSibling();

                entity.Get<AwaitPairComponent>().pair = pair;

                piece.dragOffset = new Vector2(position.X - from.X, position.Y - from.Y);
                piece.isBlocked = true;

                position.X = from.X;
                position.Y = from.Y;
                entity.Get<MoveComponent>();

                myEngine.valuesBoard[position.X, position.Y] = entity;

                return entity;
            }

            return null;
        }
    }
}