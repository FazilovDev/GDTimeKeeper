using UnityEngine;
using System.Collections;

public class InteractiveSystem : MonoBehaviour
{
    public GameObject AvailableObj;

    private Transform cachedTransform;

    private float offset = 0.3f;
    private float distance = 3f;

    private void Start()
    {
        cachedTransform = transform;
    }

    private void FixedUpdate()
    {
        var ray = new Ray(cachedTransform.position, cachedTransform.forward * offset);
        var isHit = Physics.Raycast(ray, out var hitInfo, distance);
        if (isHit)
        {
            AvailableObj = hitInfo.transform.gameObject;
        }
        else
        {
            AvailableObj = null;
        }
    }
}