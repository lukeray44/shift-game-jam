using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
   public TextMeshProUGUI[] Text;

   public AudioClip[] SFX;

    public AnimationCurve Ease;
    void Start()
    {
        Coroutines.RunCoroutine(Seq());
        LeanTween.reset();
    }

    IEnumerator<float> Seq()
    {
        GetComponent<VerticalLayoutGroup>().enabled = false;
        foreach (var t in Text) t.gameObject.SetActive(false);
        yield return Coroutines.WaitForSeconds(0.3f);
        int ind = 0;
        foreach (var t in Text)
        {
            yield return Coroutines.WaitForSeconds(0.5f);

            t.gameObject.SetActive(true);
            Sound.Play(SFX[ind]);
            ind++;
        }
        
        yield return Coroutines.WaitForSeconds(2f);

        foreach (var t in Text)
        {
            
            
            t.transform.LeanMove(t.transform.position + Vector3.right * 15 * ((ind % 2 == 0) ? 1 : -1), 0.8f).setEase(Ease);
            ind++;

            yield return Coroutines.WaitForSeconds(0.1f);
        }

        yield return Coroutines.WaitForSeconds(1.2f);

        SceneManager.LoadScene("Title");
    }
}
