using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGTest : MonoBehaviour
{
    public int num = 1000;
    public int currnum;
    public GameObject @object;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Ins", 0.1f, 0.1f);
    }

    void Ins()
    {
        if (currnum > num)
            return;
        currnum++;
        Instantiate(@object, transform.position, transform.rotation);
    }
}
