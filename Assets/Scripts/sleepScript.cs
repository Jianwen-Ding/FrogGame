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
    [SerializeField]
    GameObject sleepInteractButton;
    [SerializeField]
    bool playerInRange;

    bool gaveWarning = false;
    public static void sleep()
    {
        universalClock.incrementDay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        sleepInteractButton = GameObject.FindGameObjectWithTag("SleepPrompt");
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
        if (playerInRange && QuestSys.QuestList[totalQuestList[0]].getCompletionState())
        {
            sleepInteractButton.transform.GetChild(0).gameObject.SetActive(true);
            if (Input.GetKeyUp(KeyCode.F))
            {
                sleep();
            }
        }
        else
        {
            sleepInteractButton.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Entering doorway is locked until a certain quest is completed
        if (other.transform.tag == "Player")
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
