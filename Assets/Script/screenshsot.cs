using UnityEngine;
using System;
using System.IO;

public class CaptureController : MonoBehaviour
{
    void Update()
    {
        // Check if the "P" key is pressed
        if (Input.GetKeyDown("p"))
        {
            // Call the Capture function from the I360Render class
            byte[] capturedImage = I360Render.Capture(8192);
            Debug.Log(capturedImage);
            WriteToFile(capturedImage);


            // You can now use the 'capturedImage' byte array as needed.
            // For example, you can save it to a file or process it further.
        }
    }

    public void WriteToFile(byte[] imageName)
    {
        File.WriteAllBytes("D:\\Example\\image.jpg", imageName);
    }
}