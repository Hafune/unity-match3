﻿using System.Drawing;
using Leopotam.Ecs;
using Scripts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems
{
    public class BoardInitializer : Object
    {
        public BoardInitializer(MyEngine myEngine)
        {
            var width = myEngine.width;
            var height = myEngine.height;

            myEngine.board = new EcsEntity?[width, height];
            
            var size = new Vector2(width * 64, height * 64);
            myEngine.gameBoard.GetComponent<RectTransform>().sizeDelta = size;
            myEngine.fallBoard.GetComponent<RectTransform>().sizeDelta = size;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var _ = new SpawnPiece(myEngine, x, y);
                }
            }
        }
    }
}