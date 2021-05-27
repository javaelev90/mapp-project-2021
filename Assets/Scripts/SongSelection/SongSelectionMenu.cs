using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SongSelectionMenu : MonoBehaviour
{
    [SerializeField] BoxCollider selectionArea = default;

    //[SerializeField] BoxCollider switchSongArea = default;
    [SerializeField] TMP_Text selectionText = default;


    public GameObject panel;
    public int mainMenu;

    public SongObject[] songs;

    public GameObject [] panels;

    private Vector3 scaleChange;

    private GameObject selectedPanel;

    private AudioClip selectedSongMusic;

    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Load the Managers scene
        if (!SceneManager.GetSceneByName("Managers").isLoaded)
        {
            SceneManager.LoadSceneAsync("Managers", LoadSceneMode.Additive);
        }

        for(int i = 0; i < panels.Length; i++){
            GameObject currentPanel = panels[i];
            

            if(i <= songs.Length){ 
                SongObject currentSong = songs[i];
                currentPanel.GetComponent<SongPanel>().setSong(currentSong);
                //selectedSongMusic = currentSong.song;
            }else{
                continue;
            }

            
        }


    }
    void Update()
    {
        if (selectionArea != default) //Kollar ifall default värdet överensstämmer
        {
            CheckSelection();
        }

        revertChange();
    }

    private void CheckSelection()
    {       
        if (Physics.CheckBox(selectionArea.transform.position, selectionArea.size)) // En snabb och billig koll ifall n�gon collider befinner sig i selectionArea
        {
            string displayText = "Your Selected Song Is: ";

            Collider[] hitColliders = Physics.OverlapBox(selectionArea.transform.position, selectionArea.size); // H�mtar alla colliders som har kolliderat, relativt dyr s� vi vill g�rna f�rb�ttra koden senare s� den inte k�rs i on�dan
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i] == selectionArea || hitColliders[i] == null) // Hoppar �ver dessa
                    continue;
                displayText += hitColliders[i].gameObject.name; // H�r h�mtar man allt man vill fr�n objektet, just nu �r det bara namnet f�r GameObjectet
                
                selectedPanel = hitColliders[i].gameObject;
                print(i);

                selectedSongMusic = songs[i].song;

                float defaultScale = hitColliders[i].transform.localScale.z;
                scaleChange = new Vector3(1.25f, 1.25f, defaultScale);

                if(hitColliders[i] != selectionArea){
                    hitColliders[i].gameObject.transform.localScale = scaleChange;
                    audioSource.PlayOneShot(selectedSongMusic);
                }
            }
            selectionText.text = displayText;
        }
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

    private void revertChange(){
        foreach(GameObject panel in panels){
            if(panel.GetInstanceID() != selectedPanel.GetInstanceID()){
                panel.transform.localScale = Vector3.one;
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

    public void Back(){
        if(panel.activeInHierarchy){
            panel.SetActive(false);
        }
    }
}
