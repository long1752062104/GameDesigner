#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
namespace MVC.Control
{
    using UnityEngine;
    using System.IO;
    using ILRuntime.CLR.Method;
    using System;
    using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
    using ILRuntime.Runtime.CLRBinding;
    using Net.Component;

    public class GameInit : SingleCase<GameInit>
    {
        private AppDomain appdomain;
        private MemoryStream dllStream;
        private MemoryStream pdbStream;
        private IMethod updateMethod;
#if UNITY_EDITOR
        public enum SelectPath { StreamingAssets, AssetsPath, FullPath }
        public SelectPath pathMode = SelectPath.StreamingAssets;
#endif
        public string dllPath;
        public string pdbPath;
        public Action<AppDomain> OnRegisterDelegate;

        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            appdomain = new AppDomain();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            var dllPath = Application.persistentDataPath + "/" + Path.GetFileName(this.dllPath);
            var pdbPath = Application.persistentDataPath + "/" + Path.GetFileName(this.pdbPath);
            Debug.Log(dllPath);
#elif !UNITY_EDITOR
            var dllPath = Application.streamingAssetsPath + "/" + Path.GetFileName(this.dllPath);
            var pdbPath = Application.streamingAssetsPath + "/" + Path.GetFileName(this.pdbPath);
            Debug.Log(dllPath);
#endif
#if UNITY_EDITOR
            string dllPath;
            string pdbPath;
            if (pathMode == SelectPath.StreamingAssets)
            {
                dllPath = Application.streamingAssetsPath + "/" + this.dllPath;
                pdbPath = Application.streamingAssetsPath + "/" + this.pdbPath;
            }
            else if (pathMode == SelectPath.AssetsPath)
            {
                dllPath = Application.dataPath + "/" + this.dllPath;
                pdbPath = Application.dataPath + "/" + this.pdbPath;
            }
            else
            {
                dllPath = this.dllPath;
                pdbPath = this.pdbPath;
            }
#endif
            if (File.Exists(dllPath))
                 dllStream = new MemoryStream(File.ReadAllBytes(dllPath));
            if (File.Exists(pdbPath))
                pdbStream = new MemoryStream(File.ReadAllBytes(pdbPath));
            appdomain.LoadAssembly(dllStream, pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            CLRBindingUtils.Initialize(appdomain);
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<bool>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<bool>((value) =>
                {
                    ((Action<bool>)act)(value);
                });
            });
            appdomain.DelegateManager.RegisterMethodDelegate<bool>();
            appdomain.DelegateManager.RegisterMethodDelegate<bool, ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            OnRegisterDelegate?.Invoke(appdomain);
            var method = appdomain.LoadedTypes["Hotfix.GameEntry"].GetMethod("Init", 0);
            appdomain.Invoke(method, null);
            updateMethod = appdomain.LoadedTypes["Hotfix.GameEntry"].GetMethod("Update", 0);
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            appdomain.Invoke(updateMethod, null);
        }

        private void OnDestroy()
        {
            if (dllStream != null)
                dllStream.Close();
            if (pdbStream != null)
                pdbStream.Close();
            appdomain = null;
        }
    }
}
#endif