using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CardController : MonoBehaviour
{
    public Renderer cardRenderer;
    public Material blankMaterial;
    private Material imageMaterial;
    private bool isRevealed = false;

    private GameBoardManager gameBoard;

    private void Start()
    {
        cardRenderer = GetComponent<Renderer>(); // Fix: Ensure this is set properly
        cardRenderer.material = blankMaterial;
        gameBoard = FindObjectOfType<GameBoardManager>();
    }


    public void AssignImageMaterial(Material mat)
    {
        imageMaterial = mat;
    }

    public void RevealCard()
    {
        if (!isRevealed)
        {
            isRevealed = true;
            cardRenderer.material = imageMaterial;
            gameBoard.CardRevealed(this);
        }
    }

    public void HideCard()
    {
        isRevealed = false;
        cardRenderer.material = blankMaterial;
    }

    public void DestroyCard()
    {
        Destroy(gameObject);
    }
}
