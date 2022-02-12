using Leopotam.Ecs;

internal sealed class RollbackSystem : IEcsRunSystem
{
    private readonly EcsFilter<
            PieceComponent,
            PositionComponent,
            RollbackComponent
        >
        entities = null!;

    private readonly MyEngine myEngine;

    private readonly FindPair findPair = new FindPair();

    public RollbackSystem(MyEngine myEngine)
    {
        this.myEngine = myEngine;
    }

    public void Run()
    {
        foreach (var e in entities)
        {
            ref var entity = ref entities.GetEntity(e);
            ref var rollback = ref entity.Get<RollbackComponent>();
            ref var pieceComponent = ref entity.Get<PieceComponent>();

            if (findPair.find(rollback.pair, entities) != null)
            {
                ref var position = ref entity.Get<PositionComponent>().vec;

                pieceComponent.piece.dragOffset = position - rollback.backPosition;
                pieceComponent.piece.isBlocked = true;

                position.Set(rollback.backPosition.x, rollback.backPosition.y);
                entity.Get<MoveComponent>();

                myEngine.valuesBoard[(int) position.x, (int) position.y] = pieceComponent.value;
            }

            entity.Del<RollbackComponent>();
        }
    }
}