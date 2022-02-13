using Leopotam.Ecs;
using Source.Components;
using Systems;

namespace Source.Systems
{
    internal sealed class AwaitPairSystem : IEcsRunSystem
    {
        private readonly EcsFilter<AwaitPairComponent, MatchComponent> entities = null!;
        private readonly EcsFilter<AwaitPairComponent, PieceComponent> awaits = null!;

        private readonly FindPair findPair = new FindPair();

        public void Run()
        {
            foreach (var e in entities)
            {
                ref var entity = ref entities.GetEntity(e);
                ref var awaitComponent = ref entity.Get<AwaitPairComponent>();

                var pair = findPair.find(awaitComponent.pair, entities);
                if (pair == null) continue;

                entity.Get<RollbackComponent>().pair = awaitComponent.pair;

                entity.Del<AwaitPairComponent>();
            }

            foreach (var e in awaits)
            {
                ref var entity = ref awaits.GetEntity(e);
                ref var awaitComponent = ref entity.Get<AwaitPairComponent>();

                if (findPair.find(awaitComponent.pair, awaits) != null) continue;

                entity.Get<PieceComponent>().piece.isBlocked = false;
                entity.Del<AwaitPairComponent>();
            }
        }
    }
}