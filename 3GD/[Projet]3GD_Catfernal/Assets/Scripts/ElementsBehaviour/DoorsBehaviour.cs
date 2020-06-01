using System.Collections;
using System.Collections.Generic;
using Game.Constants;
using UnityEngine;

public class DoorsBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem fog = null;
    [SerializeField] private BoxCollider2D col = null;
    [SerializeField] private SpriteRenderer sprite = null;
    [SerializeField] private float life = 50f;

    private void OnParticleCollision(GameObject particleSystem)
    {
        if (particleSystem.tag.Contains(GameObjectTags.PlayerFlame))
        {
            this.life--;
        }
    }

    private void Update()
    {
        if (life <= 0)
        {
            this.fog.Stop();
            

            this.sprite.enabled = false;
            this.col.enabled = false;
        }
    }
}
