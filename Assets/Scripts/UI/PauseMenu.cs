using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public UIButton Resume, Restart, Skip, Exit;

    public CanvasGroup Panel;

    public LevelManager Level;

    void Start()
    {
        Resume.Clicked += () => SetPaused(false);

        Restart.Clicked += () => LevelMeta.main.LoadLevel(LevelMeta.main.Cur);

        if (!LevelMeta.main.isNextLevel) Skip.gameObject.SetActive(false);
        else Skip.Clicked += () => LevelMeta.main.LoadLevel(LevelMeta.main.Cur + 1);

        Exit.Clicked += () => LevelMeta.main.LoadScene("Level Select");

        Level.HideHUD += () =>
        {
            if (isPaused) SetPaused(false);
          cantPause = true;  
        };
    }

    bool cantPause = false;

    bool isPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SetPaused(!isPaused);
    }
    void OnDestroy()
    {
        if (isPaused) Time.timeScale = 1;
    }
    public void SetPaused(bool paused)
    {
        if (cantPause) return;
        if (isPaused != paused)
        {
            isPaused = paused;
            Panel.gameObject.LeanCancel();

            if (paused)
            {
                Panel.gameObject.SetActive(true);
                Time.timeScale = 0;
                Panel.alpha = 0.5f;
                Panel.LeanAlpha(1, 0.08f).setEaseOutCubic().setIgnoreTimeScale(true).setOnComplete(() =>
                {
                    Resume.Active = Restart.Active = Skip.Active = Exit.Active = true;
                });

                Panel.transform.localScale = Vector3.one * 0.8f;
                Panel.gameObject.LeanScale(Vector3.one, 0.08f).setEaseOutCubic().setIgnoreTimeScale(true);
            }
            else
            {
                Resume.Active = Restart.Active = Skip.Active = Exit.Active = false;
                Time.timeScale = 1;
                Panel.LeanAlpha(0, 0.08f).setEaseOutCubic().setIgnoreTimeScale(true);
                Panel.gameObject.LeanScale(Vector3.one * 0.8f, 0.08f).setEaseOutCubic().setIgnoreTimeScale(true);
            }
        }
    }
}
