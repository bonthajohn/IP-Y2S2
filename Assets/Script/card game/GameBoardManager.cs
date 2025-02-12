using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Add this namespace

public class GameBoardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Material blankMaterial;
    public TextMeshProUGUI congratulationsText; // Reference to the TextMeshPro UI text

    private List<Material> imageMaterials = new List<Material>();
    private List<CardController> revealedCards = new List<CardController>();
    private int totalCards; // Total cards in the game
    private int remainingCards; // Track remaining cards

    private void Start()
    {
        LoadCardImages();
        SpawnCards();
        totalCards = 16; // 4x4 grid, so 16 cards total
        remainingCards = totalCards; // Set remaining cards to total at the start
        congratulationsText.gameObject.SetActive(false); // Hide text initially
    }

    private void LoadCardImages()
    {
        Object[] textures = Resources.LoadAll("CardImages", typeof(Texture2D));

        List<Texture2D> textureList = new List<Texture2D>();
        foreach (var tex in textures)
        {
            textureList.Add((Texture2D)tex);
        }

        textureList.Shuffle(); // Randomize images

        for (int i = 0; i < 8; i++) // We need 8 pairs
        {
            // 🔹 Fix: Explicitly set Standard shader to avoid pink issue
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.mainTexture = textureList[i];
            imageMaterials.Add(mat);
            imageMaterials.Add(mat); // Add twice for matching pairs
        }

        imageMaterials.Shuffle(); // Shuffle pairs
    }

    private void SpawnCards()
    {
        GameObject spawnArea = GameObject.Find("CardSpawnArea"); // Find the empty GameObject
        if (spawnArea == null)
        {
            Debug.LogError("CardSpawnArea not found in the scene!");
            return;
        }

        int index = 0;
        float spacing = 0.15f; // Space between cards

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                Vector3 position = spawnArea.transform.position + new Vector3(x * spacing, 0, y * spacing);
                GameObject card = Instantiate(cardPrefab, position, Quaternion.identity, spawnArea.transform);
                CardController cardScript = card.GetComponent<CardController>();
                cardScript.AssignImageMaterial(imageMaterials[index]);
                cardScript.blankMaterial = blankMaterial;
                index++;
            }
        }
    }

    public void CardRevealed(CardController card)
    {
        revealedCards.Add(card);
        if (revealedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (revealedCards[0].GetComponent<Renderer>().material.mainTexture == revealedCards[1].GetComponent<Renderer>().material.mainTexture)
        {
            revealedCards[0].DestroyCard();
            revealedCards[1].DestroyCard();
            remainingCards -= 2; // Decrease remaining cards count after a match
        }
        else
        {
            revealedCards[0].HideCard();
            revealedCards[1].HideCard();
        }

        revealedCards.Clear();

        // Check if all cards are destroyed
        if (remainingCards == 0)
        {
            ShowCongratulations();
        }
    }

    private void ShowCongratulations()
    {
        congratulationsText.gameObject.SetActive(true); // Show the text
        congratulationsText.text = "Congratulations! You've matched all the cards!";
    }
}

// Shuffle Extension
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
