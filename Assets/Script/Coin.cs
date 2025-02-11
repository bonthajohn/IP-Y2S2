using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool isMovingToPiggyBank = false;
    private Transform targetSlot;
    private float moveSpeed = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PiggyBank"))
        {
            PiggyBankCounter piggyBank = other.GetComponent<PiggyBankCounter>();
            if (piggyBank != null)
            {
                targetSlot = piggyBank.coinInsertPoint; // Get the coin slot position
                isMovingToPiggyBank = true;
            }
        }
    }

    private void Update()
    {
        if (isMovingToPiggyBank && targetSlot != null)
        {
            // Move the coin smoothly to the insert point
            transform.position = Vector3.MoveTowards(transform.position, targetSlot.position, moveSpeed * Time.deltaTime);

            // Check if the coin has reached the slot
            if (Vector3.Distance(transform.position, targetSlot.position) < 0.01f)
            {
                isMovingToPiggyBank = false;
                PiggyBankCounter piggyBank = targetSlot.GetComponentInParent<PiggyBankCounter>();
                if (piggyBank != null)
                {
                    piggyBank.AddCoin();
                }

                Destroy(gameObject); // Remove coin after insertion
            }
        }
    }
}
