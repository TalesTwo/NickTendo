using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float defaultDuration = 0.15f;
    [SerializeField] private float defaultMagnitude = 0.3f;

    private Vector3 _shakeOffset = Vector3.zero;
    private Coroutine _shakeCoroutine;

    public Vector3 CurrentOffset => _shakeOffset;

    public void ShakeOnce(float duration = -1f, float magnitude = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        if (magnitude <= 0f) magnitude = defaultMagnitude;

        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float damper = 1f - (elapsed / duration);
            float x = Random.Range(-1f, 1f) * magnitude * damper;
            float y = Random.Range(-1f, 1f) * magnitude * damper;
            _shakeOffset = new Vector3(x, y, 0f);
            yield return null;
        }

        _shakeOffset = Vector3.zero;
        _shakeCoroutine = null;
    }
}