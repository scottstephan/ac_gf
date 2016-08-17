using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class obj_Timer : MonoBehaviour {

    public static obj_Timer instance;

    public float timeUntilFail = 10f;
    public float curTime;
    public float tickRate = 1f;
    public Text timerText;
    //plus the timer image

    public enum E_timerState{
        paused,
        active,
        inactive    
    }

    public static E_timerState timerState = E_timerState.inactive;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void setTimer(float maxTimerTime)
    {
        timeUntilFail = maxTimerTime;
        curTime = timeUntilFail;
    }

    public void setTimer()
    {
        timeUntilFail = 10f;
        curTime = timeUntilFail;
    }

    public void startTimer()
    {
        if (timerState == E_timerState.inactive)
        {
            timerState = E_timerState.active;
            timerText.text = curTime.ToString();
            Invoke("updateTimer", tickRate);
        }
        else
            Debug.Log("Timer is already active; Not starting timer");
    }

    void endTimer()
    {
        timerState = E_timerState.inactive;
    }

    public void pauseTimer()
    {
        timerState = E_timerState.paused;
        CancelInvoke();
    }

    public void unpauseTimer()
    {
        if (timerState == E_timerState.paused)
        {
            Debug.Log("Timer unpaused");

            timerState = E_timerState.active;
            Invoke("updateTimer", tickRate);
        }
        else
        {
            Debug.Log("Timer wasn't paused; Can't unpause timer");
        }
    }

    public void stopTimer()
    {
        timerState = E_timerState.inactive;
        CancelInvoke();
    }

    void updateTimer()
    {
        if (timerState != E_timerState.active)
            return;

        curTime -= tickRate;
        timerText.text = curTime.ToString();

        if (curTime <= 0)
            timerHasReachedZero();
        else
            Invoke("updateTimer", tickRate);
    }

    void timerHasReachedZero()
    {
        //BAD THING.
        gameManager.instance.inputPhaseEnd(gameManager.E_endOfRoundAction.timerEnded);
        endTimer();
    }
}
