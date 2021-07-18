#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Share;
using TrueSync;
using UnityEngine;

namespace LockStep.Client
{
    public class Enemy : Actor
    {
        public ObjectView objectView;
        private Player target;

        //public Enemy(GameSystem gameSystem) : base(gameSystem)
        //{
        //    name = "怪物01";
        //    gameObject = UnityEngine.Object.Instantiate(Game.Instance.enemyObj);
        //    objectView = gameObject.GetComponent<ObjectView>();
        //    objectView.actor = this;
        //    transform = gameObject.GetComponent<TSTransform>();
        //}

        //public override void Awake()
        //{
        //    Debug.Log("awake");
        //}

        //public override void OnEnable()
        //{
        //    Debug.Log("enable");
        //}

        //public override void OnDisable()
        //{
        //    Debug.Log("disable");
        //}

        //public override void Start()
        //{
        //    Debug.Log("start");
        //}

        //public override void Update(Operation opt)
        //{
        //    if (target != null)
        //    {
        //        var dis = TSVector3.Distance(target.transform.position, transform.position);
        //        if (dis < 1f)
        //        {

        //        }
        //        else if (dis < 15f) 
        //        {

        //        }
        //    }
        //    else 
        //    {

        //    }
        //}

        //public override void OnDestroy()
        //{
        //    Debug.Log("destroy");
        //}
    } 
}
#endif