using Leopotam.Ecs;
using UnityEngine;

internal sealed class DestroySystem : IEcsRunSystem
{
    private readonly EcsFilter<DestroyComponent, PositionComponent> entities = null!;

    private readonly EcsWorld world = null!;
    private readonly MyEngine myEngine;

    public DestroySystem(MyEngine myEngine)
    {
        this.myEngine = myEngine;
    }

    public void Run()
    {
        foreach (var i in entities)
        {
            ref var entity = ref entities.GetEntity(i);
            ref var vec = ref entity.Get<PositionComponent>().vec;
            var _ = new SpawnPiece(myEngine, vec.x, vec.y);
        }

        foreach (var i in entities)
        {
            ref var entity = ref entities.GetEntity(i);
            ref var piece = ref entity.Get<PieceComponent>().piece;

            var fallEntity = world.NewEntity();
            fallEntity.Get<FallPositionComponent>().position =
                entity.Get<PositionComponent>().vec + new Vector2(.5f, .5f);

            fallEntity.Get<FallPositionComponent>().vec = new Vector2(
                (float) (MyEngine.random.NextDouble() * (piece.horizontalSpread * 2f) - piece.horizontalSpread),
                (float) (MyEngine.random.NextDouble() * 2f));

            fallEntity.Get<FallPieceComponent>().piece = piece;

            piece.transform.SetParent(myEngine.fallBoard);
            piece.img.raycastTarget = false;

            entity.Destroy();
        }
    }
}