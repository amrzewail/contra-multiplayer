using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Invincibility : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> renderers;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;
    [SerializeField] float invincibilityTime = 3;

    public IHitbox hitbox => (IHitbox)_hitbox;

    public bool isInvincible = false;

    public UnityEvent OnStartInvincible;

    public void Trigger()
    {
        if (isInvincible) return;

        isInvincible = hitbox.isInvincible = true;
        StartCoroutine(InvincibilityCoroutine());
        OnStartInvincible?.Invoke();

    }



    private IEnumerator InvincibilityCoroutine()
    {
        int visible = 0;
        float startTime = Time.time;
        while(Time.time - startTime < invincibilityTime)
        {
            visible++;
            renderers.ForEach(x => x.enabled = (visible % 2 == 0));
            yield return new WaitForSeconds(0.1f);
        }
        renderers.ForEach(x => x.enabled = true);
        isInvincible = hitbox.isInvincible = false;
    }
}
