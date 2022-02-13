﻿using System;
using Leopotam.Ecs;
using Scripts;
using Source.Components;
using UnityEngine;

namespace Source.Systems
{
    internal sealed class MovePieceSystem : IEcsRunSystem
    {
        private readonly Rect boardRect;
        private readonly MyEngine myEngine;

        private readonly EcsFilter<MoveComponent, PositionComponent, PieceComponent> entities = null!;

        public MovePieceSystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
            boardRect = myEngine.gameBoard.rect;
        }

        public void Run()
        {
            var rect = boardRect;
            var width = myEngine.boardInitializer.width;
            var height = myEngine.boardInitializer.height;

            var halfWidth = rect.width / width / 2;
            var halfHeight = rect.height / height / 2;

            foreach (var i in entities)
            {
                ref var entity = ref entities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().position;
                ref var piece = ref entity.Get<PieceComponent>().piece;

                ref var drag = ref piece.dragOffset;

                if (!piece.isDragged || piece.isBlocked)
                {
                    if (drag.magnitude > 0) drag -= drag * Time.deltaTime * 20;

                    if (Math.Abs(drag.x) < .01) drag.x = 0f;
                    if (Math.Abs(drag.y) < .01) drag.y = 0f;

                    if (drag.magnitude == 0)
                    {
                        entity.Get<MatchComponent>();
                        entity.Del<MoveComponent>();
                        piece.isBlocked = false;
                    }
                }

                var vec = new Vector2(position.X * rect.width / width + halfWidth,
                    position.Y * rect.height / height + halfHeight);

                piece.rect.anchoredPosition =
                    vec + piece.dragOffset * myEngine.pixelPerMeter;
            }
        }
    }
}