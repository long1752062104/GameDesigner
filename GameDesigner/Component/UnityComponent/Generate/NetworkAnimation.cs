#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// Animation同步组件, 此代码由BuildComponentTools工具生成
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Animation))]
    public class NetworkAnimation : NetworkBehaviour
    {

       private UnityEngine.Animation self;
       public bool autoCheck;
       private System.Boolean playAutomatically1;
       private UnityEngine.WrapMode wrapMode2;
       private System.Boolean animatePhysics3;
       private UnityEngine.AnimationCullingType cullingType4;
       private System.Boolean enabled5;
       private System.String tag6;
       private System.String name7;
       private UnityEngine.HideFlags hideFlags8;
       private System.String name1;
       private System.String name2;
       private UnityEngine.PlayMode mode3;
       private System.String animation4;
       private System.String animation5;
       private System.Single fadeLength6;
       private System.String animation7;
       private System.String animation8;
       private System.Single targetWeight9;
       private System.String animation10;
       private System.String animation11;
       private System.Single fadeLength12;
       private UnityEngine.QueueMode queue13;
       private System.String animation14;
       private System.Single fadeLength15;
       private System.String animation16;
       private System.String animation17;
       private UnityEngine.QueueMode queue18;
       private System.String animation19;
       private System.String clipName20;
       private System.Int32 layer21;
       private System.String name22;
       private System.String tag23;
       private System.String methodName24;
       private System.String methodName25;
       private UnityEngine.SendMessageOptions options26;
       private System.String methodName27;
       private System.String methodName28;
       private UnityEngine.SendMessageOptions options29;
       private System.String methodName30;
       private System.String methodName31;
       private UnityEngine.SendMessageOptions options32;

       public override void Awake()
      {
          base.Awake();
          self = GetComponent<UnityEngine.Animation>();
      }

      public System.Boolean playAutomatically
      {
          get
          {
              return self.playAutomatically;
          }
          set
          {
               if (playAutomatically1 == value)
                    return;
               playAutomatically1 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 1,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

      public UnityEngine.WrapMode wrapMode
      {
          get
          {
              return self.wrapMode;
          }
          set
          {
               if (wrapMode2 == value)
                    return;
               wrapMode2 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 2,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

      public System.Boolean animatePhysics
      {
          get
          {
              return self.animatePhysics;
          }
          set
          {
               if (animatePhysics3 == value)
                    return;
               animatePhysics3 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 5,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

      public UnityEngine.AnimationCullingType cullingType
      {
          get
          {
              return self.cullingType;
          }
          set
          {
               if (cullingType4 == value)
                    return;
               cullingType4 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 7,
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
               if (enabled5 == value)
                    return;
               enabled5 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 9,
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
               if (tag6 == value)
                    return;
               tag6 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 13,
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
               if (name7 == value)
                    return;
               name7 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 27,
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
               if (hideFlags8 == value)
                    return;
               hideFlags8 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 28,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

public void Stop( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 35,
        buffer = buffer
    });
}
public void Stop(System.String name, bool always = false)
{
     if(name == name1 & !always) return;
     name1 = name;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 36,
        buffer = buffer
    });
}
public void Rewind(System.String name, bool always = false)
{
     if(name == name2 & !always) return;
     name2 = name;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 37,
        buffer = buffer
    });
}
public void Rewind( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 38,
        buffer = buffer
    });
}
public void Sample( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 39,
        buffer = buffer
    });
}
public void Play( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 43,
        buffer = buffer
    });
}
public void Play(UnityEngine.PlayMode mode, bool always = false)
{
     if(mode == mode3 & !always) return;
     mode3 = mode;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { mode } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 44,
        buffer = buffer
    });
}
public void Play(System.String animation, bool always = false)
{
     if(animation == animation4 & !always) return;
     animation4 = animation;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 46,
        buffer = buffer
    });
}
public void CrossFade(System.String animation,System.Single fadeLength, bool always = false)
{
     if(animation == animation5 & fadeLength == fadeLength6 & !always) return;
     animation5 = animation;
     fadeLength6 = fadeLength;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation,fadeLength } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 48,
        buffer = buffer
    });
}
public void CrossFade(System.String animation, bool always = false)
{
     if(animation == animation7 & !always) return;
     animation7 = animation;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 49,
        buffer = buffer
    });
}
public void Blend(System.String animation,System.Single targetWeight, bool always = false)
{
     if(animation == animation8 & targetWeight == targetWeight9 & !always) return;
     animation8 = animation;
     targetWeight9 = targetWeight;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation,targetWeight } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 51,
        buffer = buffer
    });
}
public void Blend(System.String animation, bool always = false)
{
     if(animation == animation10 & !always) return;
     animation10 = animation;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 52,
        buffer = buffer
    });
}
public void CrossFadeQueued(System.String animation,System.Single fadeLength,UnityEngine.QueueMode queue, bool always = false)
{
     if(animation == animation11 & fadeLength == fadeLength12 & queue == queue13 & !always) return;
     animation11 = animation;
     fadeLength12 = fadeLength;
     queue13 = queue;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation,fadeLength,queue } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 54,
        buffer = buffer
    });
}
public void CrossFadeQueued(System.String animation,System.Single fadeLength, bool always = false)
{
     if(animation == animation14 & fadeLength == fadeLength15 & !always) return;
     animation14 = animation;
     fadeLength15 = fadeLength;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation,fadeLength } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 55,
        buffer = buffer
    });
}
public void CrossFadeQueued(System.String animation, bool always = false)
{
     if(animation == animation16 & !always) return;
     animation16 = animation;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 56,
        buffer = buffer
    });
}
public void PlayQueued(System.String animation,UnityEngine.QueueMode queue, bool always = false)
{
     if(animation == animation17 & queue == queue18 & !always) return;
     animation17 = animation;
     queue18 = queue;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation,queue } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 58,
        buffer = buffer
    });
}
public void PlayQueued(System.String animation, bool always = false)
{
     if(animation == animation19 & !always) return;
     animation19 = animation;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { animation } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 59,
        buffer = buffer
    });
}
public void RemoveClip(System.String clipName, bool always = false)
{
     if(clipName == clipName20 & !always) return;
     clipName20 = clipName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { clipName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 64,
        buffer = buffer
    });
}
public void SyncLayer(System.Int32 layer, bool always = false)
{
     if(layer == layer21 & !always) return;
     layer21 = layer;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { layer } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 68,
        buffer = buffer
    });
}
public void GetEnumerator( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 69,
        buffer = buffer
    });
}
public void GetClip(System.String name, bool always = false)
{
     if(name == name22 & !always) return;
     name22 = name;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { name } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 70,
        buffer = buffer
    });
}
public void CompareTag(System.String tag, bool always = false)
{
     if(tag == tag23 & !always) return;
     tag23 = tag;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { tag } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 112,
        buffer = buffer
    });
}
public void SendMessageUpwards(System.String methodName, bool always = false)
{
     if(methodName == methodName24 & !always) return;
     methodName24 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 115,
        buffer = buffer
    });
}
public void SendMessageUpwards(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName25 & options == options26 & !always) return;
     methodName25 = methodName;
     options26 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 116,
        buffer = buffer
    });
}
public void SendMessage(System.String methodName, bool always = false)
{
     if(methodName == methodName27 & !always) return;
     methodName27 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 118,
        buffer = buffer
    });
}
public void SendMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName28 & options == options29 & !always) return;
     methodName28 = methodName;
     options29 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 120,
        buffer = buffer
    });
}
public void BroadcastMessage(System.String methodName, bool always = false)
{
     if(methodName == methodName30 & !always) return;
     methodName30 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 123,
        buffer = buffer
    });
}
public void BroadcastMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName31 & options == options32 & !always) return;
     methodName31 = methodName;
     options32 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 124,
        buffer = buffer
    });
}
public void GetInstanceID( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 138,
        buffer = buffer
    });
}
public void GetHashCode( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 139,
        buffer = buffer
    });
}
public void ToString( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 145,
        buffer = buffer
    });
}
     public override void OnPropertyAutoCheck()
     {
     if (!autoCheck)
         return;
                playAutomatically = playAutomatically;
                wrapMode = wrapMode;
                animatePhysics = animatePhysics;
                cullingType = cullingType;
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
             case 1:
                 playAutomatically1 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.playAutomatically = playAutomatically1;
                break;
             case 2:
                 wrapMode2 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.WrapMode>(new Net.System.Segment(opt.buffer, false));
                 self.wrapMode = wrapMode2;
                break;
             case 5:
                 animatePhysics3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.animatePhysics = animatePhysics3;
                break;
             case 7:
                 cullingType4 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.AnimationCullingType>(new Net.System.Segment(opt.buffer, false));
                 self.cullingType = cullingType4;
                break;
             case 9:
                 enabled5 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.enabled = enabled5;
                break;
             case 13:
                 tag6 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                 self.tag = tag6;
                break;
             case 27:
                 name7 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                 self.name = name7;
                break;
             case 28:
                 hideFlags8 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.HideFlags>(new Net.System.Segment(opt.buffer, false));
                 self.hideFlags = hideFlags8;
                break;
             case 35:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.Stop();
              }
                break;
             case 36:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var name = data.pars[0] as System.String;
                   self.Stop(name);
              }
                break;
             case 37:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var name = data.pars[0] as System.String;
                   self.Rewind(name);
              }
                break;
             case 38:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.Rewind();
              }
                break;
             case 39:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.Sample();
              }
                break;
             case 43:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.Play();
              }
                break;
             case 44:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var mode = (UnityEngine.PlayMode)data.pars[0];
                   self.Play(mode);
              }
                break;
             case 46:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
                   self.Play(animation);
              }
                break;
             case 48:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
           var fadeLength = (System.Single)data.pars[1];
                   self.CrossFade(animation,fadeLength);
              }
                break;
             case 49:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
                   self.CrossFade(animation);
              }
                break;
             case 51:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
           var targetWeight = (System.Single)data.pars[1];
                   self.Blend(animation,targetWeight);
              }
                break;
             case 52:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
                   self.Blend(animation);
              }
                break;
             case 54:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
           var fadeLength = (System.Single)data.pars[1];
           var queue = (UnityEngine.QueueMode)data.pars[2];
                   self.CrossFadeQueued(animation,fadeLength,queue);
              }
                break;
             case 55:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
           var fadeLength = (System.Single)data.pars[1];
                   self.CrossFadeQueued(animation,fadeLength);
              }
                break;
             case 56:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
                   self.CrossFadeQueued(animation);
              }
                break;
             case 58:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
           var queue = (UnityEngine.QueueMode)data.pars[1];
                   self.PlayQueued(animation,queue);
              }
                break;
             case 59:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var animation = data.pars[0] as System.String;
                   self.PlayQueued(animation);
              }
                break;
             case 64:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var clipName = data.pars[0] as System.String;
                   self.RemoveClip(clipName);
              }
                break;
             case 68:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var layer = (System.Int32)data.pars[0];
                   self.SyncLayer(layer);
              }
                break;
             case 69:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.GetEnumerator();
              }
                break;
             case 70:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var name = data.pars[0] as System.String;
                   self.GetClip(name);
              }
                break;
             case 112:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var tag = data.pars[0] as System.String;
                   self.CompareTag(tag);
              }
                break;
             case 115:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
                   self.SendMessageUpwards(methodName);
              }
                break;
             case 116:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
           var options = (UnityEngine.SendMessageOptions)data.pars[1];
                   self.SendMessageUpwards(methodName,options);
              }
                break;
             case 118:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
                   self.SendMessage(methodName);
              }
                break;
             case 120:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
           var options = (UnityEngine.SendMessageOptions)data.pars[1];
                   self.SendMessage(methodName,options);
              }
                break;
             case 123:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
                   self.BroadcastMessage(methodName);
              }
                break;
             case 124:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
           var options = (UnityEngine.SendMessageOptions)data.pars[1];
                   self.BroadcastMessage(methodName,options);
              }
                break;
             case 138:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.GetInstanceID();
              }
                break;
             case 139:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.GetHashCode();
              }
                break;
             case 145:
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
