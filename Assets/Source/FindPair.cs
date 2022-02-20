using Leopotam.Ecs;

namespace Systems
{
    public class FindPair
    {
        public EcsEntity? find(ref EcsEntity entity, EcsFilter entities)
        {
            for (var i = 0; i < entities.GetEntitiesCount(); i++)
            {
                if (entity != entities.GetEntity(i)) continue;
                return entity;
            }

            return null;
        }
    }
}