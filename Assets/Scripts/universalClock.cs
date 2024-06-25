using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class universalClock : MonoBehaviour
{
    static public universalClock mainClock = null;
    // Clock that all gametime adherese to
    [SerializeField]
    float initialTime;
    // How fast time passes compared to actual time
    [SerializeField]
    float timeScale;
    [SerializeField]
    int startMinute;
    public class TimeRep
    {
        public int hours;
        public int minutes;
        public TimeRep(int hourSet, int minuteSet)
        {
            hours = hourSet;
            minutes = minuteSet;
        }
    }
    public static int timeToMinutes(int hours, int minutes)
    {
        return hours * 60 + minutes;
    }
    public static TimeRep minutesToTime(int hours, int minutes)
    {
        return new TimeRep(hours, minutes);
    }

    // Start is called before the first frame update
    void Start()
    {
        initialTime = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
