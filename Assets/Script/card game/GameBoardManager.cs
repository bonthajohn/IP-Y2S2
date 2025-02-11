using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Material blankMaterial;

    private List<Material> imageMaterials = new List<Material>();
    private List<CardController> revealedCards = new List<CardController>();

    private void Start()
    {
        LoadCardImages();
        SpawnCards();
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

        if (revealedCards[0].GetComponent<Renderer>().material.mainTexture ==
            revealedCards[1].GetComponent<Renderer>().material.mainTexture)
        {
            revealedCards[0].DestroyCard();
            revealedCards[1].DestroyCard();
        }
        else
        {
            revealedCards[0].HideCard();
            revealedCards[1].HideCard();
        }

        revealedCards.Clear();
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
