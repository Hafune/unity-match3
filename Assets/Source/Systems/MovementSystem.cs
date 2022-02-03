using System;
using Leopotam.Ecs;
using Source.Components;
using UnityEngine;

namespace Source.Systems
{
    internal sealed class MovementSystem : IEcsRunSystem
    {
        private readonly Rect boardRect;
        private readonly EcsGameStartup ecsGameStartup;

        //Auto inject
        // private readonly EcsWorld world = null!;

        //Auto inject
        private readonly EcsFilter<PositionComponent, PieceComponent> entities = null!;

        public MovementSystem(EcsGameStartup ecsGameStartup)
        {
            this.ecsGameStartup = ecsGameStartup;
            boardRect = ecsGameStartup.gameBoard.rect;
        }

        public void Run()
        {
            var rect = boardRect;
            var width = ecsGameStartup.boardInitializer.width;
            var height = ecsGameStartup.boardInitializer.height;

            var halfWidth = rect.width / width / 2;
            var halfHeight = rect.height / height / 2;

            foreach (var i in entities)
            {
                ref var entity = ref entities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>();
                ref var piece = ref entity.Get<PieceComponent>();

                var vec = new Vector2(position.vec.x * rect.width / width + halfWidth,
                    position.vec.y * rect.height / height + halfHeight);

                ref var drag = ref piece.piece.drag;
                if (!piece.piece.isDragged && drag.magnitude > 0)
                {
                    drag -= drag * Time.deltaTime * 10;
                    if (Math.Abs(drag.x) < 1) drag.x = 0f;
                    if (Math.Abs(drag.y) < 1) drag.y = 0f;
                }

                piece.piece.rect.anchoredPosition =
                    vec + piece.piece.drag;
            }
        }
    }
}