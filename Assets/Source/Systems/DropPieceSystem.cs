using System;
using UnityEngine;
using Leopotam.Ecs;

internal sealed class DropPieceSystem : IEcsRunSystem
{
    private readonly MyEngine myEngine;

    private readonly EcsFilter<PieceComponent> justDroppedEntities = null!;
    private readonly EcsFilter<PositionComponent, PieceComponent, DropPieceEvent> droppedEntities = null!;
    private readonly EcsFilter<PositionComponent, PieceComponent>.Exclude<DropPieceEvent> entities = null!;

    public DropPieceSystem(MyEngine myEngine)
    {
        this.myEngine = myEngine;
    }

    public void Run()
    {
        foreach (var i in justDroppedEntities)
        {
            ref var entity = ref justDroppedEntities.GetEntity(i);
            ref var piece = ref entity.Get<PieceComponent>().piece;
            if (piece.isDragged) entity.Get<MoveComponent>();

            if (!piece.justPointerUp) continue;

            piece.isBlocked = true;
            piece.isDragged = false;
            piece.justPointerUp = false;
            entity.Get<DropPieceEvent>();
        }

        foreach (var i in droppedEntities)
        {
            ref var entity = ref droppedEntities.GetEntity(i);
            ref var position = ref entity.Get<PositionComponent>().vec;
            ref var pieceComponent = ref entity.Get<PieceComponent>();
            ref var drag = ref pieceComponent.piece.dragOffset;
            var roundPosition = position + new Vector2((float) Math.Round(drag.x), (float) Math.Round(drag.y));

            var pair = switchIfExist(pair: entity, point: roundPosition, from: position);
            if (pair == null) continue;

            entity.Get<AwaitPairComponent>().startPosition = position;
            entity.Get<AwaitPairComponent>().pair = (EcsEntity) pair;

            pieceComponent.piece.dragOffset = position + drag - roundPosition;

            position.Set(roundPosition.x, roundPosition.y);
            entity.Get<MoveComponent>();

            myEngine.valuesBoard[(int) position.x, (int) position.y] = pieceComponent.value;
        }
    }

    private EcsEntity? switchIfExist(EcsEntity pair, Vector2 point, Vector2 from)
    {
        foreach (var i in entities)
        {
            ref var entity = ref entities.GetEntity(i);
            ref var position = ref entity.Get<PositionComponent>().vec;
            ref var pieceComponent = ref entity.Get<PieceComponent>();

            if (position != point) continue;
            if (pieceComponent.piece.isBlocked) continue;

            pieceComponent.piece.canvasRenderer.transform.SetAsLastSibling();

            entity.Get<AwaitPairComponent>().startPosition = position;
            entity.Get<AwaitPairComponent>().pair = pair;

            pieceComponent.piece.dragOffset = position - from;
            pieceComponent.piece.isBlocked = true;

            position.Set(from.x, from.y);
            entity.Get<MoveComponent>();

            myEngine.valuesBoard[(int) position.x, (int) position.y] = pieceComponent.value;

            return entity;
        }

        return null;
    }
}