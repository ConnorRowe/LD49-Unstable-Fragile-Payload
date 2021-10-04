using Godot;
using System;

public class TwitterLink : Label
{
    private bool mouseOver = false;

    public override void _Ready()
    {
        Connect("mouse_entered", this, nameof(MouseEntered));
        Connect("mouse_exited", this, nameof(MouseExited));
    }

    public override void _Input(InputEvent evt)
    {
        base._Input(evt);

        if (mouseOver && evt is InputEventMouseButton emb && !emb.Pressed)
        {
            OS.ShellOpen("https://twitter.com/MagsonConnor");
        }
    }

    private void MouseEntered()
    {
        mouseOver = true;
    }

    private void MouseExited()
    {
        mouseOver = false;
    }
}
