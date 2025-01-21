using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class PeppermintOutcome : MonoBehaviour
{
    public string supabaseUrl = "https://<your-project-ref>.supabase.co"; // Replace with your Supabase URL
    public string bucketName = "<your-bucket-name>"; // Replace with your Supabase bucket name
    public string[] imagePaths; // Array of image paths inside the "Peppermint Puzzle Path" folder
    public string supabaseAnonKey = "<your-anon-key>"; // Replace with your Supabase anon key
    public Renderer targetRenderer; // The renderer to which the texture will be applied

    private async void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer is not assigned.");
            return;
        }

        // Randomly pick an image from the array
        string randomImagePath = GetRandomImagePath();

        // Ensure the image paths are under the "Peppermint Puzzle Path" folder
        string fullImagePath = $"Peppermint Puzzle Path/{randomImagePath}";

        string imageUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{fullImagePath}";

        try
        {
            Texture2D texture = await GetTextureFromURL(imageUrl);
            if (texture != null)
            {
                targetRenderer.material.mainTexture = texture;
                Debug.Log("Texture applied successfully.");
            }
            else
            {
                Debug.LogError("Failed to load texture.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error downloading texture: {e.Message}");
        }
    }

    // Method to randomly select an image path from the list
    private string GetRandomImagePath()
    {
        if (imagePaths.Length == 0)
        {
            Debug.LogError("No image paths provided.");
            return null;
        }

        int randomIndex = Random.Range(0, imagePaths.Length);
        return imagePaths[randomIndex];
    }

    private async Task<Texture2D> GetTextureFromURL(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            // Add the authorization header if needed (using anon key for public access)
            request.SetRequestHeader("Authorization", $"Bearer {supabaseAnonKey}");

            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield(); // Yield until the operation is complete
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                return DownloadHandlerTexture.GetContent(request);
            }
            else
            {
                Debug.LogError($"Error in UnityWebRequest: {request.error}");
                return null;
            }
        }
    }
}
