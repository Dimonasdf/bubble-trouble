using UnityEngine;

public class Phone : MonoBehaviour
{
    [SerializeField] private Transform protectorMin;
    [SerializeField] private Transform protectorMax;

    public Transform ProtectorMin => protectorMin;

    public Vector3 GetProtectorScale
    {
        get
        {
            var scale = protectorMax.localPosition - protectorMin.localPosition;
            scale.x *= transform.localScale.x;
            scale.y = 1f;
            scale.z *= transform.localScale.z;

            return scale;
        }
    }
}
