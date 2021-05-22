using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SongSelectionMenu : MonoBehaviour
{
    [SerializeField] BoxCollider selectionArea;
    [SerializeField] TMP_Text selectionText;

    public GameObject panel;
    public int mainMenu;


    void Update()
    {
        CheckSelection();
    }

    private void CheckSelection()
    {
        if (Physics.CheckBox(selectionArea.transform.position, selectionArea.size)) // En snabb och billig koll ifall någon collider befinner sig i selectionArea
        {
            string displayText = "Your Selected Song Is: ";

            Collider[] hitColliders = Physics.OverlapBox(selectionArea.transform.position, selectionArea.size); // Hämtar alla colliders som har kolliderat, relativt dyr så vi vill gärna förbättra koden senare så den inte körs i onödan
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i] == selectionArea || hitColliders[i] == null) // Hoppar över dessa
                    continue;
                displayText += hitColliders[i].gameObject.name; // Här hämtar man allt man vill från objektet, just nu är det bara namnet för GameObjectet
            }

            selectionText.text = displayText;
        }
    }

    public void GoBack()
    {
        //if (panel.activeInHierarchy)
        //{
        //    panel.SetActive(false);
        //}
        //else
        //{
        //    SceneManager.LoadScene(mainMenu);
        //}
    }

    public void SelectSong()
    {
        //if (panel != null)
        //{
        //    panel.SetActive(true);
        //}

    }

    public void StartGame()
    {

    }
}
