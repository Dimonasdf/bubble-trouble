using System.Collections.Generic;
using UnityEngine;

public class BubblesController : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject bubbleBurstPrefab;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Vector2Int numberOfBubbles;
    [SerializeField] private Vector2 bubbleSize;

    [SerializeField] private Phone _currentPhone;

    private readonly List<Bubble> _activeBubbles = new();

    private void OnEnable()
    {
        SpawnBubblesForPhone(_currentPhone);
    }

    public void SpawnBubblesForPhone(Phone phone)
    {
        var protectorMin = phone.ProtectorMin.position;
        var protectorMax = phone.ProtectorMax.position;

        var areaSize = protectorMax - protectorMin;

        var bubblesNum = Random.Range(numberOfBubbles.x, numberOfBubbles.y);
        for (int i = 0; i < bubblesNum; i++)
        {
            var position = protectorMin + new Vector3(Random.Range(0, areaSize.x), 0f, Random.Range(0, areaSize.z));
            var size = Random.Range(bubbleSize.x, bubbleSize.y);
            SpawnBubble(position, size);
        }
    }

    public void SpawnBubble(Vector3 position, float size)
    {
        var bubbleInstance = Pool.Instance.Spawn(bubblePrefab, position);
        bubbleInstance.transform.SetParent(transform);

        var bubbleComponent = bubbleInstance.GetComponent<Bubble>();
        bubbleComponent.Setup(size, this, () => { _activeBubbles.Remove(bubbleComponent); });
        _activeBubbles.Add(bubbleComponent);
    }

    private void FixedUpdate()
    {
        var protectorMin = _currentPhone.ProtectorMin.position;
        var protectorMax = _currentPhone.ProtectorMax.position;

        for (int i = _activeBubbles.Count - 1; i >= 0; i--)
        {
            var bubblePosition = _activeBubbles[i].transform.position;
            var bubbleSize3D = Vector3.one * _activeBubbles[i].Size / 2f;
            var bubblePositionPlus = bubblePosition + bubbleSize3D;
            var bubblePositionMinus = bubblePosition - bubbleSize3D;

            if (bubblePositionPlus.x <= protectorMin.x ||
                bubblePositionPlus.z <= protectorMin.z ||
                bubblePositionMinus.x >= protectorMax.x ||
                bubblePositionMinus.z >= protectorMax.z)
            {
                PlaySuccessFX(_activeBubbles[i].transform.position);
                Pool.Instance.Despawn(_activeBubbles[i].gameObject);
            }
        }
    }

    private void PlaySuccessFX(Vector3 position)
    {
        Pool.Instance.Spawn(bubbleBurstPrefab, position);
        PlayBurstSFX();
    }

    public void PlayBurstSFX()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }

    private void OnDisable()
    {
        for (int i = _activeBubbles.Count - 1; i >= 0; i--)
        {
            if (_activeBubbles[i].gameObject.activeSelf)
                Pool.Instance.Despawn(_activeBubbles[i].gameObject);
        }

        _activeBubbles.Clear();
    }
}