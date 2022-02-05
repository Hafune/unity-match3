using System;
using System.Collections.Generic;
using Leopotam.Ecs;
using Source.Components;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Source
{
    public class BoardInitializer : Object
    {
        public readonly int width = 9;
        public readonly int height = 14;

        private readonly Random random = new Random();

        public BoardInitializer(MyEngine myEngine)
        {
            var world = myEngine.world;
            var sprites = myEngine.sprites;
            var nodePiece = myEngine.nodePiece;
            var gameBoard = myEngine.gameBoard;
            myEngine.valuesBoard = new int?[width, height];

            var canvas = gameBoard.parent.GetComponent<Canvas>();
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var entity = world.NewEntity();
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
                    myEngine.valuesBoard[x, y] = piece.value;

                    piece.piece.rect.anchoredPosition = position.vec;
                }
            }
        }
    }
}