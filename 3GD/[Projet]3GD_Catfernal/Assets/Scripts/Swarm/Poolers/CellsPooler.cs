using System.Collections;
using System.Collections.Generic;
using DesignPattern.ObjectPooling;
using UnityEngine;

namespace Game.Entities.Swarm
{
    [AddComponentMenu("Game/Swarm/Cells Pooler")]
    public class CellsPooler : AbstractPool<CellSwarmBehavior>
    {
        public override void CustomAwake()
        {
            base.CustomAwake();
            RenameObjects();
        }

        private void RenameObjects()
        {
            for (var i = 0; i < entities.Length; i++){
                entities[i].name = "Cell" + i;
            }
        }

        public CellSwarmBehavior GetNearestActiveEntity(Vector3 position)
        {
            var distanceMin = 30.0f;
            CellSwarmBehavior nearestEntity = null;
            
            foreach (var entity in entities){
                if (!entity.IsEnable) continue;
                
                var distance = Vector3.Distance(position, entity.transform.position);

                if (distance < distanceMin){
                    distanceMin = distance;
                    nearestEntity = entity;
                }
            }
            Debug.Log(distanceMin);
            return nearestEntity;
        }
    }
}
