using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] private SoundManager soundManager;
    private void Start()
    {
        Debug.Log("I do update");
        soundManager.PlaySFX("Main Beat");
    }
}
