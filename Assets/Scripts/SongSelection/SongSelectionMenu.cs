using UnityEngine;
using TMPro;
using System.Collections;

public class SongSelectionMenu : MonoBehaviour
{
    [SerializeField] BoxCollider selectionArea = default;
    [SerializeField] TMP_Text selectionText = default;

    [SerializeField] AudioSource audioSourceMain;

    public GameObject panel;
    public int mainMenu;
    public SongObject[] songs;
    public GameObject [] panels;

    private Vector3 scaleChange;
    private GameObject selectedPanel;
    private AudioClip selectedSongMusic;
    private AudioSource audioSource;

    private bool panelIsActive = false;

    private IEnumerator switchSong;
    private Collider[] collidersBuffer = new Collider[12];


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();


        for(int i = 0; i < panels.Length; i++){
            GameObject currentPanel = panels[i];

            if(i <= songs.Length){ 
                SongObject currentSong = songs[i];
                currentPanel.GetComponent<SongPanel>().setSong(currentSong);
            }
        }

    }

    private void OnEnable() {
        audioSourceMain.Pause();    
    }

    void FixedUpdate()
    {
        if (selectionArea != default) //Kollar ifall default värdet överensstämmer
        {
            CheckSelection();
        }

        revertSelection();
    }

    private void CheckSelection()
    {
        if (!Physics.CheckBox(selectionArea.transform.position, selectionArea.size, selectionArea.transform.rotation)) // En snabb och billig koll ifall n�gon collider befinner sig i selectionArea
            return;

        string displayText = "Your Selected Song Is: ";
            
        Physics.OverlapBoxNonAlloc(selectionArea.transform.position, selectionArea.size, collidersBuffer, selectionArea.transform.rotation);
        for (int i = 0; i < collidersBuffer.Length; i++)
        {
            if (collidersBuffer[i] == selectionArea || collidersBuffer[i] == null) // Hoppar �ver dessa
                continue;
            displayText += collidersBuffer[i].gameObject.name; // H�r h�mtar man allt man vill fr�n objektet, just nu �r det bara namnet f�r GameObjectet
                
            selectedPanel = collidersBuffer[i].gameObject;

            collidersBuffer[i].gameObject.transform.localScale = new Vector3(1.25f, 1.25f, collidersBuffer[i].gameObject.transform.localScale.z);
            
            selectedSongMusic = collidersBuffer[i].gameObject.GetComponent<SongPanel>().songObject.song;
            

            if(collidersBuffer[i].gameObject != selectionArea && !audioSource.isPlaying){
                panelIsActive = true;
            }else{
                panelIsActive = false;
            }
            
            if(audioSource.clip != selectedSongMusic && !audioSource.isPlaying){
                StartCoroutine(SwitchSongRoutine(selectedSongMusic));
                Debug.Log("Yay");
            }

         
            
            
            
            

            //collidersBuffer[i].gameObject.GetComponentInParent<>

            //float defaultScale = hitColliders[i].transform.localScale.z;
            //scaleChange = new Vector3(1.25f, 1.25f, defaultScale);

            //if(hitColliders[i] != selectionArea){
            //    hitColliders[i].gameObject.transform.localScale = scaleChange;
            //    audioSource.PlayOneShot(selectedSongMusic);
            //}
        }

        selectionText.text = displayText;

    }
    private IEnumerator SwitchSongRoutine(AudioClip song){

        new WaitForSeconds(1);
        while(panelIsActive){
            
            audioSource.time = 13;
            audioSource.PlayOneShot(selectedSongMusic);
            yield return new WaitForSeconds(4);
        }


        //De gör något varje frame när man kallar på dem
        //Beroende på vad man yieldar 
        //yield null - kommer den kalla på varje frame, pausar ingenting
        //yield 0.005  pausar i 5 sekunder. tills while loopen fortsätter igen
        //audioClip.play();
        //yield return 4 seconds
        //audiosource.time = 13; //sekund 13 av audioklippet

        //changeIconRoutine = changeIconeSizeRoutine

        //Spara routinen och andvänd stopRoutine(changeIconeSizeRoutine)

        //Create loop that lowers the volume. Audiosource.volume -= 0.1f;
        //yield return 0.005;

    }
    private void revertSelection()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].GetInstanceID() != selectedPanel.GetInstanceID() && panels[i].transform.localScale != Vector3.one)
            {
                panels[i].transform.localScale = Vector3.one;
                audioSource.Stop();
            }
        }
    }

    public void SelectSong()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    public void StartGame(string gameSceneName)
    {
        Debug.Log("Started game");
        SceneHandler.Instance.ChangeScene(gameSceneName);
    }

    public void Back()
    {
        if(panel.activeInHierarchy){
            panel.SetActive(false);
        }
    }
}

