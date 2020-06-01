using Game.Constants;
using UnityEngine;

namespace Game.Behaviors
{
    // Not working for now, can't detect particle trigger
    public class FlameFloor : MonoBehaviour
    {
        [SerializeField] ParticleSystem particles;
        [SerializeField] private new Rigidbody2D rigidbody2D;
        public int flameCount;

        // OnParticleTrigger called only when in the ParticleSystem gameObject
        //private void OnParticleTrigger()
        //{
        //    ContactFilter2D filter2D = new ContactFilter2D();
        //    Collider2D[] results = new Collider2D[0];
        //    int triggerNumber = this.rigidbody2D.OverlapCollider(filter2D, results);
        //    if(triggerNumber > 0)
        //    {
        //        foreach(Collider2D collider2D in results)
        //        {
        //            if(collider2D.gameObject.tag.Contains(GameObjectTags.Flame))
        //            {
        //                flameCount += 1;
        //                CheckCount();
        //            }
        //        }
        //    }
        //}

        private void CheckCount()
        {
            if (flameCount >= 50)
            {
                FlamedFloor();
            }
        }

        private void FlamedFloor()
        {
            this.particles.Play(true);
        }
    }
}