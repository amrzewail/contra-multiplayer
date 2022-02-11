using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _target;

    private bool _lockCameraToBossLocation = false;

    private IEnumerator Start()
    {
        LevelEvents.StageBossStarted += BossStartedCallback;
        LevelEvents.StageBossDefeated += BossDefeatedCallback;

        while (!_target)
        {

            Player[] players = GameObject.FindObjectsOfType<Player>();

            Player player = players.SingleOrDefault(x => x.isMine);

            if (player) _target = player.transform;


            yield return new WaitForSeconds(0.5f);
        }
    }

    private void LateUpdate()
    {
        if (!_target) return;

        Vector3 pos = transform.position;
        if (!_lockCameraToBossLocation)
        {
            pos.x = _target.position.x;
            pos.x = Mathf.Clamp(pos.x, -2, 190);
            transform.position = pos;
        }
        else
        {
            pos = Vector3.MoveTowards(pos, LevelController.instance.GetBossCameraLocation().position, Time.deltaTime * 4);
            transform.position = pos;
        }

        Vector3 targetPos = _target.transform.position;
        targetPos.x = Mathf.Clamp(targetPos.x, pos.x - 8, Mathf.Infinity);
        _target.transform.position = targetPos;
    }

    private void BossStartedCallback()
    {
        _lockCameraToBossLocation = true;
    }

    private void BossDefeatedCallback()
    {
        _lockCameraToBossLocation = false;
    }
}
