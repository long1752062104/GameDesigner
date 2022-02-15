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
        Instance.RunPhysics(timeStep);
    }
}
