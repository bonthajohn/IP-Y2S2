using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.Interaction.Toolkit;

public class CardManager : MonoBehaviour
{
    public GameObject[] cards; // Assign the 16 card GameObjects here in the Inspector
    private List<Texture2D> cardImages = new List<Texture2D>(); // Store downloaded textures
    private Dictionary<GameObject, Texture2D> cardTextureMap = new Dictionary<GameObject, Texture2D>();

    private GameObject firstCard = null;
    private GameObject secondCard = null;

    private string supabaseUrl = "https://supabase.com/dashboard/project/kcdvqhyqtlmebgnunaov/storage/buckets/Card%20Images";
    private string[] imageNames = { "image1.jpg", "image2.jpg", "image3.jpg", "image4.jpg", "image5.jpeg", "image6.jpg", "image7.jpg", "image8.jpeg" };

    void Start()
    {
        StartCoroutine(DownloadImages());
    }

    IEnumerator DownloadImages()
    {
        // Download images from Supabase and store them
        foreach (string imageName in imageNames)
        {
            string url = supabaseUrl + imageName;
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                cardImages.Add(texture);
            }
            else
            {
                Debug.LogError($"Failed to download image: {url}");
            }
        }

        // Duplicate images for pairs and randomize
        List<Texture2D> randomizedImages = new List<Texture2D>(cardImages);
        randomizedImages.AddRange(cardImages);
        Shuffle(randomizedImages);

        // Assign textures to cards
        for (int i = 0; i < cards.Length; i++)
        {
            cardTextureMap[cards[i]] = randomizedImages[i];
        }
    }

    public void FlipCard(GameObject card)
    {
        if (firstCard == null)
        {
            firstCard = card;
            ShowCardImage(card);
        }
        else if (secondCard == null)
        {
            secondCard = card;
            ShowCardImage(card);

            StartCoroutine(CheckMatch());
        }
    }

    void ShowCardImage(GameObject card)
    {
        Renderer renderer = card.GetComponent<Renderer>();
        renderer.material.mainTexture = cardTextureMap[card];
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (cardTextureMap[firstCard] == cardTextureMap[secondCard])
        {
            // Match! Remove cards
            firstCard.SetActive(false);
            secondCard.SetActive(false);
        }
        else
        {
            // No match, flip cards back
            ResetCard(firstCard);
            ResetCard(secondCard);
        }

        firstCard = null;
        secondCard = null;
    }

    void ResetCard(GameObject card)
    {
        Renderer renderer = card.GetComponent<Renderer>();
        renderer.material = Resources.Load<Material>("BlankMaterial");
    }

    void Shuffle(List<Texture2D> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Texture2D temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
