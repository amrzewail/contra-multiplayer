using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreserveTransform : MonoBehaviour
{
    [SerializeField] Vector3 localScale;
    [SerializeField] bool preserveScale;


    // Update is called once per frame
    void LateUpdate()
    {
        if (preserveScale)
        {
            Vector3 lossyScale = transform.parent.lossyScale;
            if (lossyScale.magnitude > 0)
            {
                transform.localScale = new Vector3(localScale.x / lossyScale.x, localScale.y / lossyScale.y, localScale.z / lossyScale.z);
            }
        }
    }
}
