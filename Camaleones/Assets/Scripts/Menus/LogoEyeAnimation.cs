using UnityEngine;
using DG.Tweening;

public class LogoEyeAnimation : MonoBehaviour
{
    public float minInterval;
    public float maxInterval;

    public float tweenDuration = 0.3f;
    
    private void Start()
    {
        Invoke("SetRandomEyeRotation", Random.Range(minInterval, maxInterval));
    }

    private void SetRandomEyeRotation()
    {
        float newTargetRotation = transform.rotation.eulerAngles.z + Random.Range(-120f, 120f);
        transform.DOKill();
        transform.DORotate(new Vector3(0, 0, newTargetRotation), tweenDuration, RotateMode.FastBeyond360);

        Invoke("SetRandomEyeRotation", Random.Range(minInterval, maxInterval));        
    }
}
