using Godot;

namespace Unstable
{
    public class PlayerDeadChunks : Node2D
    {
        private RigidBody2D[] chunks;
        public override void _Ready()
        {
            chunks = new RigidBody2D[4] { GetNode<RigidBody2D>("RigidBody2D"), GetNode<RigidBody2D>("RigidBody2D2"), GetNode<RigidBody2D>("RigidBody2D3"), GetNode<RigidBody2D>("RigidBody2D4") };

            foreach (var chunk in chunks)
            {
                chunk.ApplyImpulse(chunk.ToLocal(GlobalPosition) + new Vector2(Helpers.Rng.RandfRange(-5f, 5f), Helpers.Rng.RandfRange(-5f, 5f)), Helpers.Rng.RandfRange(50f, 200f) * GlobalPosition.DirectionTo(chunk.GlobalPosition));
            }
        }
    }
}