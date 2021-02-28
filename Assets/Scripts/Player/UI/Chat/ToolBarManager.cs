using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolBarManager : MonoBehaviour
{

    public GameObject OpenEmoji;
    public GameObject OpenEmojiContainer;
    [SerializeField]
    private Animator animator;
    public void OpenEmojiBar()
    {
        if (!OpenEmojiContainer.activeInHierarchy)
        {
            OpenEmojiContainer.SetActive(true);
        }
        else if (OpenEmojiContainer.activeInHierarchy)
        {
            OpenEmojiContainer.SetActive(false);
        }
    }

    public void changeScene()
    {
        SceneManager.LoadScene(3);
    }

    public void animationPlay(string animationName)
    {
        animator.SetBool(animationName, true);
    }
    public void animationStop(string animationName)
    {
        animator.SetBool(animationName, false);
    }
}
