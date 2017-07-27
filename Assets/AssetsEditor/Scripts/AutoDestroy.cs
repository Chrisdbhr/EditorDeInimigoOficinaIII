using UnityEngine;

public class AutoDestroy : MonoBehaviour
{

    public float _cooldown = 1f;

    // Use this for initialization
    void Start() {
        DontDestroyOnLoad(gameObject);
        if (_cooldown > 0)
        {
            Destroy(gameObject, _cooldown);
        } else
        {
            Destroy(gameObject);
        }
    }

}
