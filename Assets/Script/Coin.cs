using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            ChestCounter cubeCounter = other.GetComponent<ChestCounter>();
            if (cubeCounter != null)
            {
                cubeCounter.AddCoin();
                Destroy(gameObject);
            }
        }
    }
}
