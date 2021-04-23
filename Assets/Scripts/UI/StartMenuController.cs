using UnityEngine;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    [Tooltip("The dropdown with songs")]
    [SerializeField] TMP_Dropdown songSelection;
    [Tooltip("The songs you have added to the dropdown, NOTE!: their position must match their positions in the dropdown")]
    [SerializeField] SongObject[] songObjects;

    SongHandler instance;

    private void Start()
    {
        instance = SongHandler.Instance;
        instance.SetSongObject(songObjects[0]);
    }

    public void OnDropdownChange()
    {
        instance.SetSongObject(songObjects[songSelection.value]);
    }

}
