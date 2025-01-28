using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CardInteraction : MonoBehaviour
{
    private CardManager cardManager;
    private bool isFlipped = false;

    void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
    }

    public void OnSelectEnter(XRBaseInteractor interactor)
    {
        if (!isFlipped)
        {
            isFlipped = true;
            cardManager.FlipCard(gameObject);
        }
    }
}
