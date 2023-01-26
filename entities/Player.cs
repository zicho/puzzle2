using Core;
using Godot;
using System;

namespace Entities;
public partial class Player : BaseEntity
{
    public Camera2D Cam => GetNode<Camera2D>("Cam");

    public bool CamControl { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        InAir = DirectionIsEmpty(MapPosition + ParentMap.CurrentGravity);

        if (InAir && !IsMoving)
        {
            Fall();
        }

        if (Map.PlayerControlEnabled && !CamControl)
        {
            if (Input.IsActionJustPressed("spin_right") && !InAir)
            {
                CamRotate(90);
            }

            if (Input.IsActionJustPressed("spin_left") && !InAir)
            {
                CamRotate(-90);
            }

            if (Input.IsActionJustPressed("flip") && !InAir)
            {
                CamRotate(180, flip: true);
            }

            if (Input.IsActionPressed("move_right") && !IsMoving)
            {
                Move(GetGravityNeutralDirection(Vector2.Right));
            }

            if (Input.IsActionPressed("move_left") && !IsMoving)
            {
                Move(GetGravityNeutralDirection(Vector2.Left));
            }
        }
    }

    private void CamRotate(int degrees, bool flip = false)
    {
        CamControl = true;
        var camRotationTween = CreateTween();
        var playerRotationTween = CreateTween();

        camRotationTween.Finished += () => ParentMap.UpdateRotation(degrees, flip);

        playerRotationTween
            .TweenProperty(GetNode<Sprite2D>("Sprite"),
            "rotation",
            Cam.Rotation + Mathf.DegToRad(degrees),
            0.2f);

        camRotationTween
            .TweenProperty(Cam,
            "rotation",
            Cam.Rotation + Mathf.DegToRad(degrees),
            0.2f);

        playerRotationTween.Play();
        camRotationTween.Play();

        camRotationTween.Finished += () => CamControl = false;
    }
}
