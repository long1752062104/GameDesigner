using UnityEngine;

public static class OtherExtend
{
    /// <summary>
    /// 向量相乘
    /// </summary>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Vector3 Multiply(this Vector3 self, Vector3 value)
    {
        return new Vector3(self.x * value.x, self.y * value.y, self.z * value.z);
    }
}
