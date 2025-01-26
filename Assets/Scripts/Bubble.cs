using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bubble : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private SphereCollider sphereCollider;

    [SerializeField, Range(0f, 1f)] private float splitChance;
    private float _ownMaxSize;

    public bool IsEngulfed;

    public float Size => _size;
    private float _size;

    private BubblesController Controller;
    private Action OnDespawnAction;

    private void OnEnable()
    {
        IsEngulfed = false;
    }

    public void Setup(float size, BubblesController controller, Action onDespawnCallback)
    {
        SetSize(size);
        Controller = controller;
        OnDespawnAction = onDespawnCallback;
    }

    private void SetSize(float size)
    {
        _size = size;
        _ownMaxSize = Random.Range(_size, _size * 2f);

        sphereCollider.radius = 0.5f;

        transform.localScale = new Vector3(size, 0.01f, size);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsEngulfed)
            return;

        var otherLayer = collision.gameObject.layer;
        if (otherLayer.Equals(Consts.Bubbles))
        {
            var otherBubble = collision.gameObject.GetComponent<Bubble>();
            TryEngulf(otherBubble);
        }
        else if (otherLayer.Equals(Consts.Scraper) && Random.Range(0f, 1f) < splitChance)
        {
            // prevent instant merge
            IsEngulfed = true;
            SetSize(_size / 2f);
            Controller.SpawnBubble(transform.position.RandomAround(0.01f), _size / 2f);
        }
    }

    private bool TryEngulf(Bubble otherBubble)
    {
        var newSize = _size + otherBubble.Size;
        if (newSize > _ownMaxSize)
            return false;

        otherBubble.IsEngulfed = true;
        SetSize(newSize);
        Pool.Instance.Despawn(otherBubble.gameObject);

        return true;
    }

    private void OnDisable()
    {
        OnDespawnAction?.Invoke();
    }
}