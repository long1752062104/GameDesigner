using System;
using UnityEngine;

namespace GameDesigner
{
    [System.Serializable]
    public class ConvertUtility
    {
        /// <summary>
        /// 解释 : 获得str字符串中的所有字符,当出现与oldValue相同的字符时更换为newValue的字符,(当出现与oldValue相同或有n个,只更换最后一个为newValue的字符)
        /// </summary>

        static public string ReplaceEndToOne(string str, string oldValue, string newValue)
        {
            string[] array = str.Split(oldValue.ToCharArray());

            string typeName = null;

            for (int i = 0; i < array.Length; i++)
            {
                if (i == array.Length - 1)
                    typeName += newValue + array[i];
                else if (i == array.Length - 2)
                    typeName += array[i];
                else
                    typeName += array[i] + oldValue;
            }

            if (array.Length == 1)
                return str;

            return typeName;
        }

        /// <summary>
        /// 解释 : 获得str字符串中的所有字符,当出现与oldValue相同的字符时进行拆分,拆分的字符只返回后面的字符
        /// 解释1 : 获得str字符串中的所有字符,当str字符中出现与oldValue字符相同的字符时进行拆分,拆分后返回后面的字符(遇到与oldValue的字符作废)
        /// </summary>

        static public string ReplaceEndToOne(string str, string oldValue)
        {
            string[] array = str.Split(new string[] { oldValue }, System.StringSplitOptions.RemoveEmptyEntries);

            return array[array.Length - 1];
        }

        /// <summary>
        /// 拆分str字符 ， 当str的字符内容中包含了split的字符则进行拆分进数组 ， 此方法只返回数组[0]的字符 (只有一行代码 return str.Split( split )[0];) ( str 要读取的全部字符串 , split 拆分符号或其他，当遇到这个字符即返回从开始到此字符的字段 )
        /// </summary>

        static public string StringSplit(string str, char split = '.')
        {
            if (str == null)
                return "";
            return str.Split(split)[0];
        }

        /// <summary>
        /// 拆分str字符 ， 当str的字符内容中包含了split的字符则进行拆分进数组 ， 此方法只返回数组[0]的字符 (只有一行代码 return str.Split( split )[array.Length-1];) ( str 要读取的全部字符串 , split 拆分符号或其他，当遇到这个字符即返回从开始到此字符的字段 )
        /// </summary>

        static public string StringSplitEndOne(string str, char split = '.')
        {
            if (str == null)
                return "";
            string[] array = str.Split(split);
            return str.Split(split)[array.Length - 1];
        }

        /// <summary>
        /// 拆分字符串从0字符段开始读到出现split拆分字符后结束并返回当前段的字符 ( str 要读取的全部字符串 , split 拆分符号或其他，当遇到这个字符即返回从开始到此字符的字段 )
        /// </summary>

        static public string StringSplitEndToOne(string str, char split = '.')
        {
            string[] array = str.Split(split);

            string typeName = null;

            for (int i = 0; i < array.Length - 1; i++)
            {
                if (i == array.Length - 2)
                    typeName += array[i];
                else
                    typeName += array[i] + split;
            }

            return typeName;
        }

        /// <summary>
        /// 字符转三维向量值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object ToVector2_3_4(string type = "UnityEngine.Vector3", string value = "( 0.1 , 0.5 , 1 )")
        {
            return To_Vector234_Rect_Quaternion_Color(type, value);
        }

        /// <summary>
        /// 字符转矩形值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object ToRect(string type = "UnityEngine.Rect", string value = "( 0.1 , 0.5 , 1 , 1 )")
        {
            return To_Vector234_Rect_Quaternion_Color(type, value);
        }

        /// <summary>
        /// 字符转颜色值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object ToColor(string type = "UnityEngine.Color", string value = "( 0.1 , 0.5 , 1 , 1 )")
        {
            return To_Vector234_Rect_Quaternion_Color(type, value);
        }

        /// <summary>
        /// 字符转欧拉角值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object ToQuaternion(string type = "UnityEngine.Quaternion", string value = "( 0.1 , 0.5 , 1 , 1 )")
        {
            return To_Vector234_Rect_Quaternion_Color(type, value);
        }

        /// <summary>
        /// 转换字符为Vector2 或 Vector3 或 Vector4 或 Rect 或 Quaternion 的值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object To_Vector234_Rect_Quaternion_Color(string type = "UnityEngine.Vector3", string value = "( 0.1 , 0.5 , 1 )")
        {
            value = value.Trim();
            value = value.TrimStart('(');
            value = value.TrimEnd(')');
            string[] array = value.Split(',');

            if (array.Length == 2 & type == "UnityEngine.Vector2") return new Vector2(System.Convert.ToSingle(array[0]), System.Convert.ToSingle(array[1]));

            if (array.Length == 3 & type == "UnityEngine.Vector3") return new Vector3(System.Convert.ToSingle(array[0]), System.Convert.ToSingle(array[1]), System.Convert.ToSingle(array[2]));

            if (array.Length == 4 & type == "UnityEngine.Vector4") return new Vector4(System.Convert.ToSingle(array[0]), System.Convert.ToSingle(array[1]), System.Convert.ToSingle(array[2]), System.Convert.ToSingle(array[3]));

            if (array.Length == 4 & type == "UnityEngine.Rect") return new Rect(System.Convert.ToSingle(array[0].Replace("x:", "")), System.Convert.ToSingle(array[1].Replace("y:", "")), System.Convert.ToSingle(array[2].Replace("width:", "")), System.Convert.ToSingle(array[3].Replace("height:", "")));

            if (array.Length == 4 & type == "UnityEngine.Quaternion") return new Quaternion(System.Convert.ToSingle(array[0]), System.Convert.ToSingle(array[1]), System.Convert.ToSingle(array[2]), System.Convert.ToSingle(array[3]));

            if (array.Length == 4 & type == "UnityEngine.Color") return new Color(System.Convert.ToSingle(array[0].Replace("RGBA(", "")), System.Convert.ToSingle(array[1]), System.Convert.ToSingle(array[2]), System.Convert.ToSingle(array[3]));

            throw new Exception("反序列化失败！");
        }

        /// <summary>
        /// 三维向量转完整字符 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object Vector2_3_4ToString(object value)
        {
            return Vector234_Rect_Quaternion_ColorToString(value);
        }

        /// <summary>
        /// 字符转矩形值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object RectToString(object value)
        {
            return Vector234_Rect_Quaternion_ColorToString(value);
        }

        /// <summary>
        /// 字符转颜色值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object ColorToString(object value)
        {
            return Vector234_Rect_Quaternion_ColorToString(value);
        }

        /// <summary>
        /// 字符转欧拉角值 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object QuaternionToString(object value)
        {
            return Vector234_Rect_Quaternion_ColorToString(value);
        }

        static public string BooleanToString(object boolean)
        {
            if (boolean is bool)
            {
                return (bool)boolean ? "true" : "false";
            }
            return "false";
        }

        /// <summary>
        /// 将Vector2 或 Vector3 或 Vector4 或 Rect 或 Quaternion 的值转换成变量声明的完整字符 ( type 以字符形式给定一个类型 , value 类型的值 注：类型的值必须以给定类型的值符合才能转换成功 )
        /// </summary>

        static public object Vector234_Rect_Quaternion_ColorToString(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is Vector2)
            {
                Vector2 v2 = (Vector2)value;
                return "(" + v2.x + "F, " + v2.y + "F)";
            }

            if (value is Vector3)
            {
                Vector3 v3 = (Vector3)value;
                return "(" + v3.x + "F, " + v3.y + "F, " + v3.z + "F)";
            }

            if (value is Vector4)
            {
                Vector4 v4 = (Vector4)value;
                return "(" + v4.x + "F, " + v4.y + "F, " + v4.z + "F, " + v4.w + "F)";
            }

            if (value is Rect)
            {
                Rect v4 = (Rect)value;
                return "(" + v4.x + "F, " + v4.y + "F, " + v4.width + "F, " + v4.height + "F)";
            }

            if (value is Color)
            {
                Color v4 = (Color)value;
                return "(" + v4.r + "F, " + v4.g + "F, " + v4.b + "F, " + v4.a + "F)";
            }

            if (value is Quaternion)
            {
                Quaternion v4 = (Quaternion)value;
                return "(" + v4.x + "F, " + v4.y + "F, " + v4.z + "F, " + v4.w + "F)";
            }

            return null;
        }
    }
}