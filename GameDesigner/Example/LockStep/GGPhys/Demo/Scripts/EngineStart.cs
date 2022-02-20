using GGPhysUnity;

public class EngineStart : RigidPhysicsEngine
{
    public static EngineStart I;

    private void Start()
    {
        I = this;
    }

    private void Update()
    {
        if (autoStep)
        {
            Step();
        }
    }

    public void Step()
    {
        for (int i = 0; i < stepCount; i++)
            Instance.RunPhysics(timeStep);
    }
}
