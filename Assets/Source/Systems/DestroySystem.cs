using System.Collections.Generic;

namespace Source.Systems
{
    using Leopotam.Ecs;
    using Components;

    internal sealed class DestroySystem : IEcsRunSystem
    {
        private readonly EcsFilter<DestroyComponent, PositionComponent> entities = null!;
        private readonly EcsFilter<PieceComponent, RollbackComponent> entitiesWithRollBack = null!;
        
        private readonly EcsWorld world = null!;
        private readonly MyEngine myEngine;

        public DestroySystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
        }

        public void Run()
        {
            foreach (var e in entitiesWithRollBack)
            {
                ref var entity = ref entitiesWithRollBack.GetEntity(e);
                ref var rollback = ref entity.Get<RollbackComponent>();
            
                EcsEntity? p = null;
                for (var i = 0; i < entities.GetEntitiesCount(); i++)
                {
                    if (rollback.pair != entities.GetEntity(i)) continue;
                    p = rollback.pair;
                    break;
                }
                if (p == null) continue;
            
                entity.Del<ReadyToRollBackComponent>();
                entity.Del<RollbackComponent>();
            
                entity.Get<PieceComponent>().piece.blocked = false;
            }

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
                ref var vec = ref entity.Get<PositionComponent>().vec;

                var newEntity = world.NewEntity();
                newEntity.Get<FallPositionComponent>().vec = vec;
                newEntity.Get<FallPieceComponent>().piece = piece;
                piece.canvasRenderer.transform.SetAsLastSibling();
                
                entity.Destroy();
            }
        }
    }
}