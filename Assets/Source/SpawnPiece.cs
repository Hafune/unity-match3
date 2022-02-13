using System;
using Leopotam.Ecs;
using Scripts;
using Source.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems
{
    public class SpawnPiece : Object
    {
        public SpawnPiece(MyEngine myEngine, int x, int y)
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

            var index = Convert.ToInt16(MyEngine.random.NextDouble() * (sprites.Length - 1));
            var obj = Instantiate(nodePiece, gameBoard);

            piece.value = index;
            piece.piece = obj.GetComponent<Piece>();
            piece.piece.Initialize(myEngine);
            piece.piece.img.sprite = sprites[index];
            piece.piece.canvas = canvas;

            position.position.X = x;
            position.position.Y = y;
            myEngine.valuesBoard[x, y] = entity;

            var rect = piece.piece.rect.rect;
            piece.piece.rect.anchoredPosition = new Vector2(-rect.width,-rect.height);

            piece.piece.isBlocked = true;
            piece.piece.dragOffset = new Vector2(0f, 14f);
        }
    }
}