using UnityEngine;
using System.Collections;
public class TimeObject : MonoBehaviour
{
    public int TimeId { get; private set; }

    [SerializeField] protected float speedTime;

    private void Awake()
    {
        TimeId = GameTime.Instance.RegisterTimeObj(this);
        speedTime = 1f;
    }

    public void SetSpeedTime(float speedTime)
    {
        if (speedTime < 0)
        {
            return;
        }
        this.speedTime = speedTime;
    }
}