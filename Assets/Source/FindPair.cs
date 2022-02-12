using Leopotam.Ecs;

public class FindPair
{
    public EcsEntity? find(EcsEntity entity, EcsFilter entities)
    {
        for (var i = 0; i < entities.GetEntitiesCount(); i++)
        {
            if (entity != entities.GetEntity(i)) continue;
            return entity;
        }

        return null;
    }
}