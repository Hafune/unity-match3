using System.Drawing;
using Leopotam.Ecs;
using Source.Components;
using UnityEngine;

namespace Systems
{
    public class ChangePosition
    {
        public void Change(ref EcsEntity entity, Point newPosition)
        {
            ref var position = ref entity.Get<PositionComponent>().position;
            ref var piece = ref entity.Get<PieceComponent>().piece;

            piece.dragOffset = new Vector2(position.X - newPosition.X + piece.dragOffset.x,
                position.Y - newPosition.Y + piece.dragOffset.y);
                
            piece.isBlocked = true;
                
            position.X = newPosition.X;
            position.Y = newPosition.Y;
            
            entity.Get<MoveComponent>();
        }
    }
}