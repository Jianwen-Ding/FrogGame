using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class sleepScript : QuestInsertionBase
{
    // A way to enforce sleep times
    [SerializeField]
    int sleepHour;
    [SerializeField]
    int sleepMinute;
    bool gaveWarning = false;
    public static void sleep()
    {
        universalClock.incrementDay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if(universalClock.mainGameTime.hours > sleepHour && universalClock.mainGameTime.minutes > sleepMinute)
        {
            sleep();
        }
        if (!gaveWarning && universalClock.mainGameTime.hours > sleepHour - 1)
        {
            notificationSystem.notify("There is one hour remaining until you fall asleep and the enviroment resets");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Entering doorway is locked until a certain quest is completed
        if (QuestSys.QuestList[totalQuestList[0]].getCompletionState() && collision.transform.tag == "Player")
        {
            sleep();
        }
    }
}
