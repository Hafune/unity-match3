using System;
using Leopotam.Ecs;
using Scripts;
using Source.Components;
using UnityEngine;

namespace Source.Systems
{
    internal sealed class FallSystem : IEcsRunSystem
    {
        private readonly MyEngine myEngine;

        private readonly EcsFilter<FallPieceComponent, FallPositionComponent> entities = null!;

        public FallSystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
        }

        public void Run()
        {
            foreach (var i in entities)
            {
                ref var entity = ref entities.GetEntity(i);
                ref var piece = ref entity.Get<FallPieceComponent>().piece;
                ref var position = ref entity.Get<FallPositionComponent>().position;

                ref var val = ref entity.Get<FallPositionComponent>().vec;
                val.x -= Math.Sign(val.x) * .1f * piece.fallSpeed * Time.deltaTime;
                val.y -= 2 * piece.fallSpeed * Time.deltaTime;

                position += val * Time.deltaTime * piece.fallSpeed;

                var vec = new Vector2(position.x * myEngine.pixelPerMeter,
                    position.y * myEngine.pixelPerMeter);

                piece.rect.anchoredPosition = vec;

                if (!(piece.rect.anchoredPosition.y < -piece.img.minHeight)) continue;

                piece.DestroyScriptInstance();
                entity.Destroy();
            }
        }
    }
}