using System;
using Leopotam.Ecs;
using Source.Components;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Source
{
    public class SpawnPiece : Object
    {
        private static readonly Random random = new Random();

        public SpawnPiece(MyEngine myEngine, float x, float y)
        {
            var world = myEngine.world;
            var sprites = myEngine.sprites;
            var nodePiece = myEngine.nodePiece;
            var gameBoard = myEngine.gameBoard;

            var canvas = gameBoard.parent.GetComponent<Canvas>();
            var entity = world.NewEntity();
            
            entity.Get<MoveComponent>();
            ref var position = ref entity.Get<PositionComponent>();
            ref var piece = ref entity.Get<PieceComponent>();

            var index = Convert.ToInt16(random.NextDouble() * (sprites.Length - 1));
            var obj = Instantiate(nodePiece, gameBoard);

            piece.value = index;
            piece.piece = obj.GetComponent<Piece>();
            piece.piece.Initialize(myEngine);
            piece.piece.img.sprite = sprites[index];
            piece.piece.canvas = canvas;

            position.vec.x = x;
            position.vec.y = y;
            myEngine.valuesBoard[(int) x, (int) y] = piece.value;

            piece.piece.rect.anchoredPosition = position.vec;

            piece.piece.blocked = true;
            piece.piece.dragOffset = new Vector2((float) (random.NextDouble() * 4f - 2),
                (float) (random.NextDouble() * 4f - 2));
        }
    }
}