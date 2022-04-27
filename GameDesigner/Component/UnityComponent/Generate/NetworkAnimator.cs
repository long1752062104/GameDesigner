#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// Animator同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Animator))]
    public class NetworkAnimator : NetworkBehaviour
    {
        private UnityEngine.Animator self;
        public bool autoCheck;
        private UnityEngine.Vector3 rootPosition1;
        private UnityEngine.Quaternion rootRotation2;
        private System.Boolean applyRootMotion3;
        private UnityEngine.AnimatorUpdateMode updateMode4;
        private UnityEngine.Vector3 bodyPosition5;
        private UnityEngine.Quaternion bodyRotation6;
        private System.Boolean stabilizeFeet7;
        private System.Single feetPivotActive8;
        private System.Single speed9;
        private UnityEngine.AnimatorCullingMode cullingMode10;
        private System.Single playbackTime11;
        private System.Single recorderStartTime12;
        private System.Single recorderStopTime13;
        private UnityEngine.RuntimeAnimatorController runtimeAnimatorController14;
        private UnityEngine.Avatar avatar15;
        private System.Boolean layersAffectMassCenter16;
        private System.Boolean logWarnings17;
        private System.Boolean fireEvents18;
        private System.Boolean keepAnimatorControllerStateOnDisable19;
        private System.Boolean enabled20;
        private System.String name1;
        private System.Int32 id2;
        private System.String name3;
        private System.Single value4;
        private System.String name5;
        private System.Single value6;
        private System.Single dampTime7;
        private System.Single deltaTime8;
        private System.Int32 id9;
        private System.Single value10;
        private System.Int32 id11;
        private System.Single value12;
        private System.Single dampTime13;
        private System.Single deltaTime14;
        private System.String name15;
        private System.Int32 id16;
        private System.String name17;
        private System.Boolean value18;
        private System.Int32 id19;
        private System.Boolean value20;
        private System.String name21;
        private System.Int32 id22;
        private System.String name23;
        private System.Int32 value24;
        private System.Int32 id25;
        private System.Int32 value26;
        private System.String name27;
        private System.Int32 id28;
        private System.String name29;
        private System.Int32 id30;
        private System.String name31;
        private System.Int32 id32;
        private UnityEngine.AvatarIKGoal goal33;
        private UnityEngine.AvatarIKGoal goal34;
        private UnityEngine.Vector3 goalPosition35;
        private UnityEngine.AvatarIKGoal goal36;
        private UnityEngine.AvatarIKGoal goal37;
        private UnityEngine.Quaternion goalRotation38;
        private UnityEngine.AvatarIKGoal goal39;
        private UnityEngine.AvatarIKGoal goal40;
        private System.Single value41;
        private UnityEngine.AvatarIKGoal goal42;
        private UnityEngine.AvatarIKGoal goal43;
        private System.Single value44;
        private UnityEngine.AvatarIKHint hint45;
        private UnityEngine.AvatarIKHint hint46;
        private UnityEngine.Vector3 hintPosition47;
        private UnityEngine.AvatarIKHint hint48;
        private UnityEngine.AvatarIKHint hint49;
        private System.Single value50;
        private UnityEngine.Vector3 lookAtPosition51;
        private System.Single weight52;
        private System.Single weight53;
        private System.Single bodyWeight54;
        private System.Single weight55;
        private System.Single bodyWeight56;
        private System.Single headWeight57;
        private System.Single weight58;
        private System.Single bodyWeight59;
        private System.Single headWeight60;
        private System.Single eyesWeight61;
        private System.Single weight62;
        private System.Single bodyWeight63;
        private System.Single headWeight64;
        private System.Single eyesWeight65;
        private System.Single clampWeight66;
        private UnityEngine.HumanBodyBones humanBoneId67;
        private UnityEngine.Quaternion rotation68;
        private System.Int32 fullPathHash69;
        private System.Int32 layerIndex70;
        private System.Int32 layerIndex71;
        private System.Int32 layerIndex72;
        private System.Int32 layerIndex73;
        private System.Int32 layerIndex74;
        private System.Int32 layerIndex75;
        private System.Int32 index76;
        private System.String stateName77;
        private System.Single fixedTransitionDuration78;
        private System.String stateName79;
        private System.Single fixedTransitionDuration80;
        private System.Int32 layer81;
        private System.String stateName82;
        private System.Single fixedTransitionDuration83;
        private System.Int32 layer84;
        private System.Single fixedTimeOffset85;
        private System.String stateName86;
        private System.Single fixedTransitionDuration87;
        private System.Int32 layer88;
        private System.Single fixedTimeOffset89;
        private System.Single normalizedTransitionTime90;
        private System.Int32 stateHashName91;
        private System.Single fixedTransitionDuration92;
        private System.Int32 layer93;
        private System.Single fixedTimeOffset94;
        private System.Int32 stateHashName95;
        private System.Single fixedTransitionDuration96;
        private System.Int32 layer97;
        private System.Int32 stateHashName98;
        private System.Single fixedTransitionDuration99;
        private System.String stateName100;
        private System.Single normalizedTransitionDuration101;
        private System.Int32 layer102;
        private System.Single normalizedTimeOffset103;
        private System.String stateName104;
        private System.Single normalizedTransitionDuration105;
        private System.Int32 layer106;
        private System.String stateName107;
        private System.Single normalizedTransitionDuration108;
        private System.String stateName109;
        private System.Single normalizedTransitionDuration110;
        private System.Int32 layer111;
        private System.Single normalizedTimeOffset112;
        private System.Single normalizedTransitionTime113;
        private System.Int32 stateHashName114;
        private System.Single normalizedTransitionDuration115;
        private System.Int32 layer116;
        private System.Single normalizedTimeOffset117;
        private System.Int32 stateHashName118;
        private System.Single normalizedTransitionDuration119;
        private System.Int32 layer120;
        private System.Int32 stateHashName121;
        private System.Single normalizedTransitionDuration122;
        private System.String stateName123;
        private System.Int32 layer124;
        private System.String stateName125;
        private System.String stateName126;
        private System.Int32 layer127;
        private System.Single fixedTime128;
        private System.Int32 stateNameHash129;
        private System.Int32 layer130;
        private System.Int32 stateNameHash131;
        private System.String stateName132;
        private System.Int32 layer133;
        private System.String stateName134;
        private System.String stateName135;
        private System.Int32 layer136;
        private System.Single normalizedTime137;
        private System.Int32 stateNameHash138;
        private System.Int32 layer139;
        private System.Int32 stateNameHash140;
        private UnityEngine.HumanBodyBones humanBoneId141;
        private System.String tag142;
        private System.String methodName143;
        private System.String methodName144;
        private UnityEngine.SendMessageOptions options145;
        private System.String methodName146;
        private System.String methodName147;
        private UnityEngine.SendMessageOptions options148;
        private System.String methodName149;
        private System.String methodName150;
        private UnityEngine.SendMessageOptions options151;

        public override void Awake()
        {
            base.Awake();
            self = GetComponent<UnityEngine.Animator>();
            rootPosition1 = self.rootPosition;
            rootRotation2 = self.rootRotation;
            applyRootMotion3 = self.applyRootMotion;
            updateMode4 = self.updateMode;
            bodyPosition5 = self.bodyPosition;
            bodyRotation6 = self.bodyRotation;
            stabilizeFeet7 = self.stabilizeFeet;
            feetPivotActive8 = self.feetPivotActive;
            speed9 = self.speed;
            cullingMode10 = self.cullingMode;
            playbackTime11 = self.playbackTime;
            recorderStartTime12 = self.recorderStartTime;
            recorderStopTime13 = self.recorderStopTime;
            runtimeAnimatorController14 = self.runtimeAnimatorController;
            avatar15 = self.avatar;
            layersAffectMassCenter16 = self.layersAffectMassCenter;
            logWarnings17 = self.logWarnings;
            fireEvents18 = self.fireEvents;
            keepAnimatorControllerStateOnDisable19 = self.keepAnimatorControllerStateOnDisable;
            enabled20 = self.enabled;
        }

        void Start() { }//让监视面板能显示启动勾选
        public UnityEngine.Vector3 rootPosition
        {
            get
            {
                return self.rootPosition;
            }
            set
            {
                if (rootPosition1 == value)
                    return;
                rootPosition1 = value;
                self.rootPosition = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 9,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Quaternion rootRotation
        {
            get
            {
                return self.rootRotation;
            }
            set
            {
                if (rootRotation2 == value)
                    return;
                rootRotation2 = value;
                self.rootRotation = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 10,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean applyRootMotion
        {
            get
            {
                return self.applyRootMotion;
            }
            set
            {
                if (applyRootMotion3 == value)
                    return;
                applyRootMotion3 = value;
                self.applyRootMotion = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 11,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.AnimatorUpdateMode updateMode
        {
            get
            {
                return self.updateMode;
            }
            set
            {
                if (updateMode4 == value)
                    return;
                updateMode4 = value;
                self.updateMode = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 14,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Vector3 bodyPosition
        {
            get
            {
                return self.bodyPosition;
            }
            set
            {
                if (bodyPosition5 == value)
                    return;
                bodyPosition5 = value;
                self.bodyPosition = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 17,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Quaternion bodyRotation
        {
            get
            {
                return self.bodyRotation;
            }
            set
            {
                if (bodyRotation6 == value)
                    return;
                bodyRotation6 = value;
                self.bodyRotation = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 18,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean stabilizeFeet
        {
            get
            {
                return self.stabilizeFeet;
            }
            set
            {
                if (stabilizeFeet7 == value)
                    return;
                stabilizeFeet7 = value;
                self.stabilizeFeet = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 19,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single feetPivotActive
        {
            get
            {
                return self.feetPivotActive;
            }
            set
            {
                if (feetPivotActive8 == value)
                    return;
                feetPivotActive8 = value;
                self.feetPivotActive = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 23,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single speed
        {
            get
            {
                return self.speed;
            }
            set
            {
                if (speed9 == value)
                    return;
                speed9 = value;
                self.speed = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 27,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.AnimatorCullingMode cullingMode
        {
            get
            {
                return self.cullingMode;
            }
            set
            {
                if (cullingMode10 == value)
                    return;
                cullingMode10 = value;
                self.cullingMode = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 30,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single playbackTime
        {
            get
            {
                return self.playbackTime;
            }
            set
            {
                if (playbackTime11 == value)
                    return;
                playbackTime11 = value;
                self.playbackTime = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 31,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single recorderStartTime
        {
            get
            {
                return self.recorderStartTime;
            }
            set
            {
                if (recorderStartTime12 == value)
                    return;
                recorderStartTime12 = value;
                self.recorderStartTime = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 32,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single recorderStopTime
        {
            get
            {
                return self.recorderStopTime;
            }
            set
            {
                if (recorderStopTime13 == value)
                    return;
                recorderStopTime13 = value;
                self.recorderStopTime = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 33,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.RuntimeAnimatorController runtimeAnimatorController
        {
            get
            {
                return self.runtimeAnimatorController;
            }
            set
            {
                if (runtimeAnimatorController14 == value)
                    return;
                runtimeAnimatorController14 = value;
                self.runtimeAnimatorController = value;
                if (!NetworkResources.I.TryGetValue(runtimeAnimatorController14, out ObjectRecord objectRecord))
                    return;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 35,
                    index2 = objectRecord.ID,
                    name = objectRecord.path,
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Avatar avatar
        {
            get
            {
                return self.avatar;
            }
            set
            {
                if (avatar15 == value)
                    return;
                avatar15 = value;
                self.avatar = value;
                if (!NetworkResources.I.TryGetValue(avatar15, out ObjectRecord objectRecord))
                    return;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 37,
                    index2 = objectRecord.ID,
                    name = objectRecord.path,
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean layersAffectMassCenter
        {
            get
            {
                return self.layersAffectMassCenter;
            }
            set
            {
                if (layersAffectMassCenter16 == value)
                    return;
                layersAffectMassCenter16 = value;
                self.layersAffectMassCenter = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 39,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean logWarnings
        {
            get
            {
                return self.logWarnings;
            }
            set
            {
                if (logWarnings17 == value)
                    return;
                logWarnings17 = value;
                self.logWarnings = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 42,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean fireEvents
        {
            get
            {
                return self.fireEvents;
            }
            set
            {
                if (fireEvents18 == value)
                    return;
                fireEvents18 = value;
                self.fireEvents = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 43,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean keepAnimatorControllerStateOnDisable
        {
            get
            {
                return self.keepAnimatorControllerStateOnDisable;
            }
            set
            {
                if (keepAnimatorControllerStateOnDisable19 == value)
                    return;
                keepAnimatorControllerStateOnDisable19 = value;
                self.keepAnimatorControllerStateOnDisable = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 44,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean enabled
        {
            get
            {
                return self.enabled;
            }
            set
            {
                if (enabled20 == value)
                    return;
                enabled20 = value;
                self.enabled = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 45,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public void GetFloat(System.String name, bool always = false)
        {
            if (name == name1 & !always) return;
            name1 = name;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 73,
                buffer = buffer
            });
        }
        public void GetFloat(System.Int32 id, bool always = false)
        {
            if (id == id2 & !always) return;
            id2 = id;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 74,
                buffer = buffer
            });
        }
        public void SetFloat(System.String name, System.Single value, bool always = false)
        {
            if (name == name3 & value == value4 & !always) return;
            name3 = name;
            value4 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 75,
                buffer = buffer
            });
        }
        public void SetFloat(System.String name, System.Single value, System.Single dampTime, System.Single deltaTime, bool always = false)
        {
            if (name == name5 & value == value6 & dampTime == dampTime7 & deltaTime == deltaTime8 & !always) return;
            name5 = name;
            value6 = value;
            dampTime7 = dampTime;
            deltaTime8 = deltaTime;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name, value, dampTime, deltaTime } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 76,
                buffer = buffer
            });
        }
        public void SetFloat(System.Int32 id, System.Single value, bool always = false)
        {
            if (id == id9 & value == value10 & !always) return;
            id9 = id;
            value10 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 77,
                buffer = buffer
            });
        }
        public void SetFloat(System.Int32 id, System.Single value, System.Single dampTime, System.Single deltaTime, bool always = false)
        {
            if (id == id11 & value == value12 & dampTime == dampTime13 & deltaTime == deltaTime14 & !always) return;
            id11 = id;
            value12 = value;
            dampTime13 = dampTime;
            deltaTime14 = deltaTime;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id, value, dampTime, deltaTime } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 78,
                buffer = buffer
            });
        }
        public void GetBool(System.String name, bool always = false)
        {
            if (name == name15 & !always) return;
            name15 = name;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 79,
                buffer = buffer
            });
        }
        public void GetBool(System.Int32 id, bool always = false)
        {
            if (id == id16 & !always) return;
            id16 = id;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 80,
                buffer = buffer
            });
        }
        public void SetBool(System.String name, System.Boolean value, bool always = false)
        {
            if (name == name17 & value == value18 & !always) return;
            name17 = name;
            value18 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 81,
                buffer = buffer
            });
        }
        public void SetBool(System.Int32 id, System.Boolean value, bool always = false)
        {
            if (id == id19 & value == value20 & !always) return;
            id19 = id;
            value20 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 82,
                buffer = buffer
            });
        }
        public void GetInteger(System.String name, bool always = false)
        {
            if (name == name21 & !always) return;
            name21 = name;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 83,
                buffer = buffer
            });
        }
        public void GetInteger(System.Int32 id, bool always = false)
        {
            if (id == id22 & !always) return;
            id22 = id;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 84,
                buffer = buffer
            });
        }
        public void SetInteger(System.String name, System.Int32 value, bool always = false)
        {
            if (name == name23 & value == value24 & !always) return;
            name23 = name;
            value24 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 85,
                buffer = buffer
            });
        }
        public void SetInteger(System.Int32 id, System.Int32 value, bool always = false)
        {
            if (id == id25 & value == value26 & !always) return;
            id25 = id;
            value26 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 86,
                buffer = buffer
            });
        }
        public void SetTrigger(System.String name, bool always = false)
        {
            if (name == name27 & !always) return;
            name27 = name;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 87,
                buffer = buffer
            });
        }
        public void SetTrigger(System.Int32 id, bool always = false)
        {
            if (id == id28 & !always) return;
            id28 = id;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 88,
                buffer = buffer
            });
        }
        public void ResetTrigger(System.String name, bool always = false)
        {
            if (name == name29 & !always) return;
            name29 = name;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 89,
                buffer = buffer
            });
        }
        public void ResetTrigger(System.Int32 id, bool always = false)
        {
            if (id == id30 & !always) return;
            id30 = id;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 90,
                buffer = buffer
            });
        }
        public void IsParameterControlledByCurve(System.String name, bool always = false)
        {
            if (name == name31 & !always) return;
            name31 = name;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 91,
                buffer = buffer
            });
        }
        public void IsParameterControlledByCurve(System.Int32 id, bool always = false)
        {
            if (id == id32 & !always) return;
            id32 = id;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { id } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 92,
                buffer = buffer
            });
        }
        public void GetIKPosition(UnityEngine.AvatarIKGoal goal, bool always = false)
        {
            if (goal == goal33 & !always) return;
            goal33 = goal;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 115,
                buffer = buffer
            });
        }
        public void SetIKPosition(UnityEngine.AvatarIKGoal goal, UnityEngine.Vector3 goalPosition, bool always = false)
        {
            if (goal == goal34 & goalPosition == goalPosition35 & !always) return;
            goal34 = goal;
            goalPosition35 = goalPosition;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal, goalPosition } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 116,
                buffer = buffer
            });
        }
        public void GetIKRotation(UnityEngine.AvatarIKGoal goal, bool always = false)
        {
            if (goal == goal36 & !always) return;
            goal36 = goal;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 117,
                buffer = buffer
            });
        }
        public void SetIKRotation(UnityEngine.AvatarIKGoal goal, UnityEngine.Quaternion goalRotation, bool always = false)
        {
            if (goal == goal37 & goalRotation == goalRotation38 & !always) return;
            goal37 = goal;
            goalRotation38 = goalRotation;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal, goalRotation } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 118,
                buffer = buffer
            });
        }
        public void GetIKPositionWeight(UnityEngine.AvatarIKGoal goal, bool always = false)
        {
            if (goal == goal39 & !always) return;
            goal39 = goal;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 119,
                buffer = buffer
            });
        }
        public void SetIKPositionWeight(UnityEngine.AvatarIKGoal goal, System.Single value, bool always = false)
        {
            if (goal == goal40 & value == value41 & !always) return;
            goal40 = goal;
            value41 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 120,
                buffer = buffer
            });
        }
        public void GetIKRotationWeight(UnityEngine.AvatarIKGoal goal, bool always = false)
        {
            if (goal == goal42 & !always) return;
            goal42 = goal;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 121,
                buffer = buffer
            });
        }
        public void SetIKRotationWeight(UnityEngine.AvatarIKGoal goal, System.Single value, bool always = false)
        {
            if (goal == goal43 & value == value44 & !always) return;
            goal43 = goal;
            value44 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { goal, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 122,
                buffer = buffer
            });
        }
        public void GetIKHintPosition(UnityEngine.AvatarIKHint hint, bool always = false)
        {
            if (hint == hint45 & !always) return;
            hint45 = hint;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { hint } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 123,
                buffer = buffer
            });
        }
        public void SetIKHintPosition(UnityEngine.AvatarIKHint hint, UnityEngine.Vector3 hintPosition, bool always = false)
        {
            if (hint == hint46 & hintPosition == hintPosition47 & !always) return;
            hint46 = hint;
            hintPosition47 = hintPosition;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { hint, hintPosition } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 124,
                buffer = buffer
            });
        }
        public void GetIKHintPositionWeight(UnityEngine.AvatarIKHint hint, bool always = false)
        {
            if (hint == hint48 & !always) return;
            hint48 = hint;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { hint } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 125,
                buffer = buffer
            });
        }
        public void SetIKHintPositionWeight(UnityEngine.AvatarIKHint hint, System.Single value, bool always = false)
        {
            if (hint == hint49 & value == value50 & !always) return;
            hint49 = hint;
            value50 = value;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { hint, value } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 126,
                buffer = buffer
            });
        }
        public void SetLookAtPosition(UnityEngine.Vector3 lookAtPosition, bool always = false)
        {
            if (lookAtPosition == lookAtPosition51 & !always) return;
            lookAtPosition51 = lookAtPosition;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { lookAtPosition } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 127,
                buffer = buffer
            });
        }
        public void SetLookAtWeight(System.Single weight, bool always = false)
        {
            if (weight == weight52 & !always) return;
            weight52 = weight;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { weight } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 128,
                buffer = buffer
            });
        }
        public void SetLookAtWeight(System.Single weight, System.Single bodyWeight, bool always = false)
        {
            if (weight == weight53 & bodyWeight == bodyWeight54 & !always) return;
            weight53 = weight;
            bodyWeight54 = bodyWeight;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { weight, bodyWeight } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 129,
                buffer = buffer
            });
        }
        public void SetLookAtWeight(System.Single weight, System.Single bodyWeight, System.Single headWeight, bool always = false)
        {
            if (weight == weight55 & bodyWeight == bodyWeight56 & headWeight == headWeight57 & !always) return;
            weight55 = weight;
            bodyWeight56 = bodyWeight;
            headWeight57 = headWeight;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { weight, bodyWeight, headWeight } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 130,
                buffer = buffer
            });
        }
        public void SetLookAtWeight(System.Single weight, System.Single bodyWeight, System.Single headWeight, System.Single eyesWeight, bool always = false)
        {
            if (weight == weight58 & bodyWeight == bodyWeight59 & headWeight == headWeight60 & eyesWeight == eyesWeight61 & !always) return;
            weight58 = weight;
            bodyWeight59 = bodyWeight;
            headWeight60 = headWeight;
            eyesWeight61 = eyesWeight;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { weight, bodyWeight, headWeight, eyesWeight } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 131,
                buffer = buffer
            });
        }
        public void SetLookAtWeight(System.Single weight, System.Single bodyWeight, System.Single headWeight, System.Single eyesWeight, System.Single clampWeight, bool always = false)
        {
            if (weight == weight62 & bodyWeight == bodyWeight63 & headWeight == headWeight64 & eyesWeight == eyesWeight65 & clampWeight == clampWeight66 & !always) return;
            weight62 = weight;
            bodyWeight63 = bodyWeight;
            headWeight64 = headWeight;
            eyesWeight65 = eyesWeight;
            clampWeight66 = clampWeight;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { weight, bodyWeight, headWeight, eyesWeight, clampWeight } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 132,
                buffer = buffer
            });
        }
        public void SetBoneLocalRotation(UnityEngine.HumanBodyBones humanBoneId, UnityEngine.Quaternion rotation, bool always = false)
        {
            if (humanBoneId == humanBoneId67 & rotation == rotation68 & !always) return;
            humanBoneId67 = humanBoneId;
            rotation68 = rotation;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { humanBoneId, rotation } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 133,
                buffer = buffer
            });
        }
        public void GetBehaviours(System.Int32 fullPathHash, System.Int32 layerIndex, bool always = false)
        {
            if (fullPathHash == fullPathHash69 & layerIndex == layerIndex70 & !always) return;
            fullPathHash69 = fullPathHash;
            layerIndex70 = layerIndex;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { fullPathHash, layerIndex } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 136,
                buffer = buffer
            });
        }
        public void GetCurrentAnimatorStateInfo(System.Int32 layerIndex, bool always = false)
        {
            if (layerIndex == layerIndex71 & !always) return;
            layerIndex71 = layerIndex;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { layerIndex } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 144,
                buffer = buffer
            });
        }
        public void GetNextAnimatorStateInfo(System.Int32 layerIndex, bool always = false)
        {
            if (layerIndex == layerIndex72 & !always) return;
            layerIndex72 = layerIndex;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { layerIndex } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 145,
                buffer = buffer
            });
        }
        public void GetAnimatorTransitionInfo(System.Int32 layerIndex, bool always = false)
        {
            if (layerIndex == layerIndex73 & !always) return;
            layerIndex73 = layerIndex;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { layerIndex } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 146,
                buffer = buffer
            });
        }
        public void GetCurrentAnimatorClipInfoCount(System.Int32 layerIndex, bool always = false)
        {
            if (layerIndex == layerIndex74 & !always) return;
            layerIndex74 = layerIndex;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { layerIndex } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 147,
                buffer = buffer
            });
        }
        public void GetNextAnimatorClipInfoCount(System.Int32 layerIndex, bool always = false)
        {
            if (layerIndex == layerIndex75 & !always) return;
            layerIndex75 = layerIndex;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { layerIndex } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 148,
                buffer = buffer
            });
        }
        public void GetParameter(System.Int32 index, bool always = false)
        {
            if (index == index76 & !always) return;
            index76 = index;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { index } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 156,
                buffer = buffer
            });
        }
        public void InterruptMatchTarget(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 164,
                buffer = buffer
            });
        }
        public void CrossFadeInFixedTime(System.String stateName, System.Single fixedTransitionDuration, bool always = false)
        {
            if (stateName == stateName77 & fixedTransitionDuration == fixedTransitionDuration78 & !always) return;
            stateName77 = stateName;
            fixedTransitionDuration78 = fixedTransitionDuration;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, fixedTransitionDuration } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 170,
                buffer = buffer
            });
        }
        public void CrossFadeInFixedTime(System.String stateName, System.Single fixedTransitionDuration, System.Int32 layer, bool always = false)
        {
            if (stateName == stateName79 & fixedTransitionDuration == fixedTransitionDuration80 & layer == layer81 & !always) return;
            stateName79 = stateName;
            fixedTransitionDuration80 = fixedTransitionDuration;
            layer81 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, fixedTransitionDuration, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 171,
                buffer = buffer
            });
        }
        public void CrossFadeInFixedTime(System.String stateName, System.Single fixedTransitionDuration, System.Int32 layer, System.Single fixedTimeOffset, bool always = false)
        {
            if (stateName == stateName82 & fixedTransitionDuration == fixedTransitionDuration83 & layer == layer84 & fixedTimeOffset == fixedTimeOffset85 & !always) return;
            stateName82 = stateName;
            fixedTransitionDuration83 = fixedTransitionDuration;
            layer84 = layer;
            fixedTimeOffset85 = fixedTimeOffset;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, fixedTransitionDuration, layer, fixedTimeOffset } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 172,
                buffer = buffer
            });
        }
        public void CrossFadeInFixedTime(System.String stateName, System.Single fixedTransitionDuration, System.Int32 layer, System.Single fixedTimeOffset, System.Single normalizedTransitionTime, bool always = false)
        {
            if (stateName == stateName86 & fixedTransitionDuration == fixedTransitionDuration87 & layer == layer88 & fixedTimeOffset == fixedTimeOffset89 & normalizedTransitionTime == normalizedTransitionTime90 & !always) return;
            stateName86 = stateName;
            fixedTransitionDuration87 = fixedTransitionDuration;
            layer88 = layer;
            fixedTimeOffset89 = fixedTimeOffset;
            normalizedTransitionTime90 = normalizedTransitionTime;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 173,
                buffer = buffer
            });
        }
        public void CrossFadeInFixedTime(System.Int32 stateHashName, System.Single fixedTransitionDuration, System.Int32 layer, System.Single fixedTimeOffset, bool always = false)
        {
            if (stateHashName == stateHashName91 & fixedTransitionDuration == fixedTransitionDuration92 & layer == layer93 & fixedTimeOffset == fixedTimeOffset94 & !always) return;
            stateHashName91 = stateHashName;
            fixedTransitionDuration92 = fixedTransitionDuration;
            layer93 = layer;
            fixedTimeOffset94 = fixedTimeOffset;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateHashName, fixedTransitionDuration, layer, fixedTimeOffset } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 174,
                buffer = buffer
            });
        }
        public void CrossFadeInFixedTime(System.Int32 stateHashName, System.Single fixedTransitionDuration, System.Int32 layer, bool always = false)
        {
            if (stateHashName == stateHashName95 & fixedTransitionDuration == fixedTransitionDuration96 & layer == layer97 & !always) return;
            stateHashName95 = stateHashName;
            fixedTransitionDuration96 = fixedTransitionDuration;
            layer97 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateHashName, fixedTransitionDuration, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 175,
                buffer = buffer
            });
        }
        public void CrossFadeInFixedTime(System.Int32 stateHashName, System.Single fixedTransitionDuration, bool always = false)
        {
            if (stateHashName == stateHashName98 & fixedTransitionDuration == fixedTransitionDuration99 & !always) return;
            stateHashName98 = stateHashName;
            fixedTransitionDuration99 = fixedTransitionDuration;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateHashName, fixedTransitionDuration } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 176,
                buffer = buffer
            });
        }
        public void CrossFade(System.String stateName, System.Single normalizedTransitionDuration, System.Int32 layer, System.Single normalizedTimeOffset, bool always = false)
        {
            if (stateName == stateName100 & normalizedTransitionDuration == normalizedTransitionDuration101 & layer == layer102 & normalizedTimeOffset == normalizedTimeOffset103 & !always) return;
            stateName100 = stateName;
            normalizedTransitionDuration101 = normalizedTransitionDuration;
            layer102 = layer;
            normalizedTimeOffset103 = normalizedTimeOffset;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, normalizedTransitionDuration, layer, normalizedTimeOffset } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 179,
                buffer = buffer
            });
        }
        public void CrossFade(System.String stateName, System.Single normalizedTransitionDuration, System.Int32 layer, bool always = false)
        {
            if (stateName == stateName104 & normalizedTransitionDuration == normalizedTransitionDuration105 & layer == layer106 & !always) return;
            stateName104 = stateName;
            normalizedTransitionDuration105 = normalizedTransitionDuration;
            layer106 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, normalizedTransitionDuration, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 180,
                buffer = buffer
            });
        }
        public void CrossFade(System.String stateName, System.Single normalizedTransitionDuration, bool always = false)
        {
            if (stateName == stateName107 & normalizedTransitionDuration == normalizedTransitionDuration108 & !always) return;
            stateName107 = stateName;
            normalizedTransitionDuration108 = normalizedTransitionDuration;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, normalizedTransitionDuration } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 181,
                buffer = buffer
            });
        }
        public void CrossFade(System.String stateName, System.Single normalizedTransitionDuration, System.Int32 layer, System.Single normalizedTimeOffset, System.Single normalizedTransitionTime, bool always = false)
        {
            if (stateName == stateName109 & normalizedTransitionDuration == normalizedTransitionDuration110 & layer == layer111 & normalizedTimeOffset == normalizedTimeOffset112 & normalizedTransitionTime == normalizedTransitionTime113 & !always) return;
            stateName109 = stateName;
            normalizedTransitionDuration110 = normalizedTransitionDuration;
            layer111 = layer;
            normalizedTimeOffset112 = normalizedTimeOffset;
            normalizedTransitionTime113 = normalizedTransitionTime;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 182,
                buffer = buffer
            });
        }
        public void CrossFade(System.Int32 stateHashName, System.Single normalizedTransitionDuration, System.Int32 layer, System.Single normalizedTimeOffset, bool always = false)
        {
            if (stateHashName == stateHashName114 & normalizedTransitionDuration == normalizedTransitionDuration115 & layer == layer116 & normalizedTimeOffset == normalizedTimeOffset117 & !always) return;
            stateHashName114 = stateHashName;
            normalizedTransitionDuration115 = normalizedTransitionDuration;
            layer116 = layer;
            normalizedTimeOffset117 = normalizedTimeOffset;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 184,
                buffer = buffer
            });
        }
        public void CrossFade(System.Int32 stateHashName, System.Single normalizedTransitionDuration, System.Int32 layer, bool always = false)
        {
            if (stateHashName == stateHashName118 & normalizedTransitionDuration == normalizedTransitionDuration119 & layer == layer120 & !always) return;
            stateHashName118 = stateHashName;
            normalizedTransitionDuration119 = normalizedTransitionDuration;
            layer120 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateHashName, normalizedTransitionDuration, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 185,
                buffer = buffer
            });
        }
        public void CrossFade(System.Int32 stateHashName, System.Single normalizedTransitionDuration, bool always = false)
        {
            if (stateHashName == stateHashName121 & normalizedTransitionDuration == normalizedTransitionDuration122 & !always) return;
            stateHashName121 = stateHashName;
            normalizedTransitionDuration122 = normalizedTransitionDuration;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateHashName, normalizedTransitionDuration } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 186,
                buffer = buffer
            });
        }
        public void PlayInFixedTime(System.String stateName, System.Int32 layer, bool always = false)
        {
            if (stateName == stateName123 & layer == layer124 & !always) return;
            stateName123 = stateName;
            layer124 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 187,
                buffer = buffer
            });
        }
        public void PlayInFixedTime(System.String stateName, bool always = false)
        {
            if (stateName == stateName125 & !always) return;
            stateName125 = stateName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 188,
                buffer = buffer
            });
        }
        public void PlayInFixedTime(System.String stateName, System.Int32 layer, System.Single fixedTime, bool always = false)
        {
            if (stateName == stateName126 & layer == layer127 & fixedTime == fixedTime128 & !always) return;
            stateName126 = stateName;
            layer127 = layer;
            fixedTime128 = fixedTime;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, layer, fixedTime } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 189,
                buffer = buffer
            });
        }
        public void PlayInFixedTime(System.Int32 stateNameHash, System.Int32 layer, bool always = false)
        {
            if (stateNameHash == stateNameHash129 & layer == layer130 & !always) return;
            stateNameHash129 = stateNameHash;
            layer130 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateNameHash, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 191,
                buffer = buffer
            });
        }
        public void PlayInFixedTime(System.Int32 stateNameHash, bool always = false)
        {
            if (stateNameHash == stateNameHash131 & !always) return;
            stateNameHash131 = stateNameHash;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateNameHash } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 192,
                buffer = buffer
            });
        }
        public void Play(System.String stateName, System.Int32 layer, bool always = false)
        {
            if (stateName == stateName132 & layer == layer133 & !always) return;
            stateName132 = stateName;
            layer133 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 193,
                buffer = buffer
            });
        }
        public void Play(System.String stateName, bool always = false)
        {
            if (stateName == stateName134 & !always) return;
            stateName134 = stateName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 194,
                buffer = buffer
            });
        }
        public void Play(System.String stateName, System.Int32 layer, System.Single normalizedTime, bool always = false)
        {
            if (stateName == stateName135 & layer == layer136 & normalizedTime == normalizedTime137 & !always) return;
            stateName135 = stateName;
            layer136 = layer;
            normalizedTime137 = normalizedTime;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateName, layer, normalizedTime } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 195,
                buffer = buffer
            });
        }
        public void Play(System.Int32 stateNameHash, System.Int32 layer, bool always = false)
        {
            if (stateNameHash == stateNameHash138 & layer == layer139 & !always) return;
            stateNameHash138 = stateNameHash;
            layer139 = layer;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateNameHash, layer } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 197,
                buffer = buffer
            });
        }
        public void Play(System.Int32 stateNameHash, bool always = false)
        {
            if (stateNameHash == stateNameHash140 & !always) return;
            stateNameHash140 = stateNameHash;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { stateNameHash } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 198,
                buffer = buffer
            });
        }
        public void GetBoneTransform(UnityEngine.HumanBodyBones humanBoneId, bool always = false)
        {
            if (humanBoneId == humanBoneId141 & !always) return;
            humanBoneId141 = humanBoneId;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { humanBoneId } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 203,
                buffer = buffer
            });
        }
        public void Rebind(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 229,
                buffer = buffer
            });
        }
        public void CompareTag(System.String tag, bool always = false)
        {
            if (tag == tag142 & !always) return;
            tag142 = tag;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { tag } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 278,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, bool always = false)
        {
            if (methodName == methodName143 & !always) return;
            methodName143 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 281,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName144 & options == options145 & !always) return;
            methodName144 = methodName;
            options145 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 282,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName146 & !always) return;
            methodName146 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 284,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName147 & options == options148 & !always) return;
            methodName147 = methodName;
            options148 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 286,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName149 & !always) return;
            methodName149 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 289,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName150 & options == options151 & !always) return;
            methodName150 = methodName;
            options151 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 290,
                buffer = buffer
            });
        }
        public void GetInstanceID(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 304,
                buffer = buffer
            });
        }
        public void GetHashCode(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 305,
                buffer = buffer
            });
        }
        public void ToString(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 311,
                buffer = buffer
            });
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;
            rootPosition = rootPosition;
            rootRotation = rootRotation;
            applyRootMotion = applyRootMotion;
            updateMode = updateMode;
            bodyPosition = bodyPosition;
            bodyRotation = bodyRotation;
            stabilizeFeet = stabilizeFeet;
            feetPivotActive = feetPivotActive;
            speed = speed;
            cullingMode = cullingMode;
            playbackTime = playbackTime;
            recorderStartTime = recorderStartTime;
            recorderStopTime = recorderStopTime;
            runtimeAnimatorController = runtimeAnimatorController;
            avatar = avatar;
            layersAffectMassCenter = layersAffectMassCenter;
            logWarnings = logWarnings;
            fireEvents = fireEvents;
            keepAnimatorControllerStateOnDisable = keepAnimatorControllerStateOnDisable;
            enabled = enabled;
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            if (opt.cmd != Command.BuildComponent)
                return;
            switch (opt.index1)
            {
                case 9:
                    if (opt.uid == ClientManager.UID)
                        return;
                    rootPosition1 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector3>(new Net.System.Segment(opt.buffer, false));
                    self.rootPosition = rootPosition1;
                    break;
                case 10:
                    if (opt.uid == ClientManager.UID)
                        return;
                    rootRotation2 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Quaternion>(new Net.System.Segment(opt.buffer, false));
                    self.rootRotation = rootRotation2;
                    break;
                case 11:
                    if (opt.uid == ClientManager.UID)
                        return;
                    applyRootMotion3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.applyRootMotion = applyRootMotion3;
                    break;
                case 14:
                    if (opt.uid == ClientManager.UID)
                        return;
                    updateMode4 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.AnimatorUpdateMode>(new Net.System.Segment(opt.buffer, false));
                    self.updateMode = updateMode4;
                    break;
                case 17:
                    if (opt.uid == ClientManager.UID)
                        return;
                    bodyPosition5 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector3>(new Net.System.Segment(opt.buffer, false));
                    self.bodyPosition = bodyPosition5;
                    break;
                case 18:
                    if (opt.uid == ClientManager.UID)
                        return;
                    bodyRotation6 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Quaternion>(new Net.System.Segment(opt.buffer, false));
                    self.bodyRotation = bodyRotation6;
                    break;
                case 19:
                    if (opt.uid == ClientManager.UID)
                        return;
                    stabilizeFeet7 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.stabilizeFeet = stabilizeFeet7;
                    break;
                case 23:
                    if (opt.uid == ClientManager.UID)
                        return;
                    feetPivotActive8 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.feetPivotActive = feetPivotActive8;
                    break;
                case 27:
                    if (opt.uid == ClientManager.UID)
                        return;
                    speed9 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.speed = speed9;
                    break;
                case 30:
                    if (opt.uid == ClientManager.UID)
                        return;
                    cullingMode10 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.AnimatorCullingMode>(new Net.System.Segment(opt.buffer, false));
                    self.cullingMode = cullingMode10;
                    break;
                case 31:
                    if (opt.uid == ClientManager.UID)
                        return;
                    playbackTime11 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.playbackTime = playbackTime11;
                    break;
                case 32:
                    if (opt.uid == ClientManager.UID)
                        return;
                    recorderStartTime12 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.recorderStartTime = recorderStartTime12;
                    break;
                case 33:
                    if (opt.uid == ClientManager.UID)
                        return;
                    recorderStopTime13 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.recorderStopTime = recorderStopTime13;
                    break;
                case 35:
                    if (opt.uid == ClientManager.UID)
                        return;
                    runtimeAnimatorController14 = NetworkResources.I.GetObject<UnityEngine.RuntimeAnimatorController>(opt.index2, opt.name);
                    self.runtimeAnimatorController = runtimeAnimatorController14;
                    break;
                case 37:
                    if (opt.uid == ClientManager.UID)
                        return;
                    avatar15 = NetworkResources.I.GetObject<UnityEngine.Avatar>(opt.index2, opt.name);
                    self.avatar = avatar15;
                    break;
                case 39:
                    if (opt.uid == ClientManager.UID)
                        return;
                    layersAffectMassCenter16 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.layersAffectMassCenter = layersAffectMassCenter16;
                    break;
                case 42:
                    if (opt.uid == ClientManager.UID)
                        return;
                    logWarnings17 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.logWarnings = logWarnings17;
                    break;
                case 43:
                    if (opt.uid == ClientManager.UID)
                        return;
                    fireEvents18 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.fireEvents = fireEvents18;
                    break;
                case 44:
                    if (opt.uid == ClientManager.UID)
                        return;
                    keepAnimatorControllerStateOnDisable19 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.keepAnimatorControllerStateOnDisable = keepAnimatorControllerStateOnDisable19;
                    break;
                case 45:
                    if (opt.uid == ClientManager.UID)
                        return;
                    enabled20 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.enabled = enabled20;
                    break;
                case 73:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        self.GetFloat(name);
                    }
                    break;
                case 74:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        self.GetFloat(id);
                    }
                    break;
                case 75:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        var value = (System.Single)data.pars[1];
                        self.SetFloat(name, value);
                    }
                    break;
                case 76:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        var value = (System.Single)data.pars[1];
                        var dampTime = (System.Single)data.pars[2];
                        var deltaTime = (System.Single)data.pars[3];
                        self.SetFloat(name, value, dampTime, deltaTime);
                    }
                    break;
                case 77:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        var value = (System.Single)data.pars[1];
                        self.SetFloat(id, value);
                    }
                    break;
                case 78:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        var value = (System.Single)data.pars[1];
                        var dampTime = (System.Single)data.pars[2];
                        var deltaTime = (System.Single)data.pars[3];
                        self.SetFloat(id, value, dampTime, deltaTime);
                    }
                    break;
                case 79:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        self.GetBool(name);
                    }
                    break;
                case 80:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        self.GetBool(id);
                    }
                    break;
                case 81:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        var value = (System.Boolean)data.pars[1];
                        self.SetBool(name, value);
                    }
                    break;
                case 82:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        var value = (System.Boolean)data.pars[1];
                        self.SetBool(id, value);
                    }
                    break;
                case 83:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        self.GetInteger(name);
                    }
                    break;
                case 84:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        self.GetInteger(id);
                    }
                    break;
                case 85:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        var value = (System.Int32)data.pars[1];
                        self.SetInteger(name, value);
                    }
                    break;
                case 86:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        var value = (System.Int32)data.pars[1];
                        self.SetInteger(id, value);
                    }
                    break;
                case 87:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        self.SetTrigger(name);
                    }
                    break;
                case 88:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        self.SetTrigger(id);
                    }
                    break;
                case 89:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        self.ResetTrigger(name);
                    }
                    break;
                case 90:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        self.ResetTrigger(id);
                    }
                    break;
                case 91:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var name = data.pars[0] as System.String;
                        self.IsParameterControlledByCurve(name);
                    }
                    break;
                case 92:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var id = (System.Int32)data.pars[0];
                        self.IsParameterControlledByCurve(id);
                    }
                    break;
                case 115:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        self.GetIKPosition(goal);
                    }
                    break;
                case 116:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        var goalPosition = (UnityEngine.Vector3)data.pars[1];
                        self.SetIKPosition(goal, goalPosition);
                    }
                    break;
                case 117:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        self.GetIKRotation(goal);
                    }
                    break;
                case 118:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        var goalRotation = (UnityEngine.Quaternion)data.pars[1];
                        self.SetIKRotation(goal, goalRotation);
                    }
                    break;
                case 119:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        self.GetIKPositionWeight(goal);
                    }
                    break;
                case 120:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        var value = (System.Single)data.pars[1];
                        self.SetIKPositionWeight(goal, value);
                    }
                    break;
                case 121:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        self.GetIKRotationWeight(goal);
                    }
                    break;
                case 122:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var goal = (UnityEngine.AvatarIKGoal)data.pars[0];
                        var value = (System.Single)data.pars[1];
                        self.SetIKRotationWeight(goal, value);
                    }
                    break;
                case 123:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var hint = (UnityEngine.AvatarIKHint)data.pars[0];
                        self.GetIKHintPosition(hint);
                    }
                    break;
                case 124:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var hint = (UnityEngine.AvatarIKHint)data.pars[0];
                        var hintPosition = (UnityEngine.Vector3)data.pars[1];
                        self.SetIKHintPosition(hint, hintPosition);
                    }
                    break;
                case 125:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var hint = (UnityEngine.AvatarIKHint)data.pars[0];
                        self.GetIKHintPositionWeight(hint);
                    }
                    break;
                case 126:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var hint = (UnityEngine.AvatarIKHint)data.pars[0];
                        var value = (System.Single)data.pars[1];
                        self.SetIKHintPositionWeight(hint, value);
                    }
                    break;
                case 127:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var lookAtPosition = (UnityEngine.Vector3)data.pars[0];
                        self.SetLookAtPosition(lookAtPosition);
                    }
                    break;
                case 128:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var weight = (System.Single)data.pars[0];
                        self.SetLookAtWeight(weight);
                    }
                    break;
                case 129:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var weight = (System.Single)data.pars[0];
                        var bodyWeight = (System.Single)data.pars[1];
                        self.SetLookAtWeight(weight, bodyWeight);
                    }
                    break;
                case 130:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var weight = (System.Single)data.pars[0];
                        var bodyWeight = (System.Single)data.pars[1];
                        var headWeight = (System.Single)data.pars[2];
                        self.SetLookAtWeight(weight, bodyWeight, headWeight);
                    }
                    break;
                case 131:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var weight = (System.Single)data.pars[0];
                        var bodyWeight = (System.Single)data.pars[1];
                        var headWeight = (System.Single)data.pars[2];
                        var eyesWeight = (System.Single)data.pars[3];
                        self.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight);
                    }
                    break;
                case 132:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var weight = (System.Single)data.pars[0];
                        var bodyWeight = (System.Single)data.pars[1];
                        var headWeight = (System.Single)data.pars[2];
                        var eyesWeight = (System.Single)data.pars[3];
                        var clampWeight = (System.Single)data.pars[4];
                        self.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
                    }
                    break;
                case 133:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var humanBoneId = (UnityEngine.HumanBodyBones)data.pars[0];
                        var rotation = (UnityEngine.Quaternion)data.pars[1];
                        self.SetBoneLocalRotation(humanBoneId, rotation);
                    }
                    break;
                case 136:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var fullPathHash = (System.Int32)data.pars[0];
                        var layerIndex = (System.Int32)data.pars[1];
                        self.GetBehaviours(fullPathHash, layerIndex);
                    }
                    break;
                case 144:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var layerIndex = (System.Int32)data.pars[0];
                        self.GetCurrentAnimatorStateInfo(layerIndex);
                    }
                    break;
                case 145:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var layerIndex = (System.Int32)data.pars[0];
                        self.GetNextAnimatorStateInfo(layerIndex);
                    }
                    break;
                case 146:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var layerIndex = (System.Int32)data.pars[0];
                        self.GetAnimatorTransitionInfo(layerIndex);
                    }
                    break;
                case 147:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var layerIndex = (System.Int32)data.pars[0];
                        self.GetCurrentAnimatorClipInfoCount(layerIndex);
                    }
                    break;
                case 148:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var layerIndex = (System.Int32)data.pars[0];
                        self.GetNextAnimatorClipInfoCount(layerIndex);
                    }
                    break;
                case 156:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var index = (System.Int32)data.pars[0];
                        self.GetParameter(index);
                    }
                    break;
                case 164:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.InterruptMatchTarget();
                    }
                    break;
                case 170:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var fixedTransitionDuration = (System.Single)data.pars[1];
                        self.CrossFadeInFixedTime(stateName, fixedTransitionDuration);
                    }
                    break;
                case 171:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var fixedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        self.CrossFadeInFixedTime(stateName, fixedTransitionDuration, layer);
                    }
                    break;
                case 172:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var fixedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        var fixedTimeOffset = (System.Single)data.pars[3];
                        self.CrossFadeInFixedTime(stateName, fixedTransitionDuration, layer, fixedTimeOffset);
                    }
                    break;
                case 173:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var fixedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        var fixedTimeOffset = (System.Single)data.pars[3];
                        var normalizedTransitionTime = (System.Single)data.pars[4];
                        self.CrossFadeInFixedTime(stateName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
                    }
                    break;
                case 174:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateHashName = (System.Int32)data.pars[0];
                        var fixedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        var fixedTimeOffset = (System.Single)data.pars[3];
                        self.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset);
                    }
                    break;
                case 175:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateHashName = (System.Int32)data.pars[0];
                        var fixedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        self.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer);
                    }
                    break;
                case 176:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateHashName = (System.Int32)data.pars[0];
                        var fixedTransitionDuration = (System.Single)data.pars[1];
                        self.CrossFadeInFixedTime(stateHashName, fixedTransitionDuration);
                    }
                    break;
                case 179:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var normalizedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        var normalizedTimeOffset = (System.Single)data.pars[3];
                        self.CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset);
                    }
                    break;
                case 180:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var normalizedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        self.CrossFade(stateName, normalizedTransitionDuration, layer);
                    }
                    break;
                case 181:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var normalizedTransitionDuration = (System.Single)data.pars[1];
                        self.CrossFade(stateName, normalizedTransitionDuration);
                    }
                    break;
                case 182:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var normalizedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        var normalizedTimeOffset = (System.Single)data.pars[3];
                        var normalizedTransitionTime = (System.Single)data.pars[4];
                        self.CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
                    }
                    break;
                case 184:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateHashName = (System.Int32)data.pars[0];
                        var normalizedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        var normalizedTimeOffset = (System.Single)data.pars[3];
                        self.CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset);
                    }
                    break;
                case 185:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateHashName = (System.Int32)data.pars[0];
                        var normalizedTransitionDuration = (System.Single)data.pars[1];
                        var layer = (System.Int32)data.pars[2];
                        self.CrossFade(stateHashName, normalizedTransitionDuration, layer);
                    }
                    break;
                case 186:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateHashName = (System.Int32)data.pars[0];
                        var normalizedTransitionDuration = (System.Single)data.pars[1];
                        self.CrossFade(stateHashName, normalizedTransitionDuration);
                    }
                    break;
                case 187:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var layer = (System.Int32)data.pars[1];
                        self.PlayInFixedTime(stateName, layer);
                    }
                    break;
                case 188:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        self.PlayInFixedTime(stateName);
                    }
                    break;
                case 189:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var layer = (System.Int32)data.pars[1];
                        var fixedTime = (System.Single)data.pars[2];
                        self.PlayInFixedTime(stateName, layer, fixedTime);
                    }
                    break;
                case 191:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateNameHash = (System.Int32)data.pars[0];
                        var layer = (System.Int32)data.pars[1];
                        self.PlayInFixedTime(stateNameHash, layer);
                    }
                    break;
                case 192:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateNameHash = (System.Int32)data.pars[0];
                        self.PlayInFixedTime(stateNameHash);
                    }
                    break;
                case 193:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var layer = (System.Int32)data.pars[1];
                        self.Play(stateName, layer);
                    }
                    break;
                case 194:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        self.Play(stateName);
                    }
                    break;
                case 195:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateName = data.pars[0] as System.String;
                        var layer = (System.Int32)data.pars[1];
                        var normalizedTime = (System.Single)data.pars[2];
                        self.Play(stateName, layer, normalizedTime);
                    }
                    break;
                case 197:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateNameHash = (System.Int32)data.pars[0];
                        var layer = (System.Int32)data.pars[1];
                        self.Play(stateNameHash, layer);
                    }
                    break;
                case 198:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var stateNameHash = (System.Int32)data.pars[0];
                        self.Play(stateNameHash);
                    }
                    break;
                case 203:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var humanBoneId = (UnityEngine.HumanBodyBones)data.pars[0];
                        self.GetBoneTransform(humanBoneId);
                    }
                    break;
                case 229:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.Rebind();
                    }
                    break;
                case 278:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var tag = data.pars[0] as System.String;
                        self.CompareTag(tag);
                    }
                    break;
                case 281:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessageUpwards(methodName);
                    }
                    break;
                case 282:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessageUpwards(methodName, options);
                    }
                    break;
                case 284:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessage(methodName);
                    }
                    break;
                case 286:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessage(methodName, options);
                    }
                    break;
                case 289:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.BroadcastMessage(methodName);
                    }
                    break;
                case 290:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.BroadcastMessage(methodName, options);
                    }
                    break;
                case 304:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetInstanceID();
                    }
                    break;
                case 305:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetHashCode();
                    }
                    break;
                case 311:
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