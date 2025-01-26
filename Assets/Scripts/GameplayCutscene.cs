using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayCutscene : MonoBehaviour
{
    [SerializeField] private GameObject introCamera;

    [SerializeField] private GameObject screenProtector;

    [SerializeField] private Transform screenProtectorsBox;
    [SerializeField] private Transform screenProtectorStart;

    [SerializeField] private Material screenProtectorMaterial;

    [SerializeField] private Transform phonePoint;
    [SerializeField] private List<Phone> phones = new();

    [SerializeField] private BubblesController bubblesController;

    [SerializeField] private float initialDelay = 3f;

    [SerializeField] private float cameraChangeTime = 1f;

    [SerializeField] private float protectorFlightTime = 1f;
    [SerializeField] private float protectorApplicationTime = 1f;

    [SerializeField] private float clientMovementTime = 1f;
    [SerializeField] private Material clientMaterial;
    [SerializeField] private List<Texture2D> clients = new();

    [SerializeField] private Transform client;
    [SerializeField] private AnimationCurve clientPositionZ;
    [SerializeField] private AnimationCurve clientRotationZ;
    [SerializeField] private AnimationCurve clientScale;

    [SerializeField] private Scraper scraper;

    private Phone _currentPhone;
    private Coroutine _cutsceneCoroutine;

    private static readonly int ProtectorApplicationProgressParameterID = Shader.PropertyToID("_RollProgress");

    public void StartGame()
    {
        _cutsceneCoroutine = StartCoroutine(GameplayStartCutscene());
    }

    private IEnumerator GameplayStartCutscene()
    {
        introCamera.SetActive(true);

        // set client
        clientMaterial.SetTexture("_MainTex", clients[Random.Range(0, clients.Count)]);

        // client approach
        yield return DelayWithCallbacks(clientMovementTime,
                                (progress) =>
                                {
                                    client.SetPositionAndRotation(client.position.WithZ(clientPositionZ.Evaluate(progress)),
                                                                  Quaternion.Euler(client.rotation.eulerAngles.WithZ(clientRotationZ.Evaluate(progress))));
                                    client.localScale = clientScale.Evaluate(progress).ToVector3();
                                },
                                () => { });

        // look at
        yield return new WaitForSeconds(initialDelay);

        // phone is spawned
        var phonePrefab = phones[Random.Range(0, phones.Count)];
        _currentPhone = Pool.Instance.Spawn(phonePrefab.gameObject, phonePoint.position).GetComponent<Phone>();

        // look at phone
        introCamera.SetActive(false);

        yield return new WaitForSeconds(cameraChangeTime);

        // screen protector sequence
        screenProtector.transform.position = screenProtectorStart.position;
        screenProtectorMaterial.SetFloat(ProtectorApplicationProgressParameterID, 0f);

        screenProtector.transform.localScale = _currentPhone.GetProtectorScale;

        scraper.enabled = true;

        // flies to the corner of the phone
        yield return DelayWithCallbacks(protectorFlightTime,
                                        (progress) =>
                                        {
                                            screenProtector.transform.position = Vector3.Lerp(screenProtectorStart.position,
                                                                                              _currentPhone.ProtectorMin.position,
                                                                                              progress);
                                        },
                                        () => { screenProtector.transform.position = _currentPhone.ProtectorMin.position; });

        // bubbles are spawned
        bubblesController.SpawnBubblesForPhone(this, _currentPhone);

        // protector application vertex animation is played
        yield return DelayWithCallbacks(protectorApplicationTime,
                                        (progress) => { screenProtectorMaterial.SetFloat(ProtectorApplicationProgressParameterID, progress); },
                                        () => { screenProtectorMaterial.SetFloat(ProtectorApplicationProgressParameterID, 1f); });
    }

    public void OnAllBubblesBurst()
    {
        if (_cutsceneCoroutine != null)
            StopCoroutine(_cutsceneCoroutine);

        _cutsceneCoroutine = StartCoroutine(GameplayWrapCutscene());
    }

    private IEnumerator GameplayWrapCutscene()
    {
        // admiring the work
        yield return new WaitForSeconds(1f);

        // looking at client
        introCamera.SetActive(true);
        scraper.enabled = false;

        yield return new WaitForSeconds(1f);

        // phone despawned and reset
        Pool.Instance.Despawn(_currentPhone.gameObject);
        _currentPhone = null;

        screenProtector.transform.position = screenProtectorsBox.position;

        // client leaves
        yield return DelayWithCallbacks(clientMovementTime,
                        (progress) =>
                        {
                            client.SetPositionAndRotation(client.position.WithZ(clientPositionZ.Evaluate(1 - progress)),
                                                          Quaternion.Euler(client.rotation.eulerAngles.WithZ(clientRotationZ.Evaluate(1 - progress))));
                            client.localScale = clientScale.Evaluate(1 - progress).ToVector3();
                        },
                        () => { });

        // repeat gameplay
        yield return GameplayStartCutscene();
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
