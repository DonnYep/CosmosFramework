using UnityEngine;
using Cosmos;
public class FutureTaskTester : MonoBehaviour
{
    bool result = false;
    [SerializeField] float duration = 6;
    float currentTime = 0;
    async void Start()
    {
        Debug.Log("FutureTask await before");
        FutureTask task = new FutureTask(() => { return result; });
        await task;//await等待result=ture。若不为true，则阻塞。
        Debug.Log("FutureTask await after");
    }
    void OnValidate()
    {
        if (duration < 0)
            duration = 0;
    }
    void Update()
    {
        //这里表示若干秒后result =true
        currentTime += Time.deltaTime;
        if (currentTime >= duration)
        {
            currentTime = duration;
            result = true;
        }
    }
}
