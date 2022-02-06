namespace Source.Systems
{
    using Leopotam.Ecs;
    using Components;
    using UnityEngine;
    using System.Collections.Generic;

    internal sealed class MatchSystem : IEcsRunSystem
    {
        private readonly bool[,] patternA = 
        {
            {true, true},
            {true, true}
        };

        private readonly bool[,] patternB = {
            {true, true, true},
        };

        private readonly bool[,] patternC = {
            {true},
            {true},
            {true},
        };

        private readonly EcsWorld world = null!;

        private readonly List<bool[,]> patterns;

        private readonly EcsFilter<
            StartComponent,
            ReadyToMatchComponent,
            PositionComponent,
            PieceComponent
        > newEntities = null!;

        private readonly EcsFilter<
            ReadyToMatchComponent,
            RollbackComponent,
            PositionComponent,
            PieceComponent
        > entities = null!;

        private readonly EcsFilter<PositionComponent>.Exclude<FallPositionComponent> freeEntities =
            null!;

        private readonly MyEngine myEngine;

        public MatchSystem(MyEngine myEngine)
        {
            this.myEngine = myEngine;

            patterns = new List<bool[,]>
            {
                patternA,
                patternB,
                patternC
            };
        }

        public void Run()
        {
            foreach (var e in newEntities)
            {
                ref var entity = ref newEntities.GetEntity(e);
                ref var position = ref entity.Get<PositionComponent>().vec;
                ref var piece = ref entity.Get<PieceComponent>();
                
                entity.Del<StartComponent>();
                entity.Del<ReadyToMatchComponent>();

                piece.piece.blocked = checkPatterns(position, piece.value);
            }

            if (entities.GetEntitiesCount() < 2) return;

            foreach (var e in entities)
            {
                ref var entity = ref entities.GetEntity(e);
                ref var rollback = ref entity.Get<RollbackComponent>();

                EcsEntity? p = null;
                for (var i = 0; i < entities.GetEntitiesCount(); i++)
                {
                    if (rollback.pair != entities.GetEntity(i)) continue;
                    p = rollback.pair;
                    break;
                }

                if (p == null) continue;
                var pair = (EcsEntity) p;

                ref var position = ref entity.Get<PositionComponent>().vec;
                ref var piece = ref entity.Get<PieceComponent>();

                ref var pairPos = ref pair.Get<PositionComponent>().vec;
                ref var pairPiece = ref pair.Get<PieceComponent>();

                myEngine.valuesBoard[(int) position.x, (int) position.y] = piece.value;
                myEngine.valuesBoard[(int) pairPos.x, (int) pairPos.y] = pairPiece.value;

                entity.Del<ReadyToMatchComponent>();
                
                if (checkPatterns(position, piece.value)) continue;
                entity.Get<ReadyToRollBackComponent>();
            }
        }

        private bool checkPatterns(Vector2 position, int key)
        {
            var set = new HashSet<string>();
            var total = new List<int>();

            foreach (var pattern in patterns)
            {
                checkPatternAllState(pattern, position, key, set, total);
            }

            for (var i = 0; i < total.Count; i += 2)
            {
                var pos = new Vector2(total[i], total[i + 1]);

                foreach (var o in freeEntities)
                {
                    var free = freeEntities.GetEntity(o);

                    if (free.Get<PositionComponent>().vec != pos) continue;

                    var piece = free.Get<PieceComponent>().piece;
                    free.Get<DestroyComponent>();

                    var entity = world.NewEntity();
                    entity.Get<FallPositionComponent>().vec = pos;
                    entity.Get<FallPieceComponent>().piece = piece;

                    piece.canvasRenderer.transform.SetAsLastSibling();
                    piece.blocked = true;

                    myEngine.valuesBoard[(int) position.x, (int) position.y] = null;
                }
            }

            return total.Count > 0;
        }

        private void checkPatternAllState(bool[,] pattern, Vector2 position, int key, ISet<string> set,
            ICollection<int> total)
        {
            for (var x = 0; x < pattern.GetLength(1); x++)
            {
                for (var y = 0; y < pattern.GetLength(0); y++)
                {
                    if (!pattern[y, x]) continue;

                    var vec = new Vector2(-x + position.x, -y + position.y);

                    var list = checkPattern(pattern, vec, key);

                    if (list == null) continue;

                    for (var i = 0; i < list.Count; i += 2)
                    {
                        var str = $"{list[i]}:{list[i + 1]}";
                        if (!set.Add(str)) continue;

                        total.Add(list[i]);
                        total.Add(list[i + 1]);
                    }
                }
            }
        }

        private List<int>? checkPattern(bool[,] pattern, Vector2 position, int key)
        {
            var list = new List<int>();

            for (var x = 0; x < pattern.GetLength(1); x++)
            {
                for (var y = 0; y < pattern.GetLength(0); y++)
                {
                    if (!pattern[y, x]) continue;
                    var vec = new Vector2(x + position.x, y + position.y);

                    var board = myEngine.valuesBoard;
                    if (vec.x < 0 ||
                        vec.y < 0 ||
                        vec.x >= board.GetLength(0) ||
                        vec.y >= board.GetLength(1)) return null;
                    var val = board[(int) vec.x, (int) vec.y];
                    if (val != key) return null;
                    list.Add((int) vec.x);
                    list.Add((int) vec.y);
                }
            }

            return list;
        }
    }
}