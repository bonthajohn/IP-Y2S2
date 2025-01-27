using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CardController : MonoBehaviour
{
    public string imageUrl;  // URL of the image to be retrieved
    private Renderer cardRenderer;
    private bool isFlipped = false;
    private Material frontMaterial;
    private Material backMaterial;

    private static List<CardController> flippedCards = new List<CardController>();

    void Start()
    {
        cardRenderer = GetComponent<Renderer>();
        backMaterial = cardRenderer.material; // Blank material
        frontMaterial = new Material(Shader.Find("Unlit/Texture")); // For displaying images
    }

    void OnEnable()
    {
        // Subscribe to the selectEntered event
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnCardClicked);
    }

    void OnDisable()
    {
        // Remove the listener correctly
        GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnCardClicked);
    }

    void OnCardClicked(SelectEnterEventArgs args)
    {
        if (isFlipped) return;

        // Flip the card
        isFlipped = true;
        cardRenderer.material = frontMaterial;

        // Fetch image from Supabase bucket
        StartCoroutine(FetchImage());

        flippedCards.Add(this);
        if (flippedCards.Count == 2)
        {
            Debug.Log("Checking for match...");
            CheckMatch();
        }
    }

    IEnumerator FetchImage()
    {
        UnityWebRequest request = UnityWebRequest.Get(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(request.downloadHandler.data); // Set the image
            frontMaterial.mainTexture = texture;
        }
        else
        {
            Debug.LogError("Error fetching image: " + request.error);
        }
    }

    void CheckMatch()
    {
        Debug.Log("Flipped cards count: " + flippedCards.Count);

        if (flippedCards[0].imageUrl == flippedCards[1].imageUrl)
        {
            Debug.Log("Match found!");
            // If match, remove cards (or hide them)
            Destroy(flippedCards[0].gameObject);
            Destroy(flippedCards[1].gameObject);
        }
        else
        {
            Debug.Log("No match, flipping back.");
            // If no match, flip them back
            StartCoroutine(FlipBack());
        }

        flippedCards.Clear();
    }

    IEnumerator FlipBack()
    {
        // Debug to ensure this is being called
        Debug.Log("Starting flip back...");
        yield return new WaitForSeconds(1);

        foreach (var card in flippedCards)
        {
            card.cardRenderer.material = card.backMaterial;
            card.isFlipped = false;
            // Debug for each card being reset
            Debug.Log("Card flipped back: " + card.gameObject.name);
        }
    }
}
