using SeolMJ;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SSController : MonoBehaviour
{
    public CurSelected cs;
    public AudioSource previewAudioSource;
    
    [Space(5)]
    public Sprite defaultCoverImage;
    
    #region UI
    [Space(10), Header("UI Elements")]
    public CanvasGroup infoPanel;
    public Image     coverImage;
    public TMP_Text  titleText;
    public TMP_Text  artistText;
    public TMP_Text  highScoreText;
    public TMP_Text  bpmText;
    public DiffViewer diffView;
    #endregion

    private void Start()
    {
        if (cs == null) {Debug.LogError("No CurSelected found in scene");}
        else {cs.OnTrackSelected += UpdateUI; cs.OnTrackDeselected += () => UpdateUI();}
        cs.ClearTrack();
    }
    
    private void Update()
    {
        // 우클릭시 deselect
        if (Input.GetMouseButtonDown(1))
        {
            cs.ClearTrack();
        }
    }
    
    private void UpdateUI(Track t = null)
    {
        if (cs.HasTrack() is false)
        {
             infoPanel.alpha = 0;
            previewAudioSource.Stop();
            
            return;
        }
        
        coverImage.sprite = cs.GetTrack().Cover;
        if (coverImage.sprite == null) coverImage.sprite = defaultCoverImage;
        
        infoPanel.alpha = 1;
        Track track = cs.GetTrack();
        titleText.text = track.Name;
        artistText.text = track.Artist;
        bpmText.text = "BPM: " + track.BPM.ToString("F0");
        highScoreText.text = "High Score: " + ScoreSystem.GetHighScore(track);
        
        // set audio
        previewAudioSource.clip = track.Clip;
        
        // Highlight 초로 변경
        previewAudioSource.time = track.PreviewStart;
        previewAudioSource.Play();
        
        diffView.SetDifficulty(track.Difficulty);
    }
}
