using System.Drawing;
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
            var layout = myEngine.startLevel;
            myEngine.width = layout.list[0].images.Count;
            myEngine.height = layout.list.Count;
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
                    var sprite = layout.list[height - y - 1].images[x];
                    if (sprite == null) continue;
                    
                    if (myEngine.sprites.IndexOf(sprite) == -1) myEngine.sprites.Add(sprite);
                    new SpawnPiece(myEngine, x, y, sprite);
                }
            }
        }
    }
}