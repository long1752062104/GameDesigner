#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// BoxCollider同步组件, 此代码由BuildComponentTools工具生成
    /// </summary>
    [RequireComponent(typeof(UnityEngine.BoxCollider))]
    public class NetworkBoxCollider : NetworkBehaviour
    {

       private UnityEngine.BoxCollider self;
       public bool autoCheck;
       private UnityEngine.Vector3 center1;
       private UnityEngine.Vector3 size2;
       private System.Boolean enabled3;
       private System.Boolean isTrigger4;
       private System.Single contactOffset5;
       private System.String tag6;
       private System.String name7;
       private UnityEngine.HideFlags hideFlags8;
       private UnityEngine.Vector3 position1;
       private UnityEngine.Vector3 position2;
       private System.String tag3;
       private System.String methodName4;
       private System.String methodName5;
       private UnityEngine.SendMessageOptions options6;
       private System.String methodName7;
       private System.String methodName8;
       private UnityEngine.SendMessageOptions options9;
       private System.String methodName10;
       private System.String methodName11;
       private UnityEngine.SendMessageOptions options12;

       public override void Awake()
      {
          base.Awake();
          self = GetComponent<UnityEngine.BoxCollider>();
      }

      public UnityEngine.Vector3 center
      {
          get
          {
              return self.center;
          }
          set
          {
               if (center1 == value)
                    return;
               center1 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 0,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

      public UnityEngine.Vector3 size
      {
          get
          {
              return self.size;
          }
          set
          {
               if (size2 == value)
                    return;
               size2 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 1,
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
               if (enabled3 == value)
                    return;
               enabled3 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 3,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

      public System.Boolean isTrigger
      {
          get
          {
              return self.isTrigger;
          }
          set
          {
               if (isTrigger4 == value)
                    return;
               isTrigger4 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 5,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

      public System.Single contactOffset
      {
          get
          {
              return self.contactOffset;
          }
          set
          {
               if (contactOffset5 == value)
                    return;
               contactOffset5 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
                {
                    index = netObj.registerObjectIndex,
                    index1 = 6,
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
                    index1 = 12,
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
                    index1 = 26,
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
                    index1 = 27,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
          }
     }

public void ClosestPoint(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position1 & !always) return;
     position1 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 41,
        buffer = buffer
    });
}
public void ClosestPointOnBounds(UnityEngine.Vector3 position, bool always = false)
{
     if(position == position2 & !always) return;
     position2 = position;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { position } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 48,
        buffer = buffer
    });
}
public void CompareTag(System.String tag, bool always = false)
{
     if(tag == tag3 & !always) return;
     tag3 = tag;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { tag } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 79,
        buffer = buffer
    });
}
public void SendMessageUpwards(System.String methodName, bool always = false)
{
     if(methodName == methodName4 & !always) return;
     methodName4 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 82,
        buffer = buffer
    });
}
public void SendMessageUpwards(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName5 & options == options6 & !always) return;
     methodName5 = methodName;
     options6 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 83,
        buffer = buffer
    });
}
public void SendMessage(System.String methodName, bool always = false)
{
     if(methodName == methodName7 & !always) return;
     methodName7 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 85,
        buffer = buffer
    });
}
public void SendMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName8 & options == options9 & !always) return;
     methodName8 = methodName;
     options9 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 87,
        buffer = buffer
    });
}
public void BroadcastMessage(System.String methodName, bool always = false)
{
     if(methodName == methodName10 & !always) return;
     methodName10 = methodName;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 90,
        buffer = buffer
    });
}
public void BroadcastMessage(System.String methodName,UnityEngine.SendMessageOptions options, bool always = false)
{
     if(methodName == methodName11 & options == options12 & !always) return;
     methodName11 = methodName;
     options12 = options;
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName,options } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 91,
        buffer = buffer
    });
}
public void GetInstanceID( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 105,
        buffer = buffer
    });
}
public void GetHashCode( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 106,
        buffer = buffer
    });
}
public void ToString( bool always = false)
{
   var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] {  } });
    ClientManager.AddOperation(new Operation(Command.BuildComponent, netObj.m_identity)
    {
       index = netObj.registerObjectIndex,
       index1 = 112,
        buffer = buffer
    });
}
     public override void OnPropertyAutoCheck()
     {
     if (!autoCheck)
         return;
                center = center;
                size = size;
                enabled = enabled;
                isTrigger = isTrigger;
                contactOffset = contactOffset;
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
                 center1 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector3>(new Net.System.Segment(opt.buffer, false));
                 self.center = center1;
                break;
             case 1:
                 size2 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Vector3>(new Net.System.Segment(opt.buffer, false));
                 self.size = size2;
                break;
             case 3:
                 enabled3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.enabled = enabled3;
                break;
             case 5:
                 isTrigger4 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                 self.isTrigger = isTrigger4;
                break;
             case 6:
                 contactOffset5 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                 self.contactOffset = contactOffset5;
                break;
             case 12:
                 tag6 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                 self.tag = tag6;
                break;
             case 26:
                 name7 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                 self.name = name7;
                break;
             case 27:
                 hideFlags8 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.HideFlags>(new Net.System.Segment(opt.buffer, false));
                 self.hideFlags = hideFlags8;
                break;
             case 41:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var position = (UnityEngine.Vector3)data.pars[0];
                   self.ClosestPoint(position);
              }
                break;
             case 48:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var position = (UnityEngine.Vector3)data.pars[0];
                   self.ClosestPointOnBounds(position);
              }
                break;
             case 79:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var tag = data.pars[0] as System.String;
                   self.CompareTag(tag);
              }
                break;
             case 82:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
                   self.SendMessageUpwards(methodName);
              }
                break;
             case 83:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
           var options = (UnityEngine.SendMessageOptions)data.pars[1];
                   self.SendMessageUpwards(methodName,options);
              }
                break;
             case 85:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
                   self.SendMessage(methodName);
              }
                break;
             case 87:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
           var options = (UnityEngine.SendMessageOptions)data.pars[1];
                   self.SendMessage(methodName,options);
              }
                break;
             case 90:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
                   self.BroadcastMessage(methodName);
              }
                break;
             case 91:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
           var methodName = data.pars[0] as System.String;
           var options = (UnityEngine.SendMessageOptions)data.pars[1];
                   self.BroadcastMessage(methodName,options);
              }
                break;
             case 105:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.GetInstanceID();
              }
                break;
             case 106:
              {
                    var segment = new Net.System.Segment(opt.buffer, false);
                   var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                   self.GetHashCode();
              }
                break;
             case 112:
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
