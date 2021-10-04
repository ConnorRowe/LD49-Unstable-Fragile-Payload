using Godot;

namespace Unstable
{
    public class HeatZone : Area2D
    {
        private Player player;

        [Export]
        public float HeatPerSecond { get; set; } = 70f;
        [Export]
        public int ParticleRate { get; set; } = 80;

        private RectangleShape2D rectangleShape;
        private Particles2D fire;

        public override void _Ready()
        {
            fire = GetNode<Particles2D>("Particles2D");

            Connect("area_entered", this, nameof(AreaEntered));
            Connect("area_exited", this, nameof(AreaExited));

            rectangleShape = (RectangleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;

            (fire.ProcessMaterial as ParticlesMaterial).EmissionBoxExtents = new Vector3(rectangleShape.Extents.x, rectangleShape.Extents.y, 0f);
            fire.Amount = ParticleRate;
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (player != null && !player.IsDead)
            {
                player.AddHeat(HeatPerSecond * delta);
            }
        }

        private void AreaEntered(Area2D area)
        {
            if (area.GetParent() is Player p)
            {
                player = p;
            }
        }

        private void AreaExited(Area2D area)
        {
            if (area.GetParent() is Player p)
            {
                player = null;
            }
        }
    }
}
