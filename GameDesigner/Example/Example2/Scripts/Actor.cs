using GameDesigner;
using UnityEngine;

namespace Example2
{
    public class Actor : MonoBehaviour
    {
        public int id;
        public float health = 100f;
        public float healthMax = 100f;
        internal float preHealth;
        public float moveSpeed = 6f;
        public float damage = 30f;
        public float defense = 5f;
        public float returnBlood = 5f;
        public bool isDead;
        internal HeadBloodBar headBloodBar;
        public Vector3 headBarOffset = new Vector3(0, 1f, 0);
        public StateManager sm;

        public virtual void BeAttacked(Actor target, float damage)
        {
            var flyingWord = Instantiate(GameManager.I.flyingWord, GameManager.I.UIRoot);
            flyingWord.text.text = damage.ToString("f0");
            flyingWord.position = transform.position + new Vector3(0, 0.5f, 0);
            headBloodBar.text.text = $"{health:f0}/{healthMax:f0}";
            headBloodBar.image.fillAmount = health / healthMax;
        }
    }
}