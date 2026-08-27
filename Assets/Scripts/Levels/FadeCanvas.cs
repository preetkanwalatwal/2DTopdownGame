// using UnityEngine;

// public class FadeCanvas : MonoBehaviour
// {
//     public static FadeCanvas Instance;
//     private Animator fadeAnim;

//    private void Awake()
//     {
//         if (Instance != null)
//         {
//             Destroy(gameObject);
//             return;
//         }

//         Instance = this;

//         DontDestroyOnLoad(gameObject);

//         fadeAnim = GetComponentInChildren<Animator>();

//         if (fadeAnim == null)
//         {
//             Debug.LogError("Fade Animator not found.");
//         }
//     }

//     public void FadeIn()
//     {
//         //Debug.Log($"FadeIn on {GetInstanceID()}");
//         //fadeAnim.ResetTrigger("FadeIn");
//         fadeAnim.SetTrigger("FadeIn");
//     }

//     public void FadeOut()
//     {
//        // Debug.Log($"FadeOut on {GetInstanceID()}");
//         //fadeAnim.ResetTrigger("FadeOut");
//         //Debug.Log("Fade out is reached");
//         fadeAnim.SetTrigger("FadeOut");
//     }
// }