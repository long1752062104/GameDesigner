#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// Camera同步组件, 此代码由BuildComponentTools工具生成, 如果同步发生相互影响的字段或属性, 请自行检查处理一下!
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class NetworkCamera : NetworkBehaviour
    {
        private UnityEngine.Camera self;
        public bool autoCheck;
        private System.Single nearClipPlane1;
        private System.Single farClipPlane2;
        private System.Single fieldOfView3;
        private UnityEngine.RenderingPath renderingPath4;
        private System.Boolean allowHDR5;
        private System.Boolean allowMSAA6;
        private System.Boolean allowDynamicResolution7;
        private System.Boolean forceIntoRenderTexture8;
        private System.Single orthographicSize9;
        private System.Boolean orthographic10;
        private UnityEngine.Rendering.OpaqueSortMode opaqueSortMode11;
        private UnityEngine.TransparencySortMode transparencySortMode12;
        private UnityEngine.Vector3 transparencySortAxis13;
        private System.Single depth14;
        private System.Single aspect15;
        private System.Int32 cullingMask16;
        private System.Int32 eventMask17;
        private System.Boolean layerCullSpherical18;
        private UnityEngine.CameraType cameraType19;
        private System.UInt64 overrideSceneCullingMask20;
        private System.Boolean useOcclusionCulling21;
        private UnityEngine.Color backgroundColor22;
        private UnityEngine.CameraClearFlags clearFlags23;
        private UnityEngine.DepthTextureMode depthTextureMode24;
        private System.Boolean clearStencilAfterLightingPass25;
        private System.Boolean usePhysicalProperties26;
        private UnityEngine.Vector2 sensorSize27;
        private UnityEngine.Vector2 lensShift28;
        private System.Single focalLength29;
        private UnityEngine.Camera.GateFitMode gateFit30;
        private UnityEngine.Rect rect31;
        private UnityEngine.RenderTexture targetTexture33;
        private System.Int32 targetDisplay34;
        private System.Boolean useJitteredProjectionMatrixForTransparentRendering35;
        private System.Single stereoSeparation36;
        private System.Single stereoConvergence37;
        private UnityEngine.StereoTargetEyeMask stereoTargetEye38;
        private System.Boolean enabled39;
        private System.String tag40;
        private System.String name41;
        private UnityEngine.HideFlags hideFlags42;
        private UnityEngine.Vector4 clipPlane1;
        private UnityEngine.Vector3 position2;
        private UnityEngine.Camera.MonoOrStereoscopicEye eye3;
        private UnityEngine.Vector3 position4;
        private UnityEngine.Camera.MonoOrStereoscopicEye eye5;
        private UnityEngine.Vector3 position6;
        private UnityEngine.Camera.MonoOrStereoscopicEye eye7;
        private UnityEngine.Vector3 position8;
        private UnityEngine.Camera.MonoOrStereoscopicEye eye9;
        private UnityEngine.Vector3 position10;
        private UnityEngine.Vector3 position11;
        private UnityEngine.Vector3 position12;
        private UnityEngine.Vector3 position13;
        private UnityEngine.Vector3 position14;
        private UnityEngine.Vector3 position15;
        private UnityEngine.Vector3 pos16;
        private UnityEngine.Camera.MonoOrStereoscopicEye eye17;
        private UnityEngine.Vector3 pos18;
        private UnityEngine.Vector3 pos19;
        private UnityEngine.Camera.MonoOrStereoscopicEye eye20;
        private UnityEngine.Vector3 pos21;
        private UnityEngine.Camera.StereoscopicEye eye22;
        private UnityEngine.Camera.StereoscopicEye eye23;
        private UnityEngine.Camera.StereoscopicEye eye24;
        private System.String tag25;
        private System.String methodName26;
        private System.String methodName27;
        private UnityEngine.SendMessageOptions options28;
        private System.String methodName29;
        private System.String methodName30;
        private UnityEngine.SendMessageOptions options31;
        private System.String methodName32;
        private System.String methodName33;
        private UnityEngine.SendMessageOptions options34;

        public override void Awake()
        {
            base.Awake();
            self = GetComponent<UnityEngine.Camera>();
            nearClipPlane1 = self.nearClipPlane;
            farClipPlane2 = self.farClipPlane;
            fieldOfView3 = self.fieldOfView;
            renderingPath4 = self.renderingPath;
            allowHDR5 = self.allowHDR;
            allowMSAA6 = self.allowMSAA;
            allowDynamicResolution7 = self.allowDynamicResolution;
            forceIntoRenderTexture8 = self.forceIntoRenderTexture;
            orthographicSize9 = self.orthographicSize;
            orthographic10 = self.orthographic;
            opaqueSortMode11 = self.opaqueSortMode;
            transparencySortMode12 = self.transparencySortMode;
            transparencySortAxis13 = self.transparencySortAxis;
            depth14 = self.depth;
            aspect15 = self.aspect;
            cullingMask16 = self.cullingMask;
            eventMask17 = self.eventMask;
            layerCullSpherical18 = self.layerCullSpherical;
            cameraType19 = self.cameraType;
            overrideSceneCullingMask20 = self.overrideSceneCullingMask;
            useOcclusionCulling21 = self.useOcclusionCulling;
            backgroundColor22 = self.backgroundColor;
            clearFlags23 = self.clearFlags;
            depthTextureMode24 = self.depthTextureMode;
            clearStencilAfterLightingPass25 = self.clearStencilAfterLightingPass;
            usePhysicalProperties26 = self.usePhysicalProperties;
            sensorSize27 = self.sensorSize;
            lensShift28 = self.lensShift;
            focalLength29 = self.focalLength;
            gateFit30 = self.gateFit;
            rect31 = self.rect;
            targetTexture33 = self.targetTexture;
            targetDisplay34 = self.targetDisplay;
            useJitteredProjectionMatrixForTransparentRendering35 = self.useJitteredProjectionMatrixForTransparentRendering;
            stereoSeparation36 = self.stereoSeparation;
            stereoConvergence37 = self.stereoConvergence;
            stereoTargetEye38 = self.stereoTargetEye;
            enabled39 = self.enabled;
            tag40 = self.tag;
            name41 = self.name;
            hideFlags42 = self.hideFlags;
        }

        public System.Single nearClipPlane
        {
            get
            {
                return self.nearClipPlane;
            }
            set
            {
                if (nearClipPlane1 == value)
                    return;
                nearClipPlane1 = value;
                self.nearClipPlane = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 0,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single farClipPlane
        {
            get
            {
                return self.farClipPlane;
            }
            set
            {
                if (farClipPlane2 == value)
                    return;
                farClipPlane2 = value;
                self.farClipPlane = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 1,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single fieldOfView
        {
            get
            {
                return self.fieldOfView;
            }
            set
            {
                if (fieldOfView3 == value)
                    return;
                fieldOfView3 = value;
                self.fieldOfView = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 2,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.RenderingPath renderingPath
        {
            get
            {
                return self.renderingPath;
            }
            set
            {
                if (renderingPath4 == value)
                    return;
                renderingPath4 = value;
                self.renderingPath = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 3,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean allowHDR
        {
            get
            {
                return self.allowHDR;
            }
            set
            {
                if (allowHDR5 == value)
                    return;
                allowHDR5 = value;
                self.allowHDR = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 5,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean allowMSAA
        {
            get
            {
                return self.allowMSAA;
            }
            set
            {
                if (allowMSAA6 == value)
                    return;
                allowMSAA6 = value;
                self.allowMSAA = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 6,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean allowDynamicResolution
        {
            get
            {
                return self.allowDynamicResolution;
            }
            set
            {
                if (allowDynamicResolution7 == value)
                    return;
                allowDynamicResolution7 = value;
                self.allowDynamicResolution = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 7,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean forceIntoRenderTexture
        {
            get
            {
                return self.forceIntoRenderTexture;
            }
            set
            {
                if (forceIntoRenderTexture8 == value)
                    return;
                forceIntoRenderTexture8 = value;
                self.forceIntoRenderTexture = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 8,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single orthographicSize
        {
            get
            {
                return self.orthographicSize;
            }
            set
            {
                if (orthographicSize9 == value)
                    return;
                orthographicSize9 = value;
                self.orthographicSize = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 9,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean orthographic
        {
            get
            {
                return self.orthographic;
            }
            set
            {
                if (orthographic10 == value)
                    return;
                orthographic10 = value;
                self.orthographic = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 10,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Rendering.OpaqueSortMode opaqueSortMode
        {
            get
            {
                return self.opaqueSortMode;
            }
            set
            {
                if (opaqueSortMode11 == value)
                    return;
                opaqueSortMode11 = value;
                self.opaqueSortMode = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 11,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.TransparencySortMode transparencySortMode
        {
            get
            {
                return self.transparencySortMode;
            }
            set
            {
                if (transparencySortMode12 == value)
                    return;
                transparencySortMode12 = value;
                self.transparencySortMode = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 12,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Vector3 transparencySortAxis
        {
            get
            {
                return self.transparencySortAxis;
            }
            set
            {
                if (transparencySortAxis13 == value)
                    return;
                transparencySortAxis13 = value;
                self.transparencySortAxis = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 13,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single depth
        {
            get
            {
                return self.depth;
            }
            set
            {
                if (depth14 == value)
                    return;
                depth14 = value;
                self.depth = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 14,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single aspect
        {
            get
            {
                return self.aspect;
            }
            set
            {
                if (aspect15 == value)
                    return;
                aspect15 = value;
                self.aspect = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 15,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
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
                if (cullingMask16 == value)
                    return;
                cullingMask16 = value;
                self.cullingMask = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 17,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Int32 eventMask
        {
            get
            {
                return self.eventMask;
            }
            set
            {
                if (eventMask17 == value)
                    return;
                eventMask17 = value;
                self.eventMask = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 18,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean layerCullSpherical
        {
            get
            {
                return self.layerCullSpherical;
            }
            set
            {
                if (layerCullSpherical18 == value)
                    return;
                layerCullSpherical18 = value;
                self.layerCullSpherical = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 19,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.CameraType cameraType
        {
            get
            {
                return self.cameraType;
            }
            set
            {
                if (cameraType19 == value)
                    return;
                cameraType19 = value;
                self.cameraType = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 20,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.UInt64 overrideSceneCullingMask
        {
            get
            {
                return self.overrideSceneCullingMask;
            }
            set
            {
                if (overrideSceneCullingMask20 == value)
                    return;
                overrideSceneCullingMask20 = value;
                self.overrideSceneCullingMask = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 21,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean useOcclusionCulling
        {
            get
            {
                return self.useOcclusionCulling;
            }
            set
            {
                if (useOcclusionCulling21 == value)
                    return;
                useOcclusionCulling21 = value;
                self.useOcclusionCulling = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 23,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Color backgroundColor
        {
            get
            {
                return self.backgroundColor;
            }
            set
            {
                if (backgroundColor22 == value)
                    return;
                backgroundColor22 = value;
                self.backgroundColor = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 25,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.CameraClearFlags clearFlags
        {
            get
            {
                return self.clearFlags;
            }
            set
            {
                if (clearFlags23 == value)
                    return;
                clearFlags23 = value;
                self.clearFlags = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 26,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.DepthTextureMode depthTextureMode
        {
            get
            {
                return self.depthTextureMode;
            }
            set
            {
                if (depthTextureMode24 == value)
                    return;
                depthTextureMode24 = value;
                self.depthTextureMode = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 27,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean clearStencilAfterLightingPass
        {
            get
            {
                return self.clearStencilAfterLightingPass;
            }
            set
            {
                if (clearStencilAfterLightingPass25 == value)
                    return;
                clearStencilAfterLightingPass25 = value;
                self.clearStencilAfterLightingPass = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 28,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean usePhysicalProperties
        {
            get
            {
                return self.usePhysicalProperties;
            }
            set
            {
                if (usePhysicalProperties26 == value)
                    return;
                usePhysicalProperties26 = value;
                self.usePhysicalProperties = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 29,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Vector2 sensorSize
        {
            get
            {
                return self.sensorSize;
            }
            set
            {
                if (sensorSize27 == value)
                    return;
                sensorSize27 = value;
                self.sensorSize = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 30,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Vector2 lensShift
        {
            get
            {
                return self.lensShift;
            }
            set
            {
                if (lensShift28 == value)
                    return;
                lensShift28 = value;
                self.lensShift = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 31,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single focalLength
        {
            get
            {
                return self.focalLength;
            }
            set
            {
                if (focalLength29 == value)
                    return;
                focalLength29 = value;
                self.focalLength = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 32,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Camera.GateFitMode gateFit
        {
            get
            {
                return self.gateFit;
            }
            set
            {
                if (gateFit30 == value)
                    return;
                gateFit30 = value;
                self.gateFit = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 33,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.Rect rect
        {
            get
            {
                return self.rect;
            }
            set
            {
                if (rect31 == value)
                    return;
                rect31 = value;
                self.rect = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 34,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.RenderTexture targetTexture
        {
            get
            {
                return self.targetTexture;
            }
            set
            {
                if (targetTexture33 == value)
                    return;
                targetTexture33 = value;
                self.targetTexture = value;
                if (!NetworkResources.I.TryGetValue(targetTexture33, out ObjectRecord objectRecord))
                    return;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 40,
                    index2 = objectRecord.ID,
                    name = objectRecord.path,
                    uid = ClientManager.UID
                });
            }
        }

        public System.Int32 targetDisplay
        {
            get
            {
                return self.targetDisplay;
            }
            set
            {
                if (targetDisplay34 == value)
                    return;
                targetDisplay34 = value;
                self.targetDisplay = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 42,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Boolean useJitteredProjectionMatrixForTransparentRendering
        {
            get
            {
                return self.useJitteredProjectionMatrixForTransparentRendering;
            }
            set
            {
                if (useJitteredProjectionMatrixForTransparentRendering35 == value)
                    return;
                useJitteredProjectionMatrixForTransparentRendering35 = value;
                self.useJitteredProjectionMatrixForTransparentRendering = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 47,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single stereoSeparation
        {
            get
            {
                return self.stereoSeparation;
            }
            set
            {
                if (stereoSeparation36 == value)
                    return;
                stereoSeparation36 = value;
                self.stereoSeparation = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 51,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public System.Single stereoConvergence
        {
            get
            {
                return self.stereoConvergence;
            }
            set
            {
                if (stereoConvergence37 == value)
                    return;
                stereoConvergence37 = value;
                self.stereoConvergence = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 52,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public UnityEngine.StereoTargetEyeMask stereoTargetEye
        {
            get
            {
                return self.stereoTargetEye;
            }
            set
            {
                if (stereoTargetEye38 == value)
                    return;
                stereoTargetEye38 = value;
                self.stereoTargetEye = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 54,
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
                if (enabled39 == value)
                    return;
                enabled39 = value;
                self.enabled = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 63,
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
                if (tag40 == value)
                    return;
                tag40 = value;
                self.tag = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 67,
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
                if (name41 == value)
                    return;
                name41 = value;
                self.name = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 81,
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
                if (hideFlags42 == value)
                    return;
                hideFlags42 = value;
                self.hideFlags = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 82,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true),
                    uid = ClientManager.UID
                });
            }
        }

        public void GetGateFittedLensShift(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 156,
                buffer = buffer
            });
        }
        public void CalculateObliqueMatrix(UnityEngine.Vector4 clipPlane, bool always = false)
        {
            if (clipPlane == clipPlane1 & !always) return;
            clipPlane1 = clipPlane;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { clipPlane } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 184,
                buffer = buffer
            });
        }
        public void WorldToScreenPoint(UnityEngine.Vector3 position, UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
        {
            if (position == position2 & eye == eye3 & !always) return;
            position2 = position;
            eye3 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position, eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 185,
                buffer = buffer
            });
        }
        public void WorldToViewportPoint(UnityEngine.Vector3 position, UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
        {
            if (position == position4 & eye == eye5 & !always) return;
            position4 = position;
            eye5 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position, eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 186,
                buffer = buffer
            });
        }
        public void ViewportToWorldPoint(UnityEngine.Vector3 position, UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
        {
            if (position == position6 & eye == eye7 & !always) return;
            position6 = position;
            eye7 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position, eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 187,
                buffer = buffer
            });
        }
        public void ScreenToWorldPoint(UnityEngine.Vector3 position, UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
        {
            if (position == position8 & eye == eye9 & !always) return;
            position8 = position;
            eye9 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position, eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 188,
                buffer = buffer
            });
        }
        public void WorldToScreenPoint(UnityEngine.Vector3 position, bool always = false)
        {
            if (position == position10 & !always) return;
            position10 = position;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 189,
                buffer = buffer
            });
        }
        public void WorldToViewportPoint(UnityEngine.Vector3 position, bool always = false)
        {
            if (position == position11 & !always) return;
            position11 = position;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 190,
                buffer = buffer
            });
        }
        public void ViewportToWorldPoint(UnityEngine.Vector3 position, bool always = false)
        {
            if (position == position12 & !always) return;
            position12 = position;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 191,
                buffer = buffer
            });
        }
        public void ScreenToWorldPoint(UnityEngine.Vector3 position, bool always = false)
        {
            if (position == position13 & !always) return;
            position13 = position;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 192,
                buffer = buffer
            });
        }
        public void ScreenToViewportPoint(UnityEngine.Vector3 position, bool always = false)
        {
            if (position == position14 & !always) return;
            position14 = position;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 193,
                buffer = buffer
            });
        }
        public void ViewportToScreenPoint(UnityEngine.Vector3 position, bool always = false)
        {
            if (position == position15 & !always) return;
            position15 = position;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 194,
                buffer = buffer
            });
        }
        public void ViewportPointToRay(UnityEngine.Vector3 pos, UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
        {
            if (pos == pos16 & eye == eye17 & !always) return;
            pos16 = pos;
            eye17 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos, eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 195,
                buffer = buffer
            });
        }
        public void ViewportPointToRay(UnityEngine.Vector3 pos, bool always = false)
        {
            if (pos == pos18 & !always) return;
            pos18 = pos;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 196,
                buffer = buffer
            });
        }
        public void ScreenPointToRay(UnityEngine.Vector3 pos, UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
        {
            if (pos == pos19 & eye == eye20 & !always) return;
            pos19 = pos;
            eye20 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos, eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 197,
                buffer = buffer
            });
        }
        public void ScreenPointToRay(UnityEngine.Vector3 pos, bool always = false)
        {
            if (pos == pos21 & !always) return;
            pos21 = pos;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 198,
                buffer = buffer
            });
        }
        public void GetStereoNonJitteredProjectionMatrix(UnityEngine.Camera.StereoscopicEye eye, bool always = false)
        {
            if (eye == eye22 & !always) return;
            eye22 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 211,
                buffer = buffer
            });
        }
        public void GetStereoViewMatrix(UnityEngine.Camera.StereoscopicEye eye, bool always = false)
        {
            if (eye == eye23 & !always) return;
            eye23 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 212,
                buffer = buffer
            });
        }
        public void GetStereoProjectionMatrix(UnityEngine.Camera.StereoscopicEye eye, bool always = false)
        {
            if (eye == eye24 & !always) return;
            eye24 = eye;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { eye } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 214,
                buffer = buffer
            });
        }
        public void CompareTag(System.String tag, bool always = false)
        {
            if (tag == tag25 & !always) return;
            tag25 = tag;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { tag } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 290,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, bool always = false)
        {
            if (methodName == methodName26 & !always) return;
            methodName26 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 293,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName27 & options == options28 & !always) return;
            methodName27 = methodName;
            options28 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 294,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName29 & !always) return;
            methodName29 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 296,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName30 & options == options31 & !always) return;
            methodName30 = methodName;
            options31 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 298,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName32 & !always) return;
            methodName32 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 301,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName33 & options == options34 & !always) return;
            methodName33 = methodName;
            options34 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 302,
                buffer = buffer
            });
        }
        public void GetInstanceID(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 316,
                buffer = buffer
            });
        }
        public void GetHashCode(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 317,
                buffer = buffer
            });
        }
        public void ToString(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
            {
                index = netObj.registerObjectIndex,
                index1 = 323,
                buffer = buffer
            });
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;
            nearClipPlane = nearClipPlane;
            farClipPlane = farClipPlane;
            fieldOfView = fieldOfView;
            renderingPath = renderingPath;
            allowHDR = allowHDR;
            allowMSAA = allowMSAA;
            allowDynamicResolution = allowDynamicResolution;
            forceIntoRenderTexture = forceIntoRenderTexture;
            orthographicSize = orthographicSize;
            orthographic = orthographic;
            opaqueSortMode = opaqueSortMode;
            transparencySortMode = transparencySortMode;
            transparencySortAxis = transparencySortAxis;
            depth = depth;
            aspect = aspect;
            cullingMask = cullingMask;
            eventMask = eventMask;
            layerCullSpherical = layerCullSpherical;
            cameraType = cameraType;
            overrideSceneCullingMask = overrideSceneCullingMask;
            useOcclusionCulling = useOcclusionCulling;
            backgroundColor = backgroundColor;
            clearFlags = clearFlags;
            depthTextureMode = depthTextureMode;
            clearStencilAfterLightingPass = clearStencilAfterLightingPass;
            usePhysicalProperties = usePhysicalProperties;
            sensorSize = sensorSize;
            lensShift = lensShift;
            focalLength = focalLength;
            gateFit = gateFit;
            rect = rect;
            targetTexture = targetTexture;
            targetDisplay = targetDisplay;
            useJitteredProjectionMatrixForTransparentRendering = useJitteredProjectionMatrixForTransparentRendering;
            stereoSeparation = stereoSeparation;
            stereoConvergence = stereoConvergence;
            stereoTargetEye = stereoTargetEye;
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
                    if (opt.uid == ClientManager.UID)
                        return;
                    nearClipPlane1 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.nearClipPlane = nearClipPlane1;
                    break;
                case 1:
                    if (opt.uid == ClientManager.UID)
                        return;
                    farClipPlane2 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.farClipPlane = farClipPlane2;
                    break;
                case 2:
                    if (opt.uid == ClientManager.UID)
                        return;
                    fieldOfView3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.fieldOfView = fieldOfView3;
                    break;
                case 3:
                    if (opt.uid == ClientManager.UID)
                        return;
                    renderingPath4 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.RenderingPath>(new Net.System.Segment(opt.buffer, false));
                    self.renderingPath = renderingPath4;
                    break;
                case 5:
                    if (opt.uid == ClientManager.UID)
                        return;
                    allowHDR5 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.allowHDR = allowHDR5;
                    break;
                case 6:
                    if (opt.uid == ClientManager.UID)
                        return;
                    allowMSAA6 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.allowMSAA = allowMSAA6;
                    break;
                case 7:
                    if (opt.uid == ClientManager.UID)
                        return;
                    allowDynamicResolution7 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.allowDynamicResolution = allowDynamicResolution7;
                    break;
                case 8:
                    if (opt.uid == ClientManager.UID)
                        return;
                    forceIntoRenderTexture8 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.forceIntoRenderTexture = forceIntoRenderTexture8;
                    break;
                case 9:
                    if (opt.uid == ClientManager.UID)
                        return;
                    orthographicSize9 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.orthographicSize = orthographicSize9;
                    break;
                case 10:
                    if (opt.uid == ClientManager.UID)
                        return;
                    orthographic10 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.orthographic = orthographic10;
                    break;
                case 11:
                    if (opt.uid == ClientManager.UID)
                        return;
                    opaqueSortMode11 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Rendering.OpaqueSortMode>(new Net.System.Segment(opt.buffer, false));
                    self.opaqueSortMode = opaqueSortMode11;
                    break;
                case 12:
                    if (opt.uid == ClientManager.UID)
                        return;
                    transparencySortMode12 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.TransparencySortMode>(new Net.System.Segment(opt.buffer, false));
                    self.transparencySortMode = transparencySortMode12;
                    break;
                case 13:
                    if (opt.uid == ClientManager.UID)
                        return;
                    transparencySortAxis13 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector3>(new Net.System.Segment(opt.buffer, false));
                    self.transparencySortAxis = transparencySortAxis13;
                    break;
                case 14:
                    if (opt.uid == ClientManager.UID)
                        return;
                    depth14 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.depth = depth14;
                    break;
                case 15:
                    if (opt.uid == ClientManager.UID)
                        return;
                    aspect15 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.aspect = aspect15;
                    break;
                case 17:
                    if (opt.uid == ClientManager.UID)
                        return;
                    cullingMask16 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.cullingMask = cullingMask16;
                    break;
                case 18:
                    if (opt.uid == ClientManager.UID)
                        return;
                    eventMask17 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.eventMask = eventMask17;
                    break;
                case 19:
                    if (opt.uid == ClientManager.UID)
                        return;
                    layerCullSpherical18 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.layerCullSpherical = layerCullSpherical18;
                    break;
                case 20:
                    if (opt.uid == ClientManager.UID)
                        return;
                    cameraType19 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.CameraType>(new Net.System.Segment(opt.buffer, false));
                    self.cameraType = cameraType19;
                    break;
                case 21:
                    if (opt.uid == ClientManager.UID)
                        return;
                    overrideSceneCullingMask20 = Net.Serialize.NetConvertFast2.DeserializeObject<System.UInt64>(new Net.System.Segment(opt.buffer, false));
                    self.overrideSceneCullingMask = overrideSceneCullingMask20;
                    break;
                case 23:
                    if (opt.uid == ClientManager.UID)
                        return;
                    useOcclusionCulling21 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.useOcclusionCulling = useOcclusionCulling21;
                    break;
                case 25:
                    if (opt.uid == ClientManager.UID)
                        return;
                    backgroundColor22 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Color>(new Net.System.Segment(opt.buffer, false));
                    self.backgroundColor = backgroundColor22;
                    break;
                case 26:
                    if (opt.uid == ClientManager.UID)
                        return;
                    clearFlags23 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.CameraClearFlags>(new Net.System.Segment(opt.buffer, false));
                    self.clearFlags = clearFlags23;
                    break;
                case 27:
                    if (opt.uid == ClientManager.UID)
                        return;
                    depthTextureMode24 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.DepthTextureMode>(new Net.System.Segment(opt.buffer, false));
                    self.depthTextureMode = depthTextureMode24;
                    break;
                case 28:
                    if (opt.uid == ClientManager.UID)
                        return;
                    clearStencilAfterLightingPass25 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.clearStencilAfterLightingPass = clearStencilAfterLightingPass25;
                    break;
                case 29:
                    if (opt.uid == ClientManager.UID)
                        return;
                    usePhysicalProperties26 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.usePhysicalProperties = usePhysicalProperties26;
                    break;
                case 30:
                    if (opt.uid == ClientManager.UID)
                        return;
                    sensorSize27 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector2>(new Net.System.Segment(opt.buffer, false));
                    self.sensorSize = sensorSize27;
                    break;
                case 31:
                    if (opt.uid == ClientManager.UID)
                        return;
                    lensShift28 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector2>(new Net.System.Segment(opt.buffer, false));
                    self.lensShift = lensShift28;
                    break;
                case 32:
                    if (opt.uid == ClientManager.UID)
                        return;
                    focalLength29 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.focalLength = focalLength29;
                    break;
                case 33:
                    if (opt.uid == ClientManager.UID)
                        return;
                    gateFit30 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Camera.GateFitMode>(new Net.System.Segment(opt.buffer, false));
                    self.gateFit = gateFit30;
                    break;
                case 34:
                    if (opt.uid == ClientManager.UID)
                        return;
                    rect31 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Rect>(new Net.System.Segment(opt.buffer, false));
                    self.rect = rect31;
                    break;
                case 40:
                    if (opt.uid == ClientManager.UID)
                        return;
                    targetTexture33 = NetworkResources.I.GetObject<UnityEngine.RenderTexture>(opt.index2, opt.name);
                    self.targetTexture = targetTexture33;
                    break;
                case 42:
                    if (opt.uid == ClientManager.UID)
                        return;
                    targetDisplay34 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.targetDisplay = targetDisplay34;
                    break;
                case 47:
                    if (opt.uid == ClientManager.UID)
                        return;
                    useJitteredProjectionMatrixForTransparentRendering35 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.useJitteredProjectionMatrixForTransparentRendering = useJitteredProjectionMatrixForTransparentRendering35;
                    break;
                case 51:
                    if (opt.uid == ClientManager.UID)
                        return;
                    stereoSeparation36 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.stereoSeparation = stereoSeparation36;
                    break;
                case 52:
                    if (opt.uid == ClientManager.UID)
                        return;
                    stereoConvergence37 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.stereoConvergence = stereoConvergence37;
                    break;
                case 54:
                    if (opt.uid == ClientManager.UID)
                        return;
                    stereoTargetEye38 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.StereoTargetEyeMask>(new Net.System.Segment(opt.buffer, false));
                    self.stereoTargetEye = stereoTargetEye38;
                    break;
                case 63:
                    if (opt.uid == ClientManager.UID)
                        return;
                    enabled39 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.enabled = enabled39;
                    break;
                case 67:
                    if (opt.uid == ClientManager.UID)
                        return;
                    tag40 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.tag = tag40;
                    break;
                case 81:
                    if (opt.uid == ClientManager.UID)
                        return;
                    name41 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.name = name41;
                    break;
                case 82:
                    if (opt.uid == ClientManager.UID)
                        return;
                    hideFlags42 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.HideFlags>(new Net.System.Segment(opt.buffer, false));
                    self.hideFlags = hideFlags42;
                    break;
                case 156:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetGateFittedLensShift();
                    }
                    break;
                case 184:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var clipPlane = (UnityEngine.Vector4)data.pars[0];
                        self.CalculateObliqueMatrix(clipPlane);
                    }
                    break;
                case 185:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                        self.WorldToScreenPoint(position, eye);
                    }
                    break;
                case 186:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                        self.WorldToViewportPoint(position, eye);
                    }
                    break;
                case 187:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                        self.ViewportToWorldPoint(position, eye);
                    }
                    break;
                case 188:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                        self.ScreenToWorldPoint(position, eye);
                    }
                    break;
                case 189:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        self.WorldToScreenPoint(position);
                    }
                    break;
                case 190:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        self.WorldToViewportPoint(position);
                    }
                    break;
                case 191:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        self.ViewportToWorldPoint(position);
                    }
                    break;
                case 192:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        self.ScreenToWorldPoint(position);
                    }
                    break;
                case 193:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        self.ScreenToViewportPoint(position);
                    }
                    break;
                case 194:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var position = (UnityEngine.Vector3)data.pars[0];
                        self.ViewportToScreenPoint(position);
                    }
                    break;
                case 195:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var pos = (UnityEngine.Vector3)data.pars[0];
                        var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                        self.ViewportPointToRay(pos, eye);
                    }
                    break;
                case 196:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var pos = (UnityEngine.Vector3)data.pars[0];
                        self.ViewportPointToRay(pos);
                    }
                    break;
                case 197:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var pos = (UnityEngine.Vector3)data.pars[0];
                        var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                        self.ScreenPointToRay(pos, eye);
                    }
                    break;
                case 198:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var pos = (UnityEngine.Vector3)data.pars[0];
                        self.ScreenPointToRay(pos);
                    }
                    break;
                case 211:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var eye = (UnityEngine.Camera.StereoscopicEye)data.pars[0];
                        self.GetStereoNonJitteredProjectionMatrix(eye);
                    }
                    break;
                case 212:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var eye = (UnityEngine.Camera.StereoscopicEye)data.pars[0];
                        self.GetStereoViewMatrix(eye);
                    }
                    break;
                case 214:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var eye = (UnityEngine.Camera.StereoscopicEye)data.pars[0];
                        self.GetStereoProjectionMatrix(eye);
                    }
                    break;
                case 290:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var tag = data.pars[0] as System.String;
                        self.CompareTag(tag);
                    }
                    break;
                case 293:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessageUpwards(methodName);
                    }
                    break;
                case 294:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessageUpwards(methodName, options);
                    }
                    break;
                case 296:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessage(methodName);
                    }
                    break;
                case 298:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessage(methodName, options);
                    }
                    break;
                case 301:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.BroadcastMessage(methodName);
                    }
                    break;
                case 302:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.BroadcastMessage(methodName, options);
                    }
                    break;
                case 316:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetInstanceID();
                    }
                    break;
                case 317:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetHashCode();
                    }
                    break;
                case 323:
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