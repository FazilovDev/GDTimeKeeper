using System;
using System.Collections.Generic;
using System.Linq;

public class GameTime
{
    private static GameTime instance;

    public static GameTime Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameTime();
            }
            return instance;
        }
    }

    public HashSet<Tuple<int, TimeObject>> TimeObjects = new HashSet<Tuple<int, TimeObject>>();
    public float Speed { get; set; } = 1f;

    private int currentTimeObjId;

    private GameTime()
    {
        currentTimeObjId = -1;
    }

    private Tuple<int, TimeObject> GetObjById(int id)
    {
        return TimeObjects.First(t => t.Item1 == id);
    }

    public bool RemoveObj(int id)
    {
        return TimeObjects.Remove(GetObjById(id));
    }

    public void StopAll()
    {
        SetSpeedTime(0);
    }

    public void DefaultAll()
    {
        SetSpeedTime(1f);
    }

    public void ApplyCurrentSpeed()
    {
        SetSpeedTime(Speed);
    }

    public void SetSpeed(float value)
    {
        Speed = value;
        ApplyCurrentSpeed();
    }

    public int RegisterTimeObj(TimeObject obj)
    {
        currentTimeObjId++;
        TimeObjects.Add(new Tuple<int, TimeObject>(currentTimeObjId, obj));
        return currentTimeObjId;
    }

    public void SetSpeedTime(float value)
    {
        TimeObjects.Map(t => t.Item2.SetSpeedTime(value));
    }

    public void SetSpeedTimeById(int id, float speedTime)
    {
        TimeObjects.First(t => t.Item1 == id).Item2.SetSpeedTime(speedTime);
    }
}
