using Leopotam.Ecs;

internal sealed class AwaitPairSystem : IEcsRunSystem
{
    private readonly EcsFilter<AwaitPairComponent, MatchComponent> entities = null!;

    private readonly FindPair findPair = new FindPair();

    public void Run()
    {
        if (entities.GetEntitiesCount() < 2) return;

        foreach (var e in entities)
        {
            ref var entity = ref entities.GetEntity(e);
            ref var pairComponent = ref entity.Get<AwaitPairComponent>();

            if (findPair.find(pairComponent.pair, entities) == null) continue;

            entity.Get<RollbackComponent>().backPosition = pairComponent.startPosition;
            entity.Get<RollbackComponent>().pair = pairComponent.pair;

            entity.Del<AwaitPairComponent>();
        }
    }
}