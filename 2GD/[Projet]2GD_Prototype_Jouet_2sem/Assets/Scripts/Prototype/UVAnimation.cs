using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVAnimation : MonoBehaviour {

    public int uvTileY = 2;
    public int uvTileX = 4;

    public int framePerSec = 5;

    private int index;

    public Vector2 size;
    public Vector2 offset;

    public Renderer render;

    private Coroutine coroutineAnimation;

    private void Start()
    {
        render = GetComponent<Renderer>();  
    }

    private void OnBecameVisible()
    {
        coroutineAnimation = StartCoroutine(AnimatedSprite());
    }

    private void OnBecameInvisible()
    {
        StopCoroutine(coroutineAnimation);
    }

    IEnumerator AnimatedSprite()
    {
        while (true)
        {
            index++;

            index = index % (uvTileX * uvTileY - 1);

            size = new Vector2(1f / uvTileX, 1f / uvTileY);

            var uIndex = index % uvTileX;
            var vIndex = index / uvTileX;

            offset = new Vector2(uIndex * size.x, 1f - size.y - vIndex * size.y);

            render.material.SetTextureOffset("_MainTex", offset);
            render.material.SetTextureScale("_MainTex", size);

            yield return new WaitForSeconds(1f / framePerSec);
        }
    }
}
