#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using System.Collections.Generic;
using UnityEngine;

namespace Example2
{
    public class GameManager : SingleCase<GameManager>
    {
        public List<Player> players = new List<Player>();
        public List<AIMonster> enemys = new List<AIMonster>();
        public RectTransform canvasRT;
        public HeadBloodBar headBloodBar;
        public FlyingWord flyingWord;
        public Transform UIRoot;
    }
}
#endif