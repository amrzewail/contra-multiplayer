using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public MonoBehaviour target;

    private void OnEnable()
    {
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        int visible = 0;
        while (true)
        {
            visible++;
            target.enabled = (visible % 2 == 0);
            yield return new WaitForSeconds(0.35f);
        }
    }
}
