using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 애니메이션
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterAnimator : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idle;
    [SerializeField] private Sprite shoot;
    public float animationDuration;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idle;
    }
    
    public void ShootAnimation()
    {
        StartCoroutine(ShootAnimationCoroutine(animationDuration));
    }
    
    IEnumerator ShootAnimationCoroutine(float duration)
    {
        spriteRenderer.sprite = shoot;
        yield return new WaitForSeconds(duration);
        spriteRenderer.sprite = idle;
    }
}
