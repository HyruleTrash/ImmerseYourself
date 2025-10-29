using UnityEngine;

public class TerMaanTerOceaan : MiniGame
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Camera mainCam;
    [SerializeField]
    private GameObject background;
    
    private void Start()
    {
        gameId = MiniGames.TerMaanTerOceaanInDeRuimtVerVandaan;
    }

    public override void StartMiniGame(bool shouldShowControls)
    {
        base.StartMiniGame(shouldShowControls);
        Debug.Log($"Ter maan ter oceaan in de ruimte ver vandaan has been started.. showing controls: {shouldShowControls}");
        mainCam.enabled = false;
        cam.enabled = true;
        background.SetActive(false);
    }

    public override void MiniGameFinished()
    {
        cam.enabled = false;
        mainCam.enabled = true;
        background.SetActive(true);
        base.MiniGameFinished();
    }
}