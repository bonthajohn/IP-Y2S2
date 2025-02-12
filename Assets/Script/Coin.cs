using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Coin : MonoBehaviour
{
    private bool isMovingToPiggyBank = false;
    private Transform targetSlot;
    private float moveSpeed = 5f;
    private PiggyBankCounter assignedPiggyBank;
    private bool hasBeenCounted = false;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!args.isCanceled) // Only start moving the coin if the release wasn't canceled
        {
            isMovingToPiggyBank = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMovingToPiggyBank) return;

        if (other.CompareTag("PiggyBank"))
        {
            PiggyBankCounter piggyBank = other.GetComponent<PiggyBankCounter>();
            if (piggyBank != null)
            {
                targetSlot = piggyBank.coinInsertPoint;
                assignedPiggyBank = piggyBank;
                isMovingToPiggyBank = true;
            }
        }
    }

    private void Update()
    {
        if (isMovingToPiggyBank && targetSlot != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetSlot.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetSlot.position) < 0.01f)
            {
                isMovingToPiggyBank = false;

                if (!hasBeenCounted && assignedPiggyBank != null)
                {
                    assignedPiggyBank.AddCoin();
                    hasBeenCounted = true;
                }

                Destroy(gameObject); // Destroy the coin after insertion
            }
        }
    }
}
