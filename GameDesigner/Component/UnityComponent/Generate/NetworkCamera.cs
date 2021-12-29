#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// Camera同步组件, 此代码由BuildComponentTools工具生成
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
       private UnityEngine.Rect pixelRect32;
       private System.Int32 targetDisplay33;
       private System.Boolean useJitteredProjectionMatrixForTransparentRendering34;
       private System.Single stereoSeparation35;
       private System.Single stereoConvergence36;
       private UnityEngine.StereoTargetEyeMask stereoTargetEye37;
       private System.Boolean enabled38;
       private System.String tag39;
       private System.String name40;
       private UnityEngine.HideFlags hideFlags41;
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 0,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 1,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 2,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 3,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 5,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 6,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 7,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 8,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 9,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 10,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 11,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 12,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 13,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 14,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 15,
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
               if (cullingMask16 == value)
                    return;
               cullingMask16 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 17,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 18,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 19,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 20,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 21,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 23,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 25,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 26,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 27,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 28,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 29,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 30,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 31,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 32,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 33,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 34,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

      public UnityEngine.Rect pixelRect
      {
          get
          {
              return self.pixelRect;
          }
          set
          {
               if (pixelRect32 == value)
                    return;
               pixelRect32 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 35,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
               if (targetDisplay33 == value)
                    return;
               targetDisplay33 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 42,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
               if (useJitteredProjectionMatrixForTransparentRendering34 == value)
                    return;
               useJitteredProjectionMatrixForTransparentRendering34 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 47,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
               if (stereoSeparation35 == value)
                    return;
               stereoSeparation35 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 51,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
               if (stereoConvergence36 == value)
                    return;
               stereoConvergence36 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 52,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
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
               if (stereoTargetEye37 == value)
                    return;
               stereoTargetEye37 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 54,
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
               if (enabled38 == value)
                    return;
               enabled38 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 63,
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
               if (tag39 == value)
                    return;
               tag39 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 67,
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
               if (name40 == value)
                    return;
               name40 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 81,
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
               if (hideFlags41 == value)
                    return;
               hideFlags41 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 82,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

public void GetGateFittedLensShift( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 156,
        buffer = buffer
    });
}
public void CalculateObliqueMatrix(UnityEngine.Vector4 clipPlane, bool always = false)
{
     if(clipPlane == clipPlane1 & !always) return;
     clipPlane1 = clipPlane;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { clipPlane } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 184,
        buffer = buffer
    });
}
public void WorldToScreenPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
{
     if(position == position2 & eye == eye3 & !always) return;
     position2 = position;
     eye3 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position,eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 185,
        buffer = buffer
    });
}
public void WorldToViewportPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
{
     if(position == position4 & eye == eye5 & !always) return;
     position4 = position;
     eye5 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position,eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 186,
        buffer = buffer
    });
}
public void ViewportToWorldPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
{
     if(position == position6 & eye == eye7 & !always) return;
     position6 = position;
     eye7 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position,eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 187,
        buffer = buffer
    });
}
public void ScreenToWorldPoint(UnityEngine.Vector3 position,UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
{
     if(position == position8 & eye == eye9 & !always) return;
     position8 = position;
     eye9 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position,eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 188,
        buffer = buffer
    });
}
public void WorldToScreenPoint(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position10 & !always) return;
     position10 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 189,
        buffer = buffer
    });
}
public void WorldToViewportPoint(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position11 & !always) return;
     position11 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 190,
        buffer = buffer
    });
}
public void ViewportToWorldPoint(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position12 & !always) return;
     position12 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 191,
        buffer = buffer
    });
}
public void ScreenToWorldPoint(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position13 & !always) return;
     position13 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 192,
        buffer = buffer
    });
}
public void ScreenToViewportPoint(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position14 & !always) return;
     position14 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 193,
        buffer = buffer
    });
}
public void ViewportToScreenPoint(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position15 & !always) return;
     position15 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 194,
        buffer = buffer
    });
}
public void ViewportPointToRay(UnityEngine.Vector3 pos,UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
{
     if(pos == pos16 & eye == eye17 & !always) return;
     pos16 = pos;
     eye17 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos,eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 195,
        buffer = buffer
    });
}
public void ViewportPointToRay(UnityEngine.Vector3 pos, bool always = false)
{
     if(pos == pos18 & !always) return;
     pos18 = pos;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 196,
        buffer = buffer
    });
}
public void ScreenPointToRay(UnityEngine.Vector3 pos,UnityEngine.Camera.MonoOrStereoscopicEye eye, bool always = false)
{
     if(pos == pos19 & eye == eye20 & !always) return;
     pos19 = pos;
     eye20 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos,eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 197,
        buffer = buffer
    });
}
public void ScreenPointToRay(UnityEngine.Vector3 pos, bool always = false)
{
     if(pos == pos21 & !always) return;
     pos21 = pos;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { pos } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 198,
        buffer = buffer
    });
}
public void GetStereoNonJitteredProjectionMatrix(UnityEngine.Camera.StereoscopicEye eye, bool always = false)
{
     if(eye == eye22 & !always) return;
     eye22 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 211,
        buffer = buffer
    });
}
public void GetStereoViewMatrix(UnityEngine.Camera.StereoscopicEye eye, bool always = false)
{
     if(eye == eye23 & !always) return;
     eye23 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 212,
        buffer = buffer
    });
}
public void GetStereoProjectionMatrix(UnityEngine.Camera.StereoscopicEye eye, bool always = false)
{
     if(eye == eye24 & !always) return;
     eye24 = eye;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { eye } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 214,
        buffer = buffer
    });
}
public void CompareTag(System.String tag, bool always = false)
{
     if(tag == tag25 & !always) return;
     tag25 = tag;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { tag } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 290,
        buffer = buffer
    });
}
public void SendMessageUpwards(System.String methodName, bool always = false)
{
     if(methodName == methodName26 & !always) return;
     methodName26 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 293,
        buffer = buffer
    });
}
public void SendMessageUpwards(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName27 & options == options28 & !always) return;
     methodName27 = methodName;
     options28 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 294,
        buffer = buffer
    });
}
public void SendMessage(System.String methodName, bool always = false)
{
     if(methodName == methodName29 & !always) return;
     methodName29 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 296,
        buffer = buffer
    });
}
public void SendMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName30 & options == options31 & !always) return;
     methodName30 = methodName;
     options31 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 298,
        buffer = buffer
    });
}
public void BroadcastMessage(System.String methodName, bool always = false)
{
     if(methodName == methodName32 & !always) return;
     methodName32 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 301,
        buffer = buffer
    });
}
public void BroadcastMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName33 & options == options34 & !always) return;
     methodName33 = methodName;
     options34 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 302,
        buffer = buffer
    });
}
public void GetInstanceID( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 316,
        buffer = buffer
    });
}
public void GetHashCode( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
       index1 = 317,
        buffer = buffer
    });
}
public void ToString( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
    {
       index = networkIdentity.registerObjectIndex,
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
                pixelRect = pixelRect;
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
                 nearClipPlane1 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.nearClipPlane = nearClipPlane1;
                break;
             case 1:
                 farClipPlane2 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.farClipPlane = farClipPlane2;
                break;
             case 2:
                 fieldOfView3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.fieldOfView = fieldOfView3;
                break;
             case 3:
                 renderingPath4 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.RenderingPath>(new Net.System.Segment(opt.buffer, false));
                 self.renderingPath = renderingPath4;
                break;
             case 5:
                 allowHDR5 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.allowHDR = allowHDR5;
                break;
             case 6:
                 allowMSAA6 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.allowMSAA = allowMSAA6;
                break;
             case 7:
                 allowDynamicResolution7 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.allowDynamicResolution = allowDynamicResolution7;
                break;
             case 8:
                 forceIntoRenderTexture8 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.forceIntoRenderTexture = forceIntoRenderTexture8;
                break;
             case 9:
                 orthographicSize9 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.orthographicSize = orthographicSize9;
                break;
             case 10:
                 orthographic10 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.orthographic = orthographic10;
                break;
             case 11:
                 opaqueSortMode11 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Rendering.OpaqueSortMode>(new Net.System.Segment(opt.buffer, false));
                 self.opaqueSortMode = opaqueSortMode11;
                break;
             case 12:
                 transparencySortMode12 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.TransparencySortMode>(new Net.System.Segment(opt.buffer, false));
                 self.transparencySortMode = transparencySortMode12;
                break;
             case 13:
                 transparencySortAxis13 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector3>(new Net.System.Segment(opt.buffer, false));
                 self.transparencySortAxis = transparencySortAxis13;
                break;
             case 14:
                 depth14 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.depth = depth14;
                break;
             case 15:
                 aspect15 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.aspect = aspect15;
                break;
             case 17:
                 cullingMask16 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                 self.cullingMask = cullingMask16;
                break;
             case 18:
                 eventMask17 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                 self.eventMask = eventMask17;
                break;
             case 19:
                 layerCullSpherical18 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.layerCullSpherical = layerCullSpherical18;
                break;
             case 20:
                 cameraType19 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.CameraType>(new Net.System.Segment(opt.buffer, false));
                 self.cameraType = cameraType19;
                break;
             case 21:
                 overrideSceneCullingMask20 = Net.Serialize.NetConvertFast2.DeserializeObject<System.UInt64>(new Net.System.Segment(opt.buffer, false));
                 self.overrideSceneCullingMask = overrideSceneCullingMask20;
                break;
             case 23:
                 useOcclusionCulling21 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.useOcclusionCulling = useOcclusionCulling21;
                break;
             case 25:
                 backgroundColor22 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Color>(new Net.System.Segment(opt.buffer, false));
                 self.backgroundColor = backgroundColor22;
                break;
             case 26:
                 clearFlags23 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.CameraClearFlags>(new Net.System.Segment(opt.buffer, false));
                 self.clearFlags = clearFlags23;
                break;
             case 27:
                 depthTextureMode24 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.DepthTextureMode>(new Net.System.Segment(opt.buffer, false));
                 self.depthTextureMode = depthTextureMode24;
                break;
             case 28:
                 clearStencilAfterLightingPass25 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.clearStencilAfterLightingPass = clearStencilAfterLightingPass25;
                break;
             case 29:
                 usePhysicalProperties26 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.usePhysicalProperties = usePhysicalProperties26;
                break;
             case 30:
                 sensorSize27 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector2>(new Net.System.Segment(opt.buffer, false));
                 self.sensorSize = sensorSize27;
                break;
             case 31:
                 lensShift28 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector2>(new Net.System.Segment(opt.buffer, false));
                 self.lensShift = lensShift28;
                break;
             case 32:
                 focalLength29 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.focalLength = focalLength29;
                break;
             case 33:
                 gateFit30 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Camera.GateFitMode>(new Net.System.Segment(opt.buffer, false));
                 self.gateFit = gateFit30;
                break;
             case 34:
                 rect31 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Rect>(new Net.System.Segment(opt.buffer, false));
                 self.rect = rect31;
                break;
             case 35:
                 pixelRect32 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Rect>(new Net.System.Segment(opt.buffer, false));
                 self.pixelRect = pixelRect32;
                break;
             case 42:
                 targetDisplay33 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                 self.targetDisplay = targetDisplay33;
                break;
             case 47:
                 useJitteredProjectionMatrixForTransparentRendering34 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.useJitteredProjectionMatrixForTransparentRendering = useJitteredProjectionMatrixForTransparentRendering34;
                break;
             case 51:
                 stereoSeparation35 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.stereoSeparation = stereoSeparation35;
                break;
             case 52:
                 stereoConvergence36 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.stereoConvergence = stereoConvergence36;
                break;
             case 54:
                 stereoTargetEye37 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.StereoTargetEyeMask>(new Net.System.Segment(opt.buffer, false));
                 self.stereoTargetEye = stereoTargetEye37;
                break;
             case 63:
                 enabled38 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.enabled = enabled38;
                break;
             case 67:
                 tag39 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                 self.tag = tag39;
                break;
             case 81:
                 name40 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                 self.name = name40;
                break;
             case 82:
                 hideFlags41 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.HideFlags>(new Net.System.Segment(opt.buffer, false));
                 self.hideFlags = hideFlags41;
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
                   self.WorldToScreenPoint(position,eye);
              }
                break;
             case 186:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var position = (UnityEngine.Vector3)data.pars[0];
           var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                   self.WorldToViewportPoint(position,eye);
              }
                break;
             case 187:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var position = (UnityEngine.Vector3)data.pars[0];
           var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                   self.ViewportToWorldPoint(position,eye);
              }
                break;
             case 188:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var position = (UnityEngine.Vector3)data.pars[0];
           var eye = (UnityEngine.Camera.MonoOrStereoscopicEye)data.pars[1];
                   self.ScreenToWorldPoint(position,eye);
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
                   self.ViewportPointToRay(pos,eye);
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
                   self.ScreenPointToRay(pos,eye);
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
                   self.SendMessageUpwards(methodName,options);
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
                   self.SendMessage(methodName,options);
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
                   self.BroadcastMessage(methodName,options);
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
