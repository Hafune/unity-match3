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
        private readonly EcsFilter<DestroyComponent, PositionComponent> entities = null!;

        private readonly EcsWorld world = null!;
        private readonly MyEngine myEngine;

        private readonly ChangePosition changePosition = new ChangePosition();

        public DestroySystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
        }

        public void Run()
        {
            var hasEmptySlots = false;
            var board = myEngine.board;
            foreach (var i in entities)
            {
                ref var entity = ref entities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().position;
                board[position.X, position.Y] = null;
            }

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

                moveAround(position);

                myEngine.score.text = (int.Parse(myEngine.score.text) + 1).ToString();

                entity.Destroy();
            }

            if (!hasEmptySlots) return;

            var width = board.GetLength(0);
            var height = board.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (board[x, y] == null)
                    {
                        new SpawnPiece(myEngine: myEngine, x: x, y: y);
                    }
                }
            }
        }

        private void moveAround(Point freePoint)
        {
            if (movePiece(freePoint.X + 1, freePoint.Y)) moveAround(new Point(freePoint.X + 1, freePoint.Y));

            if (movePiece(freePoint.X - 1, freePoint.Y)) moveAround(new Point(freePoint.X - 1, freePoint.Y));

            if (movePiece(freePoint.X, freePoint.Y + 1)) moveAround(new Point(freePoint.X, freePoint.Y + 1));

            if (movePiece(freePoint.X, freePoint.Y - 1)) moveAround(new Point(freePoint.X, freePoint.Y - 1));
        }

        private bool movePiece(int x, int y)
        {
            var width = myEngine.board.GetLength(0);
            var height = myEngine.board.GetLength(1);
            if (x < 0 || x >= width || y < 0 || y >= height) return false;

            ref var e = ref myEngine.board[x, y];
            if (e == null) return false;
            var entity = (EcsEntity) e;

            if (y - 1 < 0 || myEngine.board[x, y - 1] != null) return false;

            myEngine.board[x, y] = null;
            myEngine.board[x, y - 1] = entity;
            
            changePosition.Change(ref entity, new Point(x, y - 1));
            
            movePiece(x, y - 1);

            return true;
        }
    }
}