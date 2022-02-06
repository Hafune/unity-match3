namespace Source.Systems
{
    using System;
    using Leopotam.Ecs;
    using Components;
    using UnityEngine;

    internal sealed class FallSystem : IEcsRunSystem
    {
        private readonly Rect boardRect;
        private readonly MyEngine myEngine;

        //Auto inject
        // private readonly EcsWorld world = null!;

        //Auto inject
        private readonly EcsFilter<FallPieceComponent, FallPositionComponent> entities = null!;

        public FallSystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;
            boardRect = myEngine.gameBoard.rect;
        }

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
}