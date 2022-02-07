using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _target;

    private IEnumerator Start()
    {
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
        pos.x = _target.position.x;
        transform.position = pos;
    }
}
