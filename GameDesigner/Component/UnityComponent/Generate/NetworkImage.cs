#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using Net.Share;
using Net.UnityComponent;
using UnityEngine;

namespace BuildComponent
{
    /// <summary>
    /// Image同步组件, 此代码由BuildComponentTools工具生成
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class NetworkImage : NetworkBehaviour
    {

        private UnityEngine.UI.Image self;
        public bool autoCheck;
        private UnityEngine.UI.Image.Type type1;
        private System.Boolean preserveAspect2;
        private System.Boolean fillCenter3;
        private UnityEngine.UI.Image.FillMethod fillMethod4;
        private System.Single fillAmount5;
        private System.Boolean fillClockwise6;
        private System.Int32 fillOrigin7;
        private System.Single alphaHitTestMinimumThreshold8;
        private System.Boolean useSpriteMesh9;
        private System.Single pixelsPerUnitMultiplier10;
        private System.Boolean maskable11;
        private System.Boolean isMaskingGraphic12;
        private UnityEngine.Color color13;
        private System.Boolean raycastTarget14;
        private System.Boolean useGUILayout15;
        private System.Boolean runInEditMode16;
        private System.Boolean enabled17;
        private System.String tag18;
        private System.String name19;
        private UnityEngine.HideFlags hideFlags20;
        private UnityEngine.Rect clipRect1;
        private System.Boolean validRect2;
        private UnityEngine.Rect clipRect3;
        private System.Boolean validRect4;
        private UnityEngine.Vector2 clipSoftness5;
        private UnityEngine.UI.CanvasUpdate update6;
        private UnityEngine.Vector2 point7;
        private UnityEngine.Color targetColor8;
        private System.Single duration9;
        private System.Boolean ignoreTimeScale10;
        private System.Boolean useAlpha11;
        private UnityEngine.Color targetColor12;
        private System.Single duration13;
        private System.Boolean ignoreTimeScale14;
        private System.Boolean useAlpha15;
        private System.Boolean useRGB16;
        private System.Single alpha17;
        private System.Single duration18;
        private System.Boolean ignoreTimeScale19;
        private System.String methodName20;
        private System.Single time21;
        private System.String methodName22;
        private System.Single time23;
        private System.Single repeatRate24;
        private System.String methodName25;
        private System.String methodName26;
        private System.String methodName27;
        private System.String tag28;
        private System.String methodName29;
        private System.String methodName30;
        private UnityEngine.SendMessageOptions options31;
        private System.String methodName32;
        private System.String methodName33;
        private UnityEngine.SendMessageOptions options34;
        private System.String methodName35;
        private System.String methodName36;
        private UnityEngine.SendMessageOptions options37;

        public override void Awake()
        {
            base.Awake();
            self = GetComponent<UnityEngine.UI.Image>();
        }

        public UnityEngine.UI.Image.Type type
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
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 2,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean preserveAspect
        {
            get
            {
                return self.preserveAspect;
            }
            set
            {
                if (preserveAspect2 == value)
                    return;
                preserveAspect2 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 3,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean fillCenter
        {
            get
            {
                return self.fillCenter;
            }
            set
            {
                if (fillCenter3 == value)
                    return;
                fillCenter3 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 4,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public UnityEngine.UI.Image.FillMethod fillMethod
        {
            get
            {
                return self.fillMethod;
            }
            set
            {
                if (fillMethod4 == value)
                    return;
                fillMethod4 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 5,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single fillAmount
        {
            get
            {
                return self.fillAmount;
            }
            set
            {
                if (fillAmount5 == value)
                    return;
                fillAmount5 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 6,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean fillClockwise
        {
            get
            {
                return self.fillClockwise;
            }
            set
            {
                if (fillClockwise6 == value)
                    return;
                fillClockwise6 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 7,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Int32 fillOrigin
        {
            get
            {
                return self.fillOrigin;
            }
            set
            {
                if (fillOrigin7 == value)
                    return;
                fillOrigin7 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 8,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single alphaHitTestMinimumThreshold
        {
            get
            {
                return self.alphaHitTestMinimumThreshold;
            }
            set
            {
                if (alphaHitTestMinimumThreshold8 == value)
                    return;
                alphaHitTestMinimumThreshold8 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 10,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean useSpriteMesh
        {
            get
            {
                return self.useSpriteMesh;
            }
            set
            {
                if (useSpriteMesh9 == value)
                    return;
                useSpriteMesh9 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 11,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Single pixelsPerUnitMultiplier
        {
            get
            {
                return self.pixelsPerUnitMultiplier;
            }
            set
            {
                if (pixelsPerUnitMultiplier10 == value)
                    return;
                pixelsPerUnitMultiplier10 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 14,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean maskable
        {
            get
            {
                return self.maskable;
            }
            set
            {
                if (maskable11 == value)
                    return;
                maskable11 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 25,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean isMaskingGraphic
        {
            get
            {
                return self.isMaskingGraphic;
            }
            set
            {
                if (isMaskingGraphic12 == value)
                    return;
                isMaskingGraphic12 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 26,
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
                if (color13 == value)
                    return;
                color13 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 27,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean raycastTarget
        {
            get
            {
                return self.raycastTarget;
            }
            set
            {
                if (raycastTarget14 == value)
                    return;
                raycastTarget14 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 28,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean useGUILayout
        {
            get
            {
                return self.useGUILayout;
            }
            set
            {
                if (useGUILayout15 == value)
                    return;
                useGUILayout15 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 35,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public System.Boolean runInEditMode
        {
            get
            {
                return default;
            }
            set
            {
                if (runInEditMode16 == value)
                    return;
                runInEditMode16 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
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
                if (enabled17 == value)
                    return;
                enabled17 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 37,
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
                if (tag18 == value)
                    return;
                tag18 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 41,
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
                if (name19 == value)
                    return;
                name19 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 55,
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
                if (hideFlags20 == value)
                    return;
                hideFlags20 = value;
                ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
                {
                    index = networkIdentity.registerObjectIndex,
                    index1 = 56,
                    buffer = Net.Serialize.NetConvertFast2.SerializeObject(value).ToArray(true)
                });
            }
        }

        public void DisableSpriteOptimizations(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 59,
                buffer = buffer
            });
        }
        public void OnBeforeSerialize(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 89,
                buffer = buffer
            });
        }
        public void OnAfterDeserialize(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 90,
                buffer = buffer
            });
        }
        public void SetNativeSize(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 91,
                buffer = buffer
            });
        }
        public void CalculateLayoutInputHorizontal(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 92,
                buffer = buffer
            });
        }
        public void CalculateLayoutInputVertical(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 93,
                buffer = buffer
            });
        }
        public void Cull(UnityEngine.Rect clipRect, System.Boolean validRect, bool always = false)
        {
            if (clipRect == clipRect1 & validRect == validRect2 & !always) return;
            clipRect1 = clipRect;
            validRect2 = validRect;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { clipRect, validRect } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 109,
                buffer = buffer
            });
        }
        public void SetClipRect(UnityEngine.Rect clipRect, System.Boolean validRect, bool always = false)
        {
            if (clipRect == clipRect3 & validRect == validRect4 & !always) return;
            clipRect3 = clipRect;
            validRect4 = validRect;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { clipRect, validRect } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 110,
                buffer = buffer
            });
        }
        public void SetClipSoftness(UnityEngine.Vector2 clipSoftness, bool always = false)
        {
            if (clipSoftness == clipSoftness5 & !always) return;
            clipSoftness5 = clipSoftness;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { clipSoftness } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 111,
                buffer = buffer
            });
        }
        public void RecalculateClipping(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 113,
                buffer = buffer
            });
        }
        public void RecalculateMasking(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 114,
                buffer = buffer
            });
        }
        public void SetAllDirty(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 119,
                buffer = buffer
            });
        }
        public void SetLayoutDirty(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 120,
                buffer = buffer
            });
        }
        public void SetVerticesDirty(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 121,
                buffer = buffer
            });
        }
        public void SetMaterialDirty(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 122,
                buffer = buffer
            });
        }
        public void OnCullingChanged(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 129,
                buffer = buffer
            });
        }
        public void Rebuild(UnityEngine.UI.CanvasUpdate update, bool always = false)
        {
            if (update == update6 & !always) return;
            update6 = update;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { update } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 130,
                buffer = buffer
            });
        }
        public void LayoutComplete(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 131,
                buffer = buffer
            });
        }
        public void GraphicUpdateComplete(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 132,
                buffer = buffer
            });
        }
        public void OnRebuildRequested(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 133,
                buffer = buffer
            });
        }
        public void PixelAdjustPoint(UnityEngine.Vector2 point, bool always = false)
        {
            if (point == point7 & !always) return;
            point7 = point;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { point } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 135,
                buffer = buffer
            });
        }
        public void GetPixelAdjustedRect(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 136,
                buffer = buffer
            });
        }
        public void CrossFadeColor(UnityEngine.Color targetColor, System.Single duration, System.Boolean ignoreTimeScale, System.Boolean useAlpha, bool always = false)
        {
            if (targetColor == targetColor8 & duration == duration9 & ignoreTimeScale == ignoreTimeScale10 & useAlpha == useAlpha11 & !always) return;
            targetColor8 = targetColor;
            duration9 = duration;
            ignoreTimeScale10 = ignoreTimeScale;
            useAlpha11 = useAlpha;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { targetColor, duration, ignoreTimeScale, useAlpha } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 137,
                buffer = buffer
            });
        }
        public void CrossFadeColor(UnityEngine.Color targetColor, System.Single duration, System.Boolean ignoreTimeScale, System.Boolean useAlpha, System.Boolean useRGB, bool always = false)
        {
            if (targetColor == targetColor12 & duration == duration13 & ignoreTimeScale == ignoreTimeScale14 & useAlpha == useAlpha15 & useRGB == useRGB16 & !always) return;
            targetColor12 = targetColor;
            duration13 = duration;
            ignoreTimeScale14 = ignoreTimeScale;
            useAlpha15 = useAlpha;
            useRGB16 = useRGB;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { targetColor, duration, ignoreTimeScale, useAlpha, useRGB } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 138,
                buffer = buffer
            });
        }
        public void CrossFadeAlpha(System.Single alpha, System.Single duration, System.Boolean ignoreTimeScale, bool always = false)
        {
            if (alpha == alpha17 & duration == duration18 & ignoreTimeScale == ignoreTimeScale19 & !always) return;
            alpha17 = alpha;
            duration18 = duration;
            ignoreTimeScale19 = ignoreTimeScale;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { alpha, duration, ignoreTimeScale } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 139,
                buffer = buffer
            });
        }
        public void IsActive(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 146,
                buffer = buffer
            });
        }
        public void IsDestroyed(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 147,
                buffer = buffer
            });
        }
        public void IsInvoking(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 148,
                buffer = buffer
            });
        }
        public void CancelInvoke(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 149,
                buffer = buffer
            });
        }
        public void Invoke(System.String methodName, System.Single time, bool always = false)
        {
            if (methodName == methodName20 & time == time21 & !always) return;
            methodName20 = methodName;
            time21 = time;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, time } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 150,
                buffer = buffer
            });
        }
        public void InvokeRepeating(System.String methodName, System.Single time, System.Single repeatRate, bool always = false)
        {
            if (methodName == methodName22 & time == time23 & repeatRate == repeatRate24 & !always) return;
            methodName22 = methodName;
            time23 = time;
            repeatRate24 = repeatRate;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, time, repeatRate } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 151,
                buffer = buffer
            });
        }
        public void CancelInvoke(System.String methodName, bool always = false)
        {
            if (methodName == methodName25 & !always) return;
            methodName25 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 152,
                buffer = buffer
            });
        }
        public void IsInvoking(System.String methodName, bool always = false)
        {
            if (methodName == methodName26 & !always) return;
            methodName26 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 153,
                buffer = buffer
            });
        }
        public void StartCoroutine(System.String methodName, bool always = false)
        {
            if (methodName == methodName27 & !always) return;
            methodName27 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 154,
                buffer = buffer
            });
        }
        public void CompareTag(System.String tag, bool always = false)
        {
            if (tag == tag28 & !always) return;
            tag28 = tag;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { tag } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 199,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, bool always = false)
        {
            if (methodName == methodName29 & !always) return;
            methodName29 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 202,
                buffer = buffer
            });
        }
        public void SendMessageUpwards(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName30 & options == options31 & !always) return;
            methodName30 = methodName;
            options31 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 203,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName32 & !always) return;
            methodName32 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 205,
                buffer = buffer
            });
        }
        public void SendMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName33 & options == options34 & !always) return;
            methodName33 = methodName;
            options34 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 207,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, bool always = false)
        {
            if (methodName == methodName35 & !always) return;
            methodName35 = methodName;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 210,
                buffer = buffer
            });
        }
        public void BroadcastMessage(System.String methodName, UnityEngine.SendMessageOptions options, bool always = false)
        {
            if (methodName == methodName36 & options == options37 & !always) return;
            methodName36 = methodName;
            options37 = options;
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { methodName, options } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 211,
                buffer = buffer
            });
        }
        public void GetInstanceID(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 225,
                buffer = buffer
            });
        }
        public void GetHashCode(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 226,
                buffer = buffer
            });
        }
        public void ToString(bool always = false)
        {
            var buffer = Net.Serialize.NetConvertFast2.SerializeModel(new RPCModel() { pars = new object[] { } });
            ClientManager.AddOperation(new Operation(Command.BuildComponent, networkIdentity.identity)
            {
                index = networkIdentity.registerObjectIndex,
                index1 = 232,
                buffer = buffer
            });
        }
        public override void OnPropertyAutoCheck()
        {
            if (!autoCheck)
                return;
            type = type;
            preserveAspect = preserveAspect;
            fillCenter = fillCenter;
            fillMethod = fillMethod;
            fillAmount = fillAmount;
            fillClockwise = fillClockwise;
            fillOrigin = fillOrigin;
            alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;
            useSpriteMesh = useSpriteMesh;
            pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
            maskable = maskable;
            isMaskingGraphic = isMaskingGraphic;
            color = color;
            raycastTarget = raycastTarget;
            useGUILayout = useGUILayout;
            runInEditMode = runInEditMode;
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
                case 2:
                    type1 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.UI.Image.Type>(new Net.System.Segment(opt.buffer, false));
                    self.type = type1;
                    break;
                case 3:
                    preserveAspect2 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.preserveAspect = preserveAspect2;
                    break;
                case 4:
                    fillCenter3 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.fillCenter = fillCenter3;
                    break;
                case 5:
                    fillMethod4 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.UI.Image.FillMethod>(new Net.System.Segment(opt.buffer, false));
                    self.fillMethod = fillMethod4;
                    break;
                case 6:
                    fillAmount5 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.fillAmount = fillAmount5;
                    break;
                case 7:
                    fillClockwise6 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.fillClockwise = fillClockwise6;
                    break;
                case 8:
                    fillOrigin7 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Int32>(new Net.System.Segment(opt.buffer, false));
                    self.fillOrigin = fillOrigin7;
                    break;
                case 10:
                    alphaHitTestMinimumThreshold8 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold8;
                    break;
                case 11:
                    useSpriteMesh9 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.useSpriteMesh = useSpriteMesh9;
                    break;
                case 14:
                    pixelsPerUnitMultiplier10 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Single>(new Net.System.Segment(opt.buffer, false));
                    self.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier10;
                    break;
                case 25:
                    maskable11 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.maskable = maskable11;
                    break;
                case 26:
                    isMaskingGraphic12 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.isMaskingGraphic = isMaskingGraphic12;
                    break;
                case 27:
                    color13 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.Color>(new Net.System.Segment(opt.buffer, false));
                    self.color = color13;
                    break;
                case 28:
                    raycastTarget14 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.raycastTarget = raycastTarget14;
                    break;
                case 35:
                    useGUILayout15 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.useGUILayout = useGUILayout15;
                    break;
                case 36:
                    //runInEditMode16 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    //self.runInEditMode = runInEditMode16;
                    break;
                case 37:
                    enabled17 = Net.Serialize.NetConvertFast2.DeserializeObject<System.Boolean>(new Net.System.Segment(opt.buffer, false));
                    self.enabled = enabled17;
                    break;
                case 41:
                    tag18 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.tag = tag18;
                    break;
                case 55:
                    name19 = Net.Serialize.NetConvertFast2.DeserializeObject<System.String>(new Net.System.Segment(opt.buffer, false));
                    self.name = name19;
                    break;
                case 56:
                    hideFlags20 = Net.Serialize.NetConvertFast2.DeserializeObject<UnityEngine.HideFlags>(new Net.System.Segment(opt.buffer, false));
                    self.hideFlags = hideFlags20;
                    break;
                case 59:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.DisableSpriteOptimizations();
                    }
                    break;
                case 89:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.OnBeforeSerialize();
                    }
                    break;
                case 90:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.OnAfterDeserialize();
                    }
                    break;
                case 91:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.SetNativeSize();
                    }
                    break;
                case 92:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.CalculateLayoutInputHorizontal();
                    }
                    break;
                case 93:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.CalculateLayoutInputVertical();
                    }
                    break;
                case 109:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var clipRect = (UnityEngine.Rect)data.pars[0];
                        var validRect = (System.Boolean)data.pars[1];
                        self.Cull(clipRect, validRect);
                    }
                    break;
                case 110:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var clipRect = (UnityEngine.Rect)data.pars[0];
                        var validRect = (System.Boolean)data.pars[1];
                        self.SetClipRect(clipRect, validRect);
                    }
                    break;
                case 111:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var clipSoftness = (UnityEngine.Vector2)data.pars[0];
                        self.SetClipSoftness(clipSoftness);
                    }
                    break;
                case 113:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.RecalculateClipping();
                    }
                    break;
                case 114:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.RecalculateMasking();
                    }
                    break;
                case 119:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.SetAllDirty();
                    }
                    break;
                case 120:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.SetLayoutDirty();
                    }
                    break;
                case 121:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.SetVerticesDirty();
                    }
                    break;
                case 122:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.SetMaterialDirty();
                    }
                    break;
                case 129:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.OnCullingChanged();
                    }
                    break;
                case 130:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var update = (UnityEngine.UI.CanvasUpdate)data.pars[0];
                        self.Rebuild(update);
                    }
                    break;
                case 131:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.LayoutComplete();
                    }
                    break;
                case 132:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GraphicUpdateComplete();
                    }
                    break;
                case 133:
                    {
                        //var segment = new Net.System.Segment(opt.buffer, false);
                        //var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        //self.OnRebuildRequested();
                    }
                    break;
                case 135:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var point = (UnityEngine.Vector2)data.pars[0];
                        self.PixelAdjustPoint(point);
                    }
                    break;
                case 136:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetPixelAdjustedRect();
                    }
                    break;
                case 137:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var targetColor = (UnityEngine.Color)data.pars[0];
                        var duration = (System.Single)data.pars[1];
                        var ignoreTimeScale = (System.Boolean)data.pars[2];
                        var useAlpha = (System.Boolean)data.pars[3];
                        self.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
                    }
                    break;
                case 138:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var targetColor = (UnityEngine.Color)data.pars[0];
                        var duration = (System.Single)data.pars[1];
                        var ignoreTimeScale = (System.Boolean)data.pars[2];
                        var useAlpha = (System.Boolean)data.pars[3];
                        var useRGB = (System.Boolean)data.pars[4];
                        self.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB);
                    }
                    break;
                case 139:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var alpha = (System.Single)data.pars[0];
                        var duration = (System.Single)data.pars[1];
                        var ignoreTimeScale = (System.Boolean)data.pars[2];
                        self.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
                    }
                    break;
                case 146:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.IsActive();
                    }
                    break;
                case 147:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.IsDestroyed();
                    }
                    break;
                case 148:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.IsInvoking();
                    }
                    break;
                case 149:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.CancelInvoke();
                    }
                    break;
                case 150:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var time = (System.Single)data.pars[1];
                        self.Invoke(methodName, time);
                    }
                    break;
                case 151:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var time = (System.Single)data.pars[1];
                        var repeatRate = (System.Single)data.pars[2];
                        self.InvokeRepeating(methodName, time, repeatRate);
                    }
                    break;
                case 152:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.CancelInvoke(methodName);
                    }
                    break;
                case 153:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.IsInvoking(methodName);
                    }
                    break;
                case 154:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.StartCoroutine(methodName);
                    }
                    break;
                case 199:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var tag = data.pars[0] as System.String;
                        self.CompareTag(tag);
                    }
                    break;
                case 202:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessageUpwards(methodName);
                    }
                    break;
                case 203:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessageUpwards(methodName, options);
                    }
                    break;
                case 205:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.SendMessage(methodName);
                    }
                    break;
                case 207:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.SendMessage(methodName, options);
                    }
                    break;
                case 210:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        self.BroadcastMessage(methodName);
                    }
                    break;
                case 211:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        var methodName = data.pars[0] as System.String;
                        var options = (UnityEngine.SendMessageOptions)data.pars[1];
                        self.BroadcastMessage(methodName, options);
                    }
                    break;
                case 225:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetInstanceID();
                    }
                    break;
                case 226:
                    {
                        var segment = new Net.System.Segment(opt.buffer, false);
                        var data = Net.Serialize.NetConvertFast2.DeserializeModel(segment);
                        self.GetHashCode();
                    }
                    break;
                case 232:
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