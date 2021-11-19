using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGFPSIntegrationTool.Utilities.General.Time
{
    public class CountDown
    {
        public float StartCount { get; private set; } = 0f;
        public float CurrentCount { get; private set; } = 0f;
        public bool HasStarted { get; private set; } = false;
        public bool HasStartedOnce { get; private set; } = false;
        public bool IsCountingDown { get; private set; } = false;

        // Returns true when the count down ends, but only just after
        public bool HasEnded
        {
            get
            {
                if (CurrentCount <= 0f && HasStarted)
                {
                    return true;
                }

                return false;
            }
        }

        // Returns true continuously after the count down ends
        public bool HasCountDownEnded
        {
            get
            {
                if (CurrentCount <= 0f && HasStartedOnce)
                {
                    return true;
                }

                return false;
            }
        }

        public void Start(float startCount)
        {
            StartCount = startCount;

            CurrentCount = StartCount;
            HasStarted = true;
            HasStartedOnce = true;
            IsCountingDown = true;
        }

        public void Resume()
        {
            IsCountingDown = true;
        }

        public void Pause()
        {
            IsCountingDown = false;
        }

        public void Update()
        {
            // Ensures HasStarted is false on the next frame of count down ending
            if (CurrentCount <= 0f)
            {
                HasStarted = false;
            }

            // Decrement if necessary
            if (CurrentCount > 0f && IsCountingDown)
            {
                CurrentCount -= UnityEngine.Time.deltaTime;

                if (CurrentCount <= 0f)
                {
                    CurrentCount = 0f;
                    IsCountingDown = false;
                }
            }
        }

        // Also accepts negitive value to deduct from count down
        public void AddToCountDown(int amountAdded)
        {
            CurrentCount += amountAdded;
        }

        public static void ManualDeltaTimeDecrement(ref float count)
        {
            if (count > 0f)
            {
                count -= UnityEngine.Time.deltaTime;

                if (count < 0f)
                {
                    count = 0;
                }
            }
        }

        public static void ManualDeltaTimeDecrement(ref float[] counts)
        {
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] > 0f)
                {
                    counts[i] -= UnityEngine.Time.deltaTime;

                    if (counts[i] < 0f)
                    {
                        counts[i] = 0;
                    }
                }
            }
        }
    }
}