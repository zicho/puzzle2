using Core;
using Godot;

namespace Entities;

public partial class BaseEntity : Node2D
{
    public Map ParentMap => GetParent<Map>();

    public bool IsMoving { get; protected set; }
    public bool InAir { get; protected set; }

    public Vector2 MapPosition => ParentMap.LocalToMap(Position);

    public override void _Process(double delta)
    {
        InAir = DirectionIsEmpty(MapPosition + ParentMap.CurrentGravity);

        if (InAir && !IsMoving)
        {
            Fall();
        }
    }

    public virtual void Fall()
    {
        var newPos = GetGravityNeutralDirection(MapPosition + ParentMap.CurrentGravity);

        if (DirectionIsEmpty(newPos))
        {
            var movementTween = CreateTween();

            movementTween
                .TweenProperty(this,
                "position",
                ParentMap.MapToLocal((Vector2i)newPos),
                0.1f)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        }
    }

    public virtual void Move(Vector2 direction)
    {
        var newPos = ParentMap.LocalToMap(Position) + direction;

        if (DirectionIsEmpty(newPos))
        {
            IsMoving = true;

            var movementTween = CreateTween();

            movementTween
                .TweenProperty(this,
                "position",
                ParentMap.MapToLocal((Vector2i)newPos),
                0.1f)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);

            movementTween.Finished += () => IsMoving = false;
        }
    }

    public bool DirectionIsEmpty(Vector2 direction)
    {
        var coords = new Vector2i((int)direction.x, (int)direction.y);

        var tileIsEmpty = ParentMap.GetCellSourceId(0, coords) == -1;

        // if (tileIsEmpty)
        // {
        //     var checkEntity = ParentMap.GetEntityAtTile(coords);
        //     if (checkEntity == null) return true;
        //     if (checkEntity != null) return checkEntity.CanFall || CheckGlide(checkEntity);
        // }

        return tileIsEmpty;
    }

    protected Vector2 GetGravityNeutralDirection(Vector2 dir)
    {
        if (dir == Vector2.Left && ParentMap.CurrentGravity != Vector2.Down)
        {
            if (ParentMap.CurrentGravity == Vector2.Left) return dir.Rotated(Mathf.DegToRad(90));
            else if (ParentMap.CurrentGravity == Vector2.Up) return dir.Rotated(Mathf.DegToRad(180));
            else if (ParentMap.CurrentGravity == Vector2.Right) return dir.Rotated(Mathf.DegToRad(-90));
        }

        if (dir == Vector2.Right && ParentMap.CurrentGravity != Vector2.Down)
        {
            if (ParentMap.CurrentGravity == Vector2.Left) return dir.Rotated(Mathf.DegToRad(-270));
            else if (ParentMap.CurrentGravity == Vector2.Up) return dir.Rotated(Mathf.DegToRad(-180));
            else if (ParentMap.CurrentGravity == Vector2.Right) return dir.Rotated(Mathf.DegToRad(270));
        }

        return dir;
    }
}