#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace Example2
{
    using Net.Component;
    using Net.Share;

    public class SceneManager : Net.Component.SceneManager
    {
        public override void OnOperationOther(Operation opt)
        {
            switch (opt.cmd) 
            {
                case Command.Fire:
                    if(transforms.TryGetValue(opt.index, out NetworkTransformBase t))
                    {
                        t.GetComponent<Player>().Fire();
                    }
                    break;
            }
        }
    }
}
#endif