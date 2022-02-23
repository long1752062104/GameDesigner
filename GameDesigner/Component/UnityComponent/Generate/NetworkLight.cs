#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// Light同步组件, 此代码由BuildComponentTools工具生成
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Light))]
    public class NetworkLight : NetworkBehaviour
    {

        private UnityEngine.Light self;
        public bool autoCheck;
        private UnityEngine.LightType type1;
        private UnityEngine.LightShape shape2;
        private System.Single spotAngle3;
        private System.Single innerSpotAngle4;
        private UnityEngine.Color color5;
        private System.Single colorTemperature6;
        private System.Boolean useColorTemperature7;
        private System.Single intensity8;
        private System.Single bounceIntensity9;
        private System.Boolean useBoundingSphereOverride10;
        private UnityEngine.Vector4 boundingSphereOverride11;
        private System.Int32 shadowCustomResolution12;
        private System.Single shadowBias13;
        private System.Single shadowNormalBias14;
        private System.Single shadowNearPlane15;
        private System.Boolean useShadowMatrixOverride16;
        private System.Single range17;
        private System.Int32 cullingMask18;
        private System.Int32 renderingLayerMask19;
        private UnityEngine.LightShadowCasterMode lightShadowCasterMode20;
        private System.Single shadowRadius21;
        private System.Single shadowAngle22;
        private UnityEngine.LightShadows shadows23;
        private System.Single shadowStrength24;
        private UnityEngine.Rendering.LightShadowResolution shadowResolution25;
        private System.Single cookieSize26;
        private UnityEngine.LightRenderMode renderMode27;
        private UnityEngine.Vector2 areaSize28;
        private UnityEngine.LightmapBakeType lightmapBakeType29;
        private System.Boolean enabled30;
        private System.String tag31;
        private System.String name32;
        private UnityEngine.HideFlags hideFlags33;
        private System.String tag1;
        private System.String methodName2;
        private System.String methodName3;
        private UnityEngine.SendMessageOptions options4;
        private System.String methodName5;
        private System.String methodName6;
        private UnityEngine.SendMessageOptions options7;
        private System.String methodName8;
        private System.String methodName9;
        private UnityEngine.SendMessageOptions options10;

        public override void Awake()
        {
            base.Awake();
            self = GetComponent<UnityEngine.Light>();
        }

        public UnityEngine.LightType type
        {
            get
            {
                return self.type;
            }
            set
            {
                if (type1 == value)
                    return;
                type1 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 0,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.LightShape shape
        {
            get
            {
                return self.shape;
            }
            set
            {
                if (shape2 == value)
                    return;
                shape2 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 1,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single spotAngle
        {
            get
            {
                return self.spotAngle;
            }
            set
            {
                if (spotAngle3 == value)
                    return;
                spotAngle3 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 2,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single innerSpotAngle
        {
            get
            {
                return self.innerSpotAngle;
            }
            set
            {
                if (innerSpotAngle4 == value)
                    return;
                innerSpotAngle4 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 3,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.Color color
        {
            get
            {
                return self.color;
            }
            set
            {
                if (color5 == value)
                    return;
                color5 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 4,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single colorTemperature
        {
            get
            {
                return self.colorTemperature;
            }
            set
            {
                if (colorTemperature6 == value)
                    return;
                colorTemperature6 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 5,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean useColorTemperature
        {
            get
            {
                return self.useColorTemperature;
            }
            set
            {
                if (useColorTemperature7 == value)
                    return;
                useColorTemperature7 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 6,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single intensity
        {
            get
            {
                return self.intensity;
            }
            set
            {
                if (intensity8 == value)
                    return;
                intensity8 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 7,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single bounceIntensity
        {
            get
            {
                return self.bounceIntensity;
            }
            set
            {
                if (bounceIntensity9 == value)
                    return;
                bounceIntensity9 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 8,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean useBoundingSphereOverride
        {
            get
            {
                return self.useBoundingSphereOverride;
            }
            set
            {
                if (useBoundingSphereOverride10 == value)
                    return;
                useBoundingSphereOverride10 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 9,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.Vector4 boundingSphereOverride
        {
            get
            {
                return self.boundingSphereOverride;
            }
            set
            {
                if (boundingSphereOverride11 == value)
                    return;
                boundingSphereOverride11 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 10,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Int32 shadowCustomResolution
        {
            get
            {
                return self.shadowCustomResolution;
            }
            set
            {
                if (shadowCustomResolution12 == value)
                    return;
                shadowCustomResolution12 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 11,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single shadowBias
        {
            get
            {
                return self.shadowBias;
            }
            set
            {
                if (shadowBias13 == value)
                    return;
                shadowBias13 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 12,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single shadowNormalBias
        {
            get
            {
                return self.shadowNormalBias;
            }
            set
            {
                if (shadowNormalBias14 == value)
                    return;
                shadowNormalBias14 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 13,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single shadowNearPlane
        {
            get
            {
                return self.shadowNearPlane;
            }
            set
            {
                if (shadowNearPlane15 == value)
                    return;
                shadowNearPlane15 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 14,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean useShadowMatrixOverride
        {
            get
            {
                return self.useShadowMatrixOverride;
            }
            set
            {
                if (useShadowMatrixOverride16 == value)
                    return;
                useShadowMatrixOverride16 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 15,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single range
        {
            get
            {
                return self.range;
            }
            set
            {
                if (range17 == value)
                    return;
                range17 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 17,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Int32 cullingMask
        {
            get
            {
                return self.cullingMask;
            }
            set
            {
                if (cullingMask18 == value)
                    return;
                cullingMask18 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 20,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Int32 renderingLayerMask
        {
            get
            {
                return self.renderingLayerMask;
            }
            set
            {
                if (renderingLayerMask19 == value)
                    return;
                renderingLayerMask19 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 21,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.LightShadowCasterMode lightShadowCasterMode
        {
            get
            {
                return self.lightShadowCasterMode;
            }
            set
            {
                if (lightShadowCasterMode20 == value)
                    return;
                lightShadowCasterMode20 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 22,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single shadowRadius
        {
            get
            {
#if UNITY_EDITOR
                return self.shadowRadius;
#else
                return default;
#endif
            }
            set
            {
                if (shadowRadius21 == value)
                    return;
                shadowRadius21 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 23,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single shadowAngle
        {
            get
            {
#if UNITY_EDITOR
                return self.shadowAngle;
#else
                return default;
#endif

            }
            set
            {
                if (shadowAngle22 == value)
                    return;
                shadowAngle22 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 24,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.LightShadows shadows
        {
            get
            {
                return self.shadows;
            }
            set
            {
                if (shadows23 == value)
                    return;
                shadows23 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 25,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single shadowStrength
        {
            get
            {
                return self.shadowStrength;
            }
            set
            {
                if (shadowStrength24 == value)
                    return;
                shadowStrength24 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 26,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.Rendering.LightShadowResolution shadowResolution
        {
            get
            {
                return self.shadowResolution;
            }
            set
            {
                if (shadowResolution25 == value)
                    return;
                shadowResolution25 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 27,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single cookieSize
        {
            get
            {
                return self.cookieSize;
            }
            set
            {
                if (cookieSize26 == value)
                    return;
                cookieSize26 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 31,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.LightRenderMode renderMode
        {
            get
            {
#if UNITY_EDITOR
                return self.renderMode;
#else
                return default;
#endif

            }
            set
            {
                if (renderMode27 == value)
                    return;
                renderMode27 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 33,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.Vector2 areaSize
        {
            get
            {
#if UNITY_EDITOR
                return self.areaSize;
#else
                return default;
#endif

            }
            set
            {
                if (areaSize28 == value)
                    return;
                areaSize28 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 35,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.LightmapBakeType lightmapBakeType
        {
            get
            {
#if UNITY_EDITOR
                return self.lightmapBakeType;
#else
                return default;
#endif
            }
            set
            {
                if (lightmapBakeType29 == value)
                    return;
                lightmapBakeType29 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 36,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                if (enabled30 == value)
                    return;
                enabled30 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 44,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                if (tag31 == value)
                    return;
                tag31 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 48,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                if (name32 == value)
                    return;
                name32 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 62,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                if (hideFlags33 == value)
                    return;
                hideFlags33 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 63,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public void CompareTag(System.String tag, bool always = false)
        {
            if (tag == tag1 & !always) return;
            tag1 = tag;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { tag } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 193,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, bool always = false)
        {
            if (methodName == methodName2 & !always) return;
            methodName2 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 196,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName3 & options == options4 & !always) return;
            methodName3 = methodName;
            options4 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 197,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName5 & !always) return;
            methodName5 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 199,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName6 & options == options7 & !always) return;
            methodName6 = methodName;
            options7 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 201,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName8 & !always) return;
            methodName8 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 204,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName9 & options == options10 & !always) return;
            methodName9 = methodName;
            options10 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 205,
                buffer = buffer
            });
        }
        public void GetInstanceID(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 219,
                buffer = buffer
            });
        }
        public void GetHashCode(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 220,
                buffer = buffer
            });
        }
        public void ToString(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 226,
                buffer = buffer
            });
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;
            type = type;
            shape = shape;
            spotAngle = spotAngle;
            innerSpotAngle = innerSpotAngle;
            color = color;
            colorTemperature = colorTemperature;
            useColorTemperature = useColorTemperature;
            intensity = intensity;
            bounceIntensity = bounceIntensity;
            useBoundingSphereOverride = useBoundingSphereOverride;
            boundingSphereOverride = boundingSphereOverride;
            shadowCustomResolution = shadowCustomResolution;
            shadowBias = shadowBias;
            shadowNormalBias = shadowNormalBias;
            shadowNearPlane = shadowNearPlane;
            useShadowMatrixOverride = useShadowMatrixOverride;
            range = range;
            cullingMask = cullingMask;
            renderingLayerMask = renderingLayerMask;
            lightShadowCasterMode = lightShadowCasterMode;
            shadowRadius = shadowRadius;
            shadowAngle = shadowAngle;
            shadows = shadows;
            shadowStrength = shadowStrength;
            shadowResolution = shadowResolution;
            cookieSize = cookieSize;
            renderMode = renderMode;
            areaSize = areaSize;
            lightmapBakeType = lightmapBakeType;
            enabled = enabled;
            tag = tag;
            name = name;
            hideFlags = hideFlags;
        }

        public override void OnNetworkOperationHandler(Operation opt)
        {
            if (opt.cmd != Command.BuildComponent)
                return;
            switch (opt.index1)
            {
                case 0:
                    type1 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.LightType>(new Net.System.Segment(opt.buffer, false));
                    self.type = type1;
                    break;
                case 1:
                    shape2 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.LightShape>(new Net.System.Segment(opt.buffer, false));
                    self.shape = shape2;
                    break;
                case 2:
                    spotAngle3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.spotAngle = spotAngle3;
                    break;
                case 3:
                    innerSpotAngle4 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.innerSpotAngle = innerSpotAngle4;
                    break;
                case 4:
                    color5 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Color>(new Net.System.Segment(opt.buffer, false));
                    self.color = color5;
                    break;
                case 5:
                    colorTemperature6 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.colorTemperature = colorTemperature6;
                    break;
                case 6:
                    useColorTemperature7 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.useColorTemperature = useColorTemperature7;
                    break;
                case 7:
                    intensity8 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.intensity = intensity8;
                    break;
                case 8:
                    bounceIntensity9 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.bounceIntensity = bounceIntensity9;
                    break;
                case 9:
                    useBoundingSphereOverride10 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.useBoundingSphereOverride = useBoundingSphereOverride10;
                    break;
                case 10:
                    boundingSphereOverride11 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector4>(new Net.System.Segment(opt.buffer, false));
                    self.boundingSphereOverride = boundingSphereOverride11;
                    break;
                case 11:
                    shadowCustomResolution12 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.shadowCustomResolution = shadowCustomResolution12;
                    break;
                case 12:
                    shadowBias13 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.shadowBias = shadowBias13;
                    break;
                case 13:
                    shadowNormalBias14 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.shadowNormalBias = shadowNormalBias14;
                    break;
                case 14:
                    shadowNearPlane15 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.shadowNearPlane = shadowNearPlane15;
                    break;
                case 15:
                    useShadowMatrixOverride16 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.useShadowMatrixOverride = useShadowMatrixOverride16;
                    break;
                case 17:
                    range17 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.range = range17;
                    break;
                case 20:
                    cullingMask18 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.cullingMask = cullingMask18;
                    break;
                case 21:
                    renderingLayerMask19 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.renderingLayerMask = renderingLayerMask19;
                    break;
                case 22:
                    lightShadowCasterMode20 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.LightShadowCasterMode>(new Net.System.Segment(opt.buffer, false));
                    self.lightShadowCasterMode = lightShadowCasterMode20;
                    break;
                case 23:
                    shadowRadius21 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
#if UNITY_EDITOR
                    self.shadowRadius = shadowRadius21;
#endif

                    break;
                case 24:
                    shadowAngle22 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
#if UNITY_EDITOR
                    self.shadowAngle = shadowAngle22;
#endif

                    break;
                case 25:
                    shadows23 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.LightShadows>(new Net.System.Segment(opt.buffer, false));
                    self.shadows = shadows23;
                    break;
                case 26:
                    shadowStrength24 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.shadowStrength = shadowStrength24;
                    break;
                case 27:
                    shadowResolution25 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Rendering.LightShadowResolution>(new Net.System.Segment(opt.buffer, false));
                    self.shadowResolution = shadowResolution25;
                    break;
                case 31:
                    cookieSize26 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.cookieSize = cookieSize26;
                    break;
                case 33:
                    renderMode27 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.LightRenderMode>(new Net.System.Segment(opt.buffer, false));
                    self.renderMode = renderMode27;
                    break;
                case 35:
                    areaSize28 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector2>(new Net.System.Segment(opt.buffer, false));
#if UNITY_EDITOR
                    self.areaSize = areaSize28;
# endif
                    break;
                case 36:
                    lightmapBakeType29 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.LightmapBakeType>(new Net.System.Segment(opt.buffer, false));
#if UNITY_EDITOR
                    self.lightmapBakeType = lightmapBakeType29;
#endif

                    break;
                case 44:
                    enabled30 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.enabled = enabled30;
                    break;
                case 48:
                    tag31 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.tag = tag31;
                    break;
                case 62:
                    name32 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.name = name32;
                    break;
                case 63:
                    hideFlags33 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.HideFlags>(new Net.System.Segment(opt.buffer, false));
                    self.hideFlags = hideFlags33;
                    break;
                case 193:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var tag = data.pars[0] as System.String;
                        self.CompareTag(tag);
                    }
                    break;
                case 196:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessageUpwards(methodName);
                    }
                    break;
                case 197:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessageUpwards(methodName, options);
                    }
                    break;
                case 199:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessage(methodName);
                    }
                    break;
                case 201:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessage(methodName, options);
                    }
                    break;
                case 204:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.BroadcastMessage(methodName);
                    }
                    break;
                case 205:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.BroadcastMessage(methodName, options);
                    }
                    break;
                case 219:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetInstanceID();
                    }
                    break;
                case 220:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetHashCode();
                    }
                    break;
                case 226:
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