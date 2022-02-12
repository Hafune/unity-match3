using Leopotam.Ecs;
using Source;

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
            fallEntity.Get<FallPositionComponent>().vec = entity.Get<PositionComponent>().vec;
            fallEntity.Get<FallPieceComponent>().piece = piece;
            piece.transform.SetParent(myEngine.fallBoard);

            entity.Destroy();
        }
    }
}