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
            return GameSpeed * defaultIncPlayerSpeed;
        }
    }
    public float PlayerSprintSpeed
    {
        get
        {
            return Mathf.Max(0.18f, Mathf.Min(GameSpeed * defaultIncSprintSpeed, 0.35f));
        }
    }

    private const float defaultIncPlayerSpeed = 0.01f;
    private const float defaultIncSprintSpeed = 0.02f;
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

