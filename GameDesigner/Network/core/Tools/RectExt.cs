using UnityEngine;

public static class RectExt
{
    public static Vector2 PointToAxis(Rect rectangle, Vector2 point)
    {
        var normalized = Rect.PointToNormalized(rectangle, point);
        var axis = new Vector2(normalized.x * 2f - 1f, normalized.y * 2f - 1f);
        return axis;
    }
}