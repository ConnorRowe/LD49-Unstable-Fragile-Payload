using Godot;
using System;

namespace Unstable
{
    public class MovingPlatform : KinematicBody2D
    {
        [Export]
        private NodePath pointAPath;
        [Export]
        private NodePath pointBPath;
        [Export]
        private float pathTime = 5f;

        private bool movingTowardsA = false;

        private Tween tween;
        private Node2D pointA;
        private Node2D pointB;

        public override void _Ready()
        {
            tween = GetNode<Tween>("Tween");
            pointA = GetNode<Node2D>(pointAPath);
            pointB = GetNode<Node2D>(pointBPath);

            tween.Connect("tween_all_completed", this, nameof(TweenAllCompleted));

            MoveToNextPoint();
        }

        private void TweenAllCompleted()
        {
            MoveToNextPoint();
        }

        private void MoveToNextPoint()
        {
            Vector2 nextPos = (movingTowardsA ? pointA : pointB).GlobalPosition;

            tween.InterpolateProperty(this, "global_position", GlobalPosition, nextPos, pathTime, easeType: Tween.EaseType.InOut, delay: 4f);
            tween.Start();

            movingTowardsA = !movingTowardsA;
        }

    }
}