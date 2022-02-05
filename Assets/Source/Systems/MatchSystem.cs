namespace Source.Systems
{
    using Leopotam.Ecs;
    using Components;
    using UnityEngine;
    using System.Collections.Generic;

    internal sealed class MatchSystem : IEcsRunSystem
    {
        private readonly bool[,] patternA = new bool[3, 3]
        {
            {false, true, false},
            {true, true, true},
            {false, true, false}
        };

        private readonly bool[,] patternB = new bool[1, 3]
        {
            {true, true, true},
        };

        private readonly bool[,] patternC = new bool[3, 1]
        {
            {true},
            {true},
            {true},
        };

        private readonly List<bool[,]> patterns;

        private readonly EcsFilter<RollbackComponent, PositionComponent, PieceComponent, CheckMatchComponent> entities =
            null!;

        private readonly EcsFilter<PositionComponent>.Exclude<FallComponent> freeEntities =
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
            foreach (var i in entities)
            {
                ref var entity = ref entities.GetEntity(i);
                ref var position = ref entity.Get<PositionComponent>().vec;
                ref var piece = ref entity.Get<PieceComponent>();

                entity.Del<CheckMatchComponent>();

                entity.Get<ReadyToRollBackComponent>();
                checkPatterns(position, piece.value);
            }
        }

        private void checkPatterns(Vector2 position, int key)
        {
            foreach (var pattern in patterns)
            {
                checkPatternAllState(pattern, position, key);
            }
        }

        private void checkPatternAllState(bool[,] pattern, Vector2 position, int key)
        {
            for (var x = 0; x < pattern.GetLength(0); x++)
            {
                for (var y = 0; y < pattern.GetLength(1); y++)
                {
                    if (!pattern[x, y]) continue;
                    var vec = new Vector2(x + position.x, y + position.y);

                    var list = checkPattern(pattern, vec, key);
                    if (list == null) continue;

                    for (var i = 0; i < list.Count; i += 2)
                    {
                        var pos = new Vector2(list[i], list[i + 1]);

                        foreach (var o in freeEntities)
                        {
                            var free = freeEntities.GetEntity(o);

                            if (free.Get<PositionComponent>().vec != pos) continue;
                            
                            free.Get<FallComponent>();
                            Debug.Log("FallComponent");
                        }
                    }
                }
            }
        }

        private List<int>? checkPattern(bool[,] pattern, Vector2 position, int key)
        {
            var list = new List<int>();

            for (var x = 0; x < pattern.GetLength(0); x++)
            {
                for (var y = 0; y < pattern.GetLength(1); y++)
                {
                    if (!pattern[x, y]) continue;
                    var vec = new Vector2(x + position.x, y + position.y);

                    var board = myEngine.valuesBoard;
                    if (vec.x < 0 ||
                        vec.y < 0 ||
                        vec.x >= board.GetLength(0) ||
                        vec.y >= board.GetLength(1)) return null;
                    if (board[(int) vec.x, (int) vec.y] != key) return null;
                    list.Add((int) vec.x);
                    list.Add((int) vec.y);
                }
            }

            return list;
        }
    }
}