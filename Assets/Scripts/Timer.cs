using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentTimer;
    [SerializeField] TextMeshProUGUI lastTimer;
    [SerializeField] TextMeshProUGUI bestTimer;

    [SerializeField] private bool isRunning = false;

    private float startTime;

    private float last = float.MaxValue;
    private float best = float.MaxValue;

    private void Update()
    {
        if (isRunning)
            currentTimer.text = $"Current : {(Time.time - startTime).ToString("00:00.00")}s";
    }

    public void StartRun()
    {
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void EndRun()
    {
        float perfectRunTime = Time.time - startTime;
        if (isRunning)
        {
            StopTimer();
            last = perfectRunTime;
            if (last < best)
            {
                StartCoroutine(SetNewBest(best, last));
                currentTimer.text = $"Current : 00:00.00s";
            }
            else
            {
                currentTimer.text = $"Current : {perfectRunTime.ToString("00:00.00")}s";
            }
        }
    }

    private IEnumerator SetNewBest(float last, float best)
    {
        float time = 5f;
        bestTimer.color = new Color(1f, 0f, 0f);
        for (int i = 0; i < 60; i++)
        {
            bestTimer.text = $"Best : {(last - (i / 60f) * (last - best)).ToString("00:00.00")}s";
            bestTimer.color = new Color(1, i / 60f, i / 60f);
            yield return new WaitForSeconds(time / 60f);
        }
        this.best = best;
        lastTimer.text = $"Last : {this.last.ToString("00:00.00")}s";
        bestTimer.text = $"Best : {best.ToString("00:00.00")}s";
        bestTimer.color = Color.white;
    }
}
