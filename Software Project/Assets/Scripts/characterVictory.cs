using UnityEngine;

public class characterVictory : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "chip")
        {
            Destroy(col.gameObject);
        }
    }
}