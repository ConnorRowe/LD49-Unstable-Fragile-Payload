using Godot;
using System;

namespace Unstable
{
    public class Bullet : KinematicBody2D
    {
        public Vector2 Velocity { get; set; } = Vector2.Zero;


        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            var tryMove = MoveAndCollide(Velocity, false);

            if (tryMove != null)
            {
                if (tryMove.Collider is Player player)
                {
                    player.AddHeat(80f);
                }

                Die();
            }
        }

        private void Die()
        {
            QueueFree();
        }

    }
}