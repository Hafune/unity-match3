using Leopotam.Ecs;
using UnityEngine;

internal sealed class FallSystem : IEcsRunSystem
{
    private readonly EcsFilter<FallPieceComponent, FallPositionComponent> entities = null!;

    public void Run()
    {
        foreach (var i in entities)
        {
            ref var entity = ref entities.GetEntity(i);
            ref var piece = ref entity.Get<FallPieceComponent>().piece;


            var rectAnchoredPosition = piece.rect.anchoredPosition;
            rectAnchoredPosition.y -= 150 * Time.deltaTime;
            piece.rect.anchoredPosition = rectAnchoredPosition;

            if (!(rectAnchoredPosition.y < -piece.rect.rect.height)) continue;

            piece.DestroyScriptInstance();
            entity.Destroy();
        }
    }
}