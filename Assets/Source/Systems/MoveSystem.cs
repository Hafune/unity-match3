using System;
using Leopotam.Ecs;
using UnityEngine;

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
            ref var position = ref entity.Get<PositionComponent>().vec;
            ref var piece = ref entity.Get<PieceComponent>().piece;

            ref var drag = ref piece.dragOffset;

            if (!piece.isDragged)
            {
                drag -= drag * Time.deltaTime * 20;
                if (Math.Abs(drag.x) < .01) drag.x = 0f;
                if (Math.Abs(drag.y) < .01) drag.y = 0f;

                if (drag.magnitude == 0)
                {
                    entity.Get<MatchComponent>();
                    entity.Del<MoveComponent>();
                    piece.blocked = false;
                }
            }

            var vec = new Vector2(position.x * rect.width / width + halfWidth,
                position.y * rect.height / height + halfHeight);

            piece.rect.anchoredPosition =
                vec + piece.dragOffset * myEngine.pixelPerMeter;
        }
    }
}