using UnityEngine;

public class GlobalController : MonoBehaviour
{
    public int NbColors { get; set; }

    public Sprite[] colors;
    public int GameSpeed { get; set; }

    public float PlayerSpeed
    {
        get
        {
            return GameSpeed * defaultPlayerSpeed;
        }
    }
    public float PlayerSprintSpeed
    {
        get
        {
            return GameSpeed * defaultSprintSpeed;
        }
    }

    private const float defaultPlayerSpeed = 0.02f;
    private const float defaultSprintSpeed = 0.06f;
    private static GameObject globalControllerInstance = null;

    void Awake()
    {
        if (globalControllerInstance != null)
        {
            DestroyObject(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
            globalControllerInstance = this.gameObject;
        }
        NbColors = 4;
    }
    
    public int GetRandomColor()
    {
        return Random.Range(0, NbColors);
    }
}

