using System.Collections;
using System.Collections.Generic;
using GGPhys.Core;
using REAL = FixMath.FP;

public struct CollisionRay
{
    public Vector3d start;
    public Vector3d end;
    public Vector3d direction;
    public uint layerMask;

    public Vector3d gridStart;
    public Vector3d gridEnd;

    public void Init(Vector3d start, Vector3d end, Vector3d direction, uint layerMask)
    {
        this.start = start;
        this.end = end;
        this.direction = direction;
        this.layerMask = layerMask;
        gridStart = start;
        gridEnd = start;
    }
}
