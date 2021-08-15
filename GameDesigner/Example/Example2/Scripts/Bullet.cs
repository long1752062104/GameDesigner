namespace Example2
{
    using Net.Component;
    using UnityEngine;

    public class Bullet : MonoBehaviour
    {
        internal Player target;
        public GameObject explosionPrefab;
        public int damage = 30;

        private void OnCollisionEnter(Collision collision)
        {
            var actor = collision.collider.GetComponent<Actor>();
            if (actor is AIMonster monster)
            {
                var contact = collision.contacts[0];
                var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                var pos = contact.point;
                var explosion = Instantiate(explosionPrefab, pos, rot);
                Destroy(explosion, 1f);
                monster.target = target;
                if (target.IsLocal)
                    ClientManager.AddOperation(new Net.Share.Operation(Command.Attack, monster.id) { index1 = damage });
                Destroy(gameObject);
            }
            else if (actor is Player player)
            {
                if (player == target)
                    return;
                var contact = collision.contacts[0];
                var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                var pos = contact.point;
                var explosion = Instantiate(explosionPrefab, pos, rot);
                Destroy(explosion, 1f);
                if (target.IsLocal)
                    ClientManager.AddOperation(new Net.Share.Operation(Command.AttackPlayer, player.id) { index1 = damage });
                Destroy(gameObject);
            }
        }
    }
}