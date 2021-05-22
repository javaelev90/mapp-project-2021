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
        if (Physics.CheckBox(selectionArea.transform.position, selectionArea.size)) // En snabb och billig koll ifall n�gon collider befinner sig i selectionArea
        {
            string displayText = "Your Selected Song Is: ";

            Collider[] hitColliders = Physics.OverlapBox(selectionArea.transform.position, selectionArea.size); // H�mtar alla colliders som har kolliderat, relativt dyr s� vi vill g�rna f�rb�ttra koden senare s� den inte k�rs i on�dan
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i] == selectionArea || hitColliders[i] == null) // Hoppar �ver dessa
                    continue;
                displayText += hitColliders[i].gameObject.name; // H�r h�mtar man allt man vill fr�n objektet, just nu �r det bara namnet f�r GameObjectet
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
