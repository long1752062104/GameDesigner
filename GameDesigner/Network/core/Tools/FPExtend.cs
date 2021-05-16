using UnityEngine;

public static class FPExtend
{
    /// <summary>
    /// 定点数, 默认只保留浮点数最好4位
    /// </summary>
    /// <param name="self"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static float ToFloat(this float self, float num = 1000f)
    {
        return (int)(self * num) / num;
    }

    /// <summary>
    /// 定点数, 默认只保留浮点数最好4位
    /// </summary>
    /// <param name="self"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static Vector3 ToVector3(this Vector3 self, float num = 1000f)
    {
        return new Vector3(self.x.ToFloat(num), self.y.ToFloat(num), self.z.ToFloat(num));
    }

    /// <summary>
    /// 定点数, 默认只保留浮点数最好4位
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static Vector3 ToVector3Int(this Vector3 self)
    {
        return new Vector3((int)self.x, (int)self.y, (int)self.z);
    }

    /// <summary>
    /// 定点平移
    /// </summary>
    /// <param name="self"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="num"></param>
    public static void FPTranslate(this Transform self, Vector3 dir, float num = 100f)
    {
        FPTranslate(self, dir.x, dir.y, dir.z, num);
    }

    /// <summary>
    /// 定点平移
    /// </summary>
    /// <param name="self"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="num"></param>
    public static void FPTranslate(this Transform self, float x, float y, float z, float num = 100f)
    {
        Vector3 translation = new Vector3(x, y, z);
        Vector3 direction = self.TransformDirection(translation);
        self.position += new Vector3(direction.x.ToFloat(num), direction.y.ToFloat(num), direction.z.ToFloat(num));
        self.position = new Vector3(self.position.x.ToFloat(num), self.position.y.ToFloat(num), self.position.z.ToFloat(num));
    }

    /// <summary>
    /// 定点转向
    /// </summary>
    /// <param name="self"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="num"></param>
    public static void FPRotate(this Transform self, float x, float y, float z, float num = 100f)
    {
        self.Rotate(x, y, z);
        self.rotation = new Quaternion(self.rotation.x.ToFloat(num), self.rotation.y.ToFloat(num), self.rotation.z.ToFloat(num), self.rotation.w.ToFloat(num));
    }
}