using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public ImageSynthesis synth;
    public GameObject[] prefabs;
    public int maxObjects = 25;
    public int count = 1;
    public float waitTime = 20f; // Time to wait in seconds
    public int sceneNumber = 0; // Starting scene number
    public int maxScenes = 10; // Maximum number of scenes to generate
    public float delayTime = 5f;
    
    // Rotation range parameters (ADDED)
    public Vector3 minRotation = new Vector3(-180f, 0f, -180f);
    public Vector3 maxRotation = new Vector3(-180f, 0f, -180f);
    public bool useRandomRotation = false; // Toggle between Random.rotation and ranged rotation
    
    private int b = 0;
    public float generationTime = 40f; // Time between prefab generation
    private float timeSinceGeneration; // Time since last prefab generation
    public static int OnEnableCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRandom();
        
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Matrix4x4 projectionMatrix = mainCamera.projectionMatrix;

            // Extract the 3x3 intrinsic matrix
            Matrix4x4 intrinsicMatrix = new Matrix4x4(
                projectionMatrix.GetRow(0),
                projectionMatrix.GetRow(1),
                projectionMatrix.GetRow(2),
                new Vector4(0, 0, 0, 1)
            );

            // Log the intrinsic matrix
            Debug.Log("Intrinsic Matrix:");
            Debug.Log(intrinsicMatrix);
        }
        else
        {
            Debug.LogError("No main camera found.");
        }

        if (mainCamera != null)
        {
            // Get camera position and rotation
            Vector3 cameraPosition = mainCamera.transform.position;
            Quaternion cameraRotation = mainCamera.transform.rotation;

            // Create the extrinsic matrix
            Matrix4x4 extrinsicMatrix = Matrix4x4.TRS(cameraPosition, cameraRotation, Vector3.one);

            // Log the extrinsic matrix
            Debug.Log("Extrinsic Matrix:");
            Debug.Log(extrinsicMatrix);
        }
        else
        {
            Debug.LogError("No main camera found.");
        }
    }

    private void OnEnable()
    {
        OnEnableCount++;
        Debug.Log("OnEnable has been called " + OnEnableCount + " times.");
    }

    void Update()
    {
        timeSinceGeneration += Time.deltaTime;

        // If it's been long enough since the last prefab generation, generate a new one
        if (timeSinceGeneration >= generationTime)
        {
            timeSinceGeneration = 2f; // Reset the timer 
            string filename = $"{OnEnableCount}";
            b++;
            synth.Save(filename, 640, 480, "C:/Users/dniti/OneDrive/Desktop/dataset generation");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            Invoke("GenerateRandom", delayTime);
        }
    }

    // Helper method to generate random rotation within specified ranges (ADDED)
    Quaternion GenerateRandomRotationInRange()
    {
        float rotX = Random.Range(minRotation.x, maxRotation.x);
        float rotY = Random.Range(minRotation.y, maxRotation.y);
        float rotZ = Random.Range(minRotation.z, maxRotation.z);
        
        return Quaternion.Euler(rotX, rotY, rotZ);
    }

    void GenerateRandom()
    {
        for (int i = 0; i < maxObjects; i++)
        {
            int prefabIndx = Random.Range(0, prefabs.Length);
            GameObject prefab = prefabs[0];
            
            // Position
            float newX = Random.Range(-100.0f, 101.0f);
            float newY = Random.Range(100.0f,250.0f);
            float newZ = Random.Range(0.0f,10.0f);
            Vector3 newPos = new Vector3(newX, newY, newZ);

            // Rotation - using the new range system (MODIFIED)
            Quaternion newRot;
            if (useRandomRotation)
            {
                // Original random rotation
                newRot = Random.rotation;
            }
            else
            {
                // New ranged rotation
                newRot = GenerateRandomRotationInRange();
            }
            
            var newObj = Instantiate(prefab, newPos, newRot);

            // Scale
            float sx = Random.Range(1.0f, 1.0f);
            Vector3 newScale = new Vector3(sx, sx, sx);
            newObj.transform.localScale = newScale;
            
            // Color
            float newR = Random.Range(0.0f, 1.0f);
            float newG = Random.Range(0.0f, 1.0f);
            float newB = Random.Range(0.0f, 1.0f);
        }
    }
}

public class CameraIntrinsicMatrix : MonoBehaviour
{
    void Start()
    {
        Camera mainCamera = Camera.main;
        Matrix4x4 intrinsicMatrix = mainCamera.projectionMatrix;
        
        Debug.Log("Camera Intrinsic Matrix:");
        Debug.Log(intrinsicMatrix);
    }
}