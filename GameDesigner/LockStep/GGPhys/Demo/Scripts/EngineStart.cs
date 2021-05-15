using GGPhysUnity;

public class EngineStart : RigidPhysicsEngine
{
    public bool autoUpdate;

    protected override void Awake()
    {
        base.Awake();
        enabled = autoUpdate;
    }

    private void Update()
    {
        Instance.RunPhysics(0.01f/*Time.deltaTime*/);
    }
}
