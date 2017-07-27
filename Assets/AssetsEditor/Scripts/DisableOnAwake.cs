using System.Collections;
using UnityEngine;

public class DisableOnAwake : MonoBehaviour
{
    [Tooltip("Destroy the object instead of desactivating it.")]
    public bool orDestroyIt = false;


    private void Awake() {

            if (orDestroyIt)
            {
                Destroy(gameObject);

            } else
            {
                gameObject.SetActive(false);
            }

    }


}