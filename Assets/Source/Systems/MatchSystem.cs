using Leopotam.Ecs;
using System.Collections.Generic;
using System.Drawing;
using Scripts;
using Source.Components;

namespace Source.Systems
{
    internal sealed class MatchSystem : IEcsRunSystem
    {
        private readonly bool[,] patternA =
        {
            {true, true},
            {true, true}
        };

        private readonly bool[,] patternB =
        {
            {true, true, true},
        };

        private readonly bool[,] patternC =
        {
            {true},
            {true},
            {true},
        };

        private readonly List<bool[,]> patterns;

        private readonly EcsFilter<
            MatchComponent,
            PositionComponent,
            PieceComponent
        >.Exclude<AwaitPairComponent> entities = null!;

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
            if (entities.GetEntitiesCount() < 2) return;

            foreach (var e in entities)
            {
                ref var entity = ref entities.GetEntity(e);

                ref var vec = ref entity.Get<PositionComponent>().position;
                ref var piece = ref entity.Get<PieceComponent>();

                entity.Del<MatchComponent>();

                checkPatterns(vec, piece.value);
            }
        }

        private void checkPatterns(Point position, int value)
        {
            foreach (var pattern in patterns)
            {
                checkPatternAllState(pattern, position, value);
            }
        }

        private void checkPatternAllState(bool[,] pattern, Point position, int value)
        {
            for (var x = 0; x < pattern.GetLength(1); x++)
            {
                for (var y = 0; y < pattern.GetLength(0); y++)
                {
                    if (!pattern[y, x]) continue;

                    var vec = new Point(-x + position.X, -y + position.Y);

                    checkPattern(pattern, vec, value);
                }
            }
        }

        private void checkPattern(bool[,] pattern, Point position, int value)
        {
            var list = new HashSet<EcsEntity>();

            for (var x = 0; x < pattern.GetLength(1); x++)
            {
                for (var y = 0; y < pattern.GetLength(0); y++)
                {
                    if (!pattern[y, x]) continue;
                    var vec = new Point(x + position.X, y + position.Y);

                    var board = myEngine.board;
                    if (vec.X < 0 ||
                        vec.Y < 0 ||
                        vec.X >= board.GetLength(0) ||
                        vec.Y >= board.GetLength(1)) return;

                    var entity = board[vec.X, vec.Y];

                    if (entity == null) return;
                    if (((EcsEntity) entity).Get<PieceComponent>().value != value) return;

                    list.Add((EcsEntity) entity);
                }
            }

            foreach (var entity in list)
            {
                entity.Get<DestroyComponent>();
            }
        }
    }
}