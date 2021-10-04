using Godot;
using System;

namespace Unstable
{
    public class Turret : Area2D
    {
        private float count = 0;
        private Player player;
        private Sprite gun;
        private Timer shootTimer;
        private AudioStreamPlayer shootPlayer;

        private PackedScene bulletScene;

        public override void _Ready()
        {
            base._Ready();

            gun = GetNode<Sprite>("Gun");
            shootTimer = GetNode<Timer>("ShootTimer");
            shootPlayer = GetNode<AudioStreamPlayer>("ShootPlayer");

            bulletScene = GD.Load<PackedScene>("res://scenes/Bullet.tscn");

            Connect("area_entered", this, nameof(AreaEntered));
            Connect("area_exited", this, nameof(AreaExited));
            shootTimer.Connect("timeout", this, nameof(TryShoot));
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            count += delta * 0.25f;

            if (count >= 1f)
                count--;

            Position += new Vector2(0f, Mathf.Sin(count * Mathf.Tau) * delta * 7.5f);

            if (player != null)
            {
                var vec = gun.GlobalPosition.DirectionTo(player.GlobalPosition);
                var rot = Mathf.Atan2(vec.y, vec.x);
                gun.Rotation = Mathf.LerpAngle(gun.Rotation, rot, delta * 3f);
            }
        }

        private void AreaEntered(Area2D area)
        {
            if (area.GetParent() is Player p)
            {
                player = p;
                shootTimer.Start();
            }
        }

        private void AreaExited(Area2D area)
        {
            if (area.GetParent() is Player p)
            {
                player = null;
                shootTimer.Stop();
            }
        }

        private void TryShoot()
        {
            if (player != null && !player.IsDead)
            {
                shootPlayer.Play();

                var bullet = bulletScene.Instance<Bullet>();

                GetParent().AddChild(bullet);
                var dir = gun.GlobalPosition.DirectionTo(player.GlobalPosition);
                bullet.GlobalPosition = gun.GlobalPosition + (dir * 10f);
                bullet.Velocity = dir * 1f;
            }
        }
    }
}
