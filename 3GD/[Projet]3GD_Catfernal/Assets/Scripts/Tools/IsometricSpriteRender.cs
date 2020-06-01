using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tools
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class IsometricSpriteRender : MonoBehaviour
    {

        [SerializeField] private float offset = 0;
        private new Renderer renderer;

        private void Awake()
        {
            renderer = this.GetComponent<Renderer>();
        }

        private void Update()
        {
            renderer.sortingOrder = (int)((this.transform.position.y + this.offset) * -100);
        }
    }
}
