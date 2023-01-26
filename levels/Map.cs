using Entities;
using Godot;
using System;

namespace Core;
public partial class Map : TileMap
{

    public Vector2 CurrentGravity;
    private int _selectedGravity = 0;
    public Player Player => GetNode<Player>("Player");

    public static readonly Vector2[] Gravities = new[] {
        Vector2.Down,
        Vector2.Left,
        Vector2.Up,
        Vector2.Right,
    };

    public static bool PlayerControlEnabled { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CurrentGravity = Gravities[_selectedGravity];
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        PlayerControlEnabled = !Player.IsMoving && !Player.InAir;
    }

    public void UpdateRotation(int degrees, bool flip)
    {
        if (flip)
        {
            // TODO: Refactor this ugly shit
            if (CurrentGravity == Vector2.Down)
            {
                _selectedGravity = 2;
            }
            else if (CurrentGravity == Vector2.Up)
            {
                _selectedGravity = 0;
            }
            else if (CurrentGravity == Vector2.Right)
            {
                _selectedGravity = 1;
            }
            else if (CurrentGravity == Vector2.Left)
            {
                _selectedGravity = 3;
            }
        }
        else
        {
            if (degrees > 0)
                _selectedGravity++;
            else
                _selectedGravity--;

            if (_selectedGravity > Gravities.Length - 1)
                _selectedGravity = 0;
            else if (_selectedGravity < 0)
                _selectedGravity = Gravities.Length - 1;
        }


        CurrentGravity = Gravities[_selectedGravity];
    }
}
