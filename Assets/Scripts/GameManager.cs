using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<CardController> cards; // List of all cards
    public List<string> imageUrls; // URLs of images in the Supabase bucket

    void Start()
    {
        ShuffleCards();
    }

    void ShuffleCards()
    {
        // Shuffle the imageUrls list
        List<string> shuffledImages = new List<string>(imageUrls);
        for (int i = 0; i < shuffledImages.Count; i++)
        {
            string temp = shuffledImages[i];
            int randomIndex = Random.Range(i, shuffledImages.Count);
            shuffledImages[i] = shuffledImages[randomIndex];
            shuffledImages[randomIndex] = temp;
        }

        // Assign shuffled URLs to the cards
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].imageUrl = shuffledImages[i];
        }
    }
}
