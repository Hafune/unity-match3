using System.Collections.Generic;
using System.Drawing;
using Leopotam.Ecs;
using Scripts;
using Source.Components;
using Systems;
using UnityEngine;

namespace Source.Systems
{
    internal sealed class DestroySystem : IEcsRunSystem
    {
        private readonly EcsFilter<PositionComponent, PieceComponent>.Exclude<DestroyComponent> alivePieces = null!;
        private readonly EcsFilter<DestroyComponent, PositionComponent> entities = null!;

        private readonly EcsWorld world = null!;
        private readonly MyEngine myEngine;
        private readonly HashSet<EcsEntity> slideEntities = new HashSet<EcsEntity>();

        public DestroySystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
        }

        public void Run()
        {
            var hasEmptySlots = false;
            foreach (var i in entities)
            {
                hasEmptySlots = true;
                ref var entity = ref entities.GetEntity(i);
                ref var piece = ref entity.Get<PieceComponent>().piece;
                ref var position = ref entity.Get<PositionComponent>().position;

                var fallEntity = world.NewEntity();
                fallEntity.Get<FallPositionComponent>().position =
                    new Vector2(position.X + .5f, position.Y + .5f);

                fallEntity.Get<FallPositionComponent>().vec = new Vector2(
                    (float) (MyEngine.random.NextDouble() * (piece.horizontalSpread * 2f) - piece.horizontalSpread),
                    (float) (MyEngine.random.NextDouble() * 2f));

                fallEntity.Get<FallPieceComponent>().piece = piece;

                piece.transform.SetParent(myEngine.fallBoard);
                piece.img.raycastTarget = false;

                foreach (var ecsEntity in sideUppers(position)) slideEntities.Add(ecsEntity);

                myEngine.score.text = (int.Parse(myEngine.score.text) + 1).ToString();

                myEngine.valuesBoard[position.X, position.Y] = null;
                entity.Destroy();
            }

            foreach (var entity in slideEntities)
            {
                ref var position = ref entity.Get<PositionComponent>().position;
                ref var offset = ref entity.Get<PositionComponent>().nextPositionOffset;

                myEngine.valuesBoard[position.X, position.Y] = null;
                position.X += offset.X;
                position.Y += offset.Y;
                offset.X = 0;
                offset.Y = 0;
            }

            foreach (var entity in slideEntities)
            {
                ref var position = ref entity.Get<PositionComponent>().position;
                entity.Get<PieceComponent>().piece.isBlocked = true;

                entity.Get<MoveComponent>();
                myEngine.valuesBoard[position.X, position.Y] = entity;
            }

            slideEntities.Clear();

            if (!hasEmptySlots) return;

            var width = myEngine.valuesBoard.GetLength(0);
            var height = myEngine.valuesBoard.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (myEngine.valuesBoard[x, y] == null)
                    {
                        new SpawnPiece(myEngine: myEngine, x: x, y: y);
                    }
                }
            }
        }

        private HashSet<EcsEntity> sideUppers(Point point)
        {
            var set = new HashSet<EcsEntity>();
            foreach (var i in alivePieces)
            {
                ref var entity = ref alivePieces.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().position;
                ref var pieceComponent = ref entity.Get<PieceComponent>();

                if (!(position.X == point.X && position.Y > point.Y)) continue;

                ref var offset = ref entity.Get<PositionComponent>().nextPositionOffset;

                offset.Y -= 1;

                pieceComponent.piece.dragOffset.y += 1f;
                set.Add(entity);
            }

            return set;
        }
    }
}