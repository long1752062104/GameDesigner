#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using GGPhysUnity;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Lockstepcheck : MonoBehaviour
{
    public int index = 500;
    public int frame;
    private List<string> strs = new List<string>();
    public Button save;

    // Start is called before the first frame update
    void Start()
    {
        save.onClick.AddListener(()=> {
            File.WriteAllLines(Application.dataPath.Replace("Assets","data.txt"), strs);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (frame >= index)
            return;
        frame++;
        RigidPhysicsEngine.Instance.RunPhysics(0.01f);
        for (int i = 0; i < RigidPhysicsEngine.Instance.Bodies.Count; i++) 
        {
            strs.Add($"frame:{frame} {RigidPhysicsEngine.Instance.Bodies[i].Position}");
        }
    }
}
#endif