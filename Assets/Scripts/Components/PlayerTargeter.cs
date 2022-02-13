using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTargeter : MonoBehaviour
{
    private float _lastTargetUpdateTime;

    private Player _target;

    public Player target => _target;
    public void UpdateTarget(float cooldown, float moveRange)
    {
        if (Time.time - _lastTargetUpdateTime < cooldown) return;

        _lastTargetUpdateTime = Time.time;
        Player[] players = Object.FindObjectsOfType<Player>().Where(x => !x.isInvader).ToArray();
        float dist = Mathf.Abs(transform.position.x - players[0].transform.position.x);
        int targetIndex = 0;
        for (int i = 1; i < players.Length; i++)
        {
            float d = Mathf.Abs(transform.position.x - players[i].transform.position.x);
            if (d < dist)
            {
                dist = d;
                targetIndex = i;
            }
        }
        if (dist < moveRange) _target = players[targetIndex];
        else _target = null;
    }
}
