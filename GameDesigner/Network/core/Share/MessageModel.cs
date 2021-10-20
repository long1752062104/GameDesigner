namespace Net.Share
{
    using Newtonsoft_X.Json;
    using global::System.Collections.Generic;
    using Net.Serialize;

    /// <summary>
    /// web服务器json参数类
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// 记录参数类型
        /// </summary>
        public string typeName;
        /// <summary>
        /// 参数json解析字符串
        /// </summary>
        public string jsonStr;

        /// <summary>
        /// 构造
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="jsonStr"></param>
        public Parameter(string typeName, string jsonStr)
        {
            this.typeName = typeName;
            this.jsonStr = jsonStr;
        }
    }

    /// <summary>
    /// webSocket服务器json数据模型基础类
    /// </summary>
    public abstract class MessageModelBase
    {
        /// <summary>
        /// 操作指令
        /// </summary>
        public byte cmd;
        /// <summary>
        /// 操作的远程函数
        /// </summary>
        public string func;
        /// <summary>
        /// 操作的远程参数
        /// </summary>
        public Parameter[] pars;

        /// <summary>
        /// 解析客户端的json数据, 获取参数 
        /// </summary>
        /// <returns></returns>
        public object[] GetPars()
        {
            if (pars == null)
                return null;
            object[] pars1 = new object[pars.Length];
            for (int i = 0; i < pars.Length; i++)
            {
                global::System.Type type = NetConvertOld.GetType(pars[i].typeName);
                object obj = JsonConvert.DeserializeObject(pars[i].jsonStr, type);
                pars1[i] = obj;
            }
            return pars1;
        }
    }

    /// <summary>
    /// webSocket服务器json数据模型类
    /// </summary>
    public class MessageModel : MessageModelBase
    {
        /// <summary>
        /// 构造
        /// </summary>
        public MessageModel()
        {
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public MessageModel(byte cmd, string func, object[] pars)
        {
            this.cmd = cmd;
            this.func = func;
            if (pars == null)
                return;
            List<Parameter> pars1 = new List<Parameter>();
            foreach (object p in pars)
            {
                if (p == null)
                {
                    pars1.Add(null);
                    continue;
                }
                global::System.Type type = p.GetType();
                string json = JsonConvert.SerializeObject(p);
                Parameter par = new Parameter(type.FullName, json);
                pars1.Add(par);
            }
            this.pars = pars1.ToArray();
        }
    }
}
