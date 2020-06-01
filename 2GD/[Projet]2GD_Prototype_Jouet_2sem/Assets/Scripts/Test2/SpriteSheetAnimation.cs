using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimation : MonoBehaviour {

    public SpriteRenderer spriteRender;
    public Sprite[] frames;
    public int frameIndex;
    public float delayBetweenFrames = 0.05f;

    private void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        frameIndex = 0;
        StartCoroutine(AnimateSprite());
    }

    IEnumerator AnimateSprite()
    {
        while (true)
        {
            frameIndex = (frameIndex + 1) % (frames.Length - 1);
            spriteRender.sprite = frames[frameIndex];
            yield return new WaitForSeconds(delayBetweenFrames);
        }
    }
}
