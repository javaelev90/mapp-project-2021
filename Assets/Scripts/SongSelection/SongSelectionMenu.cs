using UnityEngine;
using TMPro;

public class SongSelectionMenu : MonoBehaviour
{
    [SerializeField] BoxCollider selectionArea = default;
    //[SerializeField] BoxCollider switchSongArea = default;
    [SerializeField] TMP_Text selectionText = default;

    public GameObject panel;
    public int mainMenu;
    public SongObject[] songs;
    public GameObject [] panels;

    private GameObject selectedPanel = default;
    private AudioClip selectedSongMusic;
    //private AudioSource audioSource;
    private Collider[] collidersBuffer = new Collider[12];


    void Awake()
    {
        //audioSource = GetComponent<AudioSource>();

        for(int i = 0; i < panels.Length; i++){
            GameObject currentPanel = panels[i];

            if(i <= songs.Length){ 
                SongObject currentSong = songs[i];
                currentPanel.GetComponent<SongPanel>().setSong(currentSong);
                //selectedSongMusic = currentSong.song;
            }
        }

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

        if (Physics.OverlapBoxNonAlloc(selectionArea.transform.position, selectionArea.size, collidersBuffer, selectionArea.transform.rotation) > 0)
        {
            for (int i = 0; i < collidersBuffer.Length; i++)
            {
                if (collidersBuffer[i] == selectionArea || collidersBuffer[i] == null) // Hoppar �ver dessa
                    continue;

                displayText += collidersBuffer[i].gameObject.name; // H�r h�mtar man allt man vill fr�n objektet, just nu �r det bara namnet f�r GameObjectet

                selectedPanel = collidersBuffer[i].gameObject;
                selectedSongMusic = songs[i].song;

                collidersBuffer[i].gameObject.transform.localScale = new Vector3(1.25f, 1.25f, collidersBuffer[i].gameObject.transform.localScale.z);
                //float defaultScale = hitColliders[i].transform.localScale.z;
                //scaleChange = new Vector3(1.25f, 1.25f, defaultScale);

                //if(hitColliders[i] != selectionArea){
                //    hitColliders[i].gameObject.transform.localScale = scaleChange;
                //    audioSource.PlayOneShot(selectedSongMusic);
                //}
            }
        }

        selectionText.text = displayText;

    }
/*
    private void ChangeSong(){
        if(Physics.CheckBox(switchSongArea.transform.position, switchSongArea.size))
        {
            Collider[] hitColliders = Physics.OverlapBox(switchSongArea.transform.position, switchSongArea.size);


            //kolla ifall en panel har kolliderat
            //Kolla riktningen eller indexet för att avgöra vilket nästa element som kommer
        }

    }
    */

    private void revertSelection()
    {
        if (selectedPanel != default && panels.Length > 0)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                if (!panels[i].Equals(selectedPanel) && panels[i].transform.localScale != Vector3.one)
                {
                    panels[i].transform.localScale = Vector3.one;
                    //audioSource.Stop();
                }
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
