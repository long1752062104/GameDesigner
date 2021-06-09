#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using UnityEngine;

namespace Example2
{
	public class PlayerAnimation : MonoBehaviour
	{
		private Animation anim;
		private Vector3 oldPosition;
		private Vector3 newPosition;
		private float time;
        internal bool isGround;

        void Start()
		{
			anim = GetComponentInChildren<Animation>();
			oldPosition = transform.position;
		}

		void LateUpdate()
		{
			if (Time.time > time)
			{
				newPosition = transform.position;
				if (isGround)
				{
					if (Vector3.Distance(oldPosition, newPosition) > 0.02f)
					{
						anim.CrossFade("soldierRun");
						oldPosition = newPosition;
					}
					else
					{
						anim.CrossFade("soldierIdleRelaxed");
					}
				}
				else 
				{
					anim.CrossFade("soldierFalling");
				}
				time = Time.time + (1f / 50f);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.collider.name == "PlayGround")
				isGround = true;
		}

		private void OnCollisionExit(Collision collision)
		{
			if (collision.collider.name == "PlayGround")
				isGround = false;
		}
	}
}
#endif