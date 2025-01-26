using UnityEngine;

public class BubbleSuccessFX : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;


    private void OnEnable()
    {
        particleSystem.Play();
    }

    private void Update()
    {
        if (!particleSystem.IsAlive())
        {
            particleSystem.Stop();
            Pool.Instance.Despawn(gameObject);
        }
    }
}
