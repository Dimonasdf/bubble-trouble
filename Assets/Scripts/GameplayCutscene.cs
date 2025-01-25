using System;
using System.Collections;
using UnityEngine;

public class GameplayCutscene : MonoBehaviour
{
    [SerializeField] private GameObject introCamera;

    [SerializeField] private GameObject screenProtector;

    [SerializeField] private Transform screenProtectorStart;

    [SerializeField] private Phone currentPhone;
    [SerializeField] private Material screenProtectorMaterial;

    // TODO remove
    [SerializeField] private float initialDelay = 3f;

    [SerializeField] private float cameraChangeTime = 1f;

    [SerializeField] private float protectorFlightTime = 1f;
    [SerializeField] private float protectorApplicationTime = 1f;

    private float _delayTimerTime;

    private static readonly int ProtectorApplicationProgressParameterID = Shader.PropertyToID("_RollProgress");

    private void OnEnable()
    {
        // TODO start on player accepting an order
        StartCoroutine(GameplayStartCutscene());
    }

    private IEnumerator GameplayStartCutscene()
    {
        yield return new WaitForSeconds(initialDelay);

        introCamera.SetActive(false);
        yield return new WaitForSeconds(cameraChangeTime);

        screenProtector.transform.position = screenProtectorStart.position;
        screenProtectorMaterial.SetFloat(ProtectorApplicationProgressParameterID, 0f);

        screenProtector.transform.localScale = currentPhone.GetProtectorScale;

        yield return DelayWithCallbacks(protectorFlightTime,
                                        (progress) =>
                                        {
                                            screenProtector.transform.position = Vector3.Lerp(screenProtectorStart.position,
                                                                                              currentPhone.ProtectorMin.position,
                                                                                              progress);
                                        },
                                        () => { screenProtector.transform.position = currentPhone.ProtectorMin.position; });

        yield return DelayWithCallbacks(protectorApplicationTime,
                                        (progress) => { screenProtectorMaterial.SetFloat(ProtectorApplicationProgressParameterID, progress); },
                                        () => { screenProtectorMaterial.SetFloat(ProtectorApplicationProgressParameterID, 1f); });
    }

    private IEnumerator DelayWithCallbacks(float delayTime, Action<float> updateAction, Action completeAction)
    {
        var timeLeft = delayTime;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            updateAction?.Invoke(1 - (timeLeft / delayTime));
            yield return null;
        }
        completeAction?.Invoke();
    }
}
