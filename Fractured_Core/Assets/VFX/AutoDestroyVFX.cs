using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    //how long this vfx object stays alive
    public float lifeTime = 0.2f;

    void Start()
    {
        //destroy this GameObject after lifeTime seconds
        Destroy(gameObject, lifeTime);
    }
}
