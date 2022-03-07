#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// GameObject同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.GameObject))]
    public class NetworkGameObject : NetworkBehaviour
    {
        private UnityEngine.GameObject self;
        public bool autoCheck;
        private System.Int32 layer1;
        private System.Boolean isStatic2;
        private System.String tag3;
        private System.String name4;
        private UnityEngine.HideFlags hideFlags5;
        private System.String type1;
        private System.String methodName2;
        private UnityEngine.SendMessageOptions options3;
        private System.String methodName4;
        private UnityEngine.SendMessageOptions options5;
        private System.String methodName6;
        private UnityEngine.SendMessageOptions options7;
        private System.String methodName8;
        private System.String methodName9;
        private System.String methodName10;
        private bool activeSelf11;

        public override void Awake()
        {
            base.Awake();
            self = gameObject;
            layer1 = self.layer;
            isStatic2 = self.isStatic;
            tag3 = self.tag;
            name4 = self.name;
            hideFlags5 = self.hideFlags;
            activeSelf11 = self.activeSelf;
        }

        void Start() { }//让监视面板能显示启动勾选

        public override bool CheckEnabled()
        {
            return true;
        }

        public System.Int32 layer
        {
            get
            {
                return self.layer;
            }
            set
            {
                if (layer1 == value)
                    return;
                layer1 = value;
                self.layer = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 1,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean isStatic
        {
            get
            {
                return self.isStatic;
            }
            set
            {
                if (isStatic2 == value)
                    return;
                isStatic2 = value;
                self.isStatic = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 5,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.String tag
        {
            get
            {
                return self.tag;
            }
            set
            {
                if (tag3 == value)
                    return;
                tag3 = value;
                self.tag = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 6,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.String name
        {
            get
            {
                return self.name;
            }
            set
            {
                if (name4 == value)
                    return;
                name4 = value;
                self.name = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 23,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.HideFlags hideFlags
        {
            get
            {
                return self.hideFlags;
            }
            set
            {
                if (hideFlags5 == value)
                    return;
                hideFlags5 = value;
                self.hideFlags = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 24,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public bool activeSelf
        {
            get
            {
                return self.activeSelf;
            }
            set
            {
                if (activeSelf11 == value)
                    return;
                activeSelf11 = value;
                self.SetActive(value);
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 25,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public void GetComponent(System.String type, bool always = false)
        {
            if (type == type1 & !always) return;
            type1 = type;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { type } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 27,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName2 & options == options3 & !always) return;
            methodName2 = methodName;
            options3 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 53,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName4 & options == options5 & !always) return;
            methodName4 = methodName;
            options5 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 54,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName6 & options == options7 & !always) return;
            methodName6 = methodName;
            options7 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 55,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, bool always = false)
        {
            if (methodName == methodName8 & !always) return;
            methodName8 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 74,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName9 & !always) return;
            methodName9 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 77,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName10 & !always) return;
            methodName10 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 80,
                buffer = buffer
            });
        }
        public void GetInstanceID(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 101,
                buffer = buffer
            });
        }
        public void GetHashCode(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 102,
                buffer = buffer
            });
        }
        public void ToString(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 108,
                buffer = buffer
            });
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;
            layer = layer;
            isStatic = isStatic;
            tag = tag;
            name = name;
            hideFlags = hideFlags;
            activeSelf = activeSelf;
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            if (opt.cmd != Command.BuildComponent)
                return;
            switch (opt.index1)
            {
                case 1:
                    if (opt.uid == ClientManager.UID)
                        return;
                    layer1 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.layer = layer1;
                    break;
                case 5:
                    if (opt.uid == ClientManager.UID)
                        return;
                    isStatic2 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.isStatic = isStatic2;
                    break;
                case 6:
                    if (opt.uid == ClientManager.UID)
                        return;
                    tag3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.tag = tag3;
                    break;
                case 23:
                    if (opt.uid == ClientManager.UID)
                        return;
                    name4 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.name = name4;
                    break;
                case 24:
                    if (opt.uid == ClientManager.UID)
                        return;
                    hideFlags5 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.HideFlags>(new Net.System.Segment(opt.buffer, false));
                    self.hideFlags = hideFlags5;
                    break;
                case 25:
                    if (opt.uid == ClientManager.UID)
                        return;
                    activeSelf11 = Net.Serialize.NetConvertFast2.DeserializeObject<bool>(new Net.System.Segment(opt.buffer, false));
                    self.SetActive(activeSelf11);
                    break;
                case 27:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var type = data.pars[0] as System.String;
                        self.GetComponent(type);
                    }
                    break;
                case 53:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessageUpwards(methodName, options);
                    }
                    break;
                case 54:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessage(methodName, options);
                    }
                    break;
                case 55:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.BroadcastMessage(methodName, options);
                    }
                    break;
                case 74:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessageUpwards(methodName);
                    }
                    break;
                case 77:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessage(methodName);
                    }
                    break;
                case 80:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.BroadcastMessage(methodName);
                    }
                    break;
                case 101:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetInstanceID();
                    }
                    break;
                case 102:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetHashCode();
                    }
                    break;
                case 108:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.ToString();
                    }
                    break;

            }
        }
    }
}
#endif