using Example2;
using Net.Component;
using UnityEngine;
using Command = Example2.Command;

public class Bullet : MonoBehaviour
{
    internal Player target;
    public GameObject explosionPrefab;
    public int damage = 30;

    private void OnCollisionEnter(Collision collision)
    {
        var monster = collision.collider.GetComponent<AIMonster>();
        if (monster != null) 
        {
            var contact = collision.contacts[0];
            var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            var pos = contact.point;
            var explosion = Instantiate(explosionPrefab, pos, rot);
            Destroy(explosion, 1f);
            monster.target = target ;
            if (target.IsLocal)
                ClientManager.AddOperation(new Net.Share.Operation(Command.Attack, monster.id) { index1 = damage });
            Destroy(gameObject);
        }
    }
}
