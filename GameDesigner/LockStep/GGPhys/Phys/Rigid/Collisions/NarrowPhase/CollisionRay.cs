using TrueSync;

public struct CollisionRay
{
    public TSVector3 start;
    public TSVector3 end;
    public TSVector3 direction;
    public uint layerMask;

    public TSVector3 gridStart;
    public TSVector3 gridEnd;

    public void Init(TSVector3 start, TSVector3 end, TSVector3 direction, uint layerMask)
    {
        this.start = start;
        this.end = end;
        this.direction = direction;
        this.layerMask = layerMask;
        gridStart = start;
        gridEnd = start;
    }
}
