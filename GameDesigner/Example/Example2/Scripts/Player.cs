#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using GameDesigner;
using UnityEngine;

namespace Example2
{
    public class Player : Actor
    {
        public GameObject bullet;
        public Transform firePoint;
        public bool IsLocal;

        private void Awake()
        {
            preHealth = health;

            headBloodBar = Instantiate(GameManager.I.headBloodBar, GameManager.I.UIRoot);
            headBloodBar.target = transform;
            headBloodBar.offset = headBarOffset;
            headBloodBar.text.text = $"{health:f0}/{healthMax:f0}";
            headBloodBar.image.fillAmount = health / healthMax;
        }

        internal void Fire()
        {
            var bullet1 = Instantiate(bullet, firePoint.position, firePoint.rotation);
            var rig1 = bullet1.GetComponent<Rigidbody>();
            rig1.AddForce(firePoint.forward * 1000f);
            var bullet2 = bullet1.GetComponent<Bullet>();
            bullet2.target = this;
            Destroy(bullet1, 5f);
        }

        public override void BeAttacked(Actor target, float damage)
        {
            if (isDead)
                return;
            var flyingWord = Instantiate(GameManager.I.flyingWord, GameManager.I.UIRoot);
            flyingWord.text.text = damage.ToString("f0");
            flyingWord.position = transform.position + new Vector3(0, 0.5f, 0);
            headBloodBar.text.text = $"{health:f0}/{healthMax:f0}";
            headBloodBar.image.fillAmount = health / healthMax;
        }

        public void Resurrection()
        {
            health = healthMax;
            isDead = false;
            GetComponent<PlayerAnimation>().enabled = true;
            headBloodBar.gameObject.SetActive(true);
            headBloodBar.text.text = $"{health:f0}/{healthMax:f0}";
            headBloodBar.image.fillAmount = health / healthMax;
        }

        internal void Check()
        {
            if (health <= 0 & !isDead) 
            {
                health = 0;
                isDead = true;
                GetComponent<PlayerAnimation>().enabled = false;
                GetComponentInChildren<Animation>().Play("soldierDieFront");
                if (IsLocal)
                {
                    StateEvent.AddEvent(1f, () =>
                    {
                        headBloodBar.gameObject.SetActive(false);
                        GameEvent.OnPlayerDead?.Invoke();
                    });
                }
            }
        }
    }
}
#endif