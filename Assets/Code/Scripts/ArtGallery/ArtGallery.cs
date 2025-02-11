using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArtGallery : MonoBehaviour
{
    [System.Serializable]
    struct displayImages 
    {
        public Sprite image;
        public string description;
    }
    public const string ARTPICKUP_PLAYERPREFS_NAME = "ArtPickup";
    [SerializeField] List<displayImages> imagesToDisplay = new List<displayImages>();
    [SerializeField] List<Image> displays;
    private List<bool> unlockedImages = new List<bool>();
    [SerializeField] string imageDescriptionForlocked = "?";
    [SerializeField] Sprite lockedImageSprite;
    [SerializeField] Image zoomedInImageDisplay;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject multyviewParent;
    [SerializeField] GameObject singleviewParent;
    [SerializeField] TextMeshProUGUI imageDescription;
    [SerializeField] string mainMenuName;

    private bool zoomedInView = false;
    private int currentID = 0;

    public void ResetArtPickups() 
    {
        for(int i = 0; i <= imagesToDisplay.Count; i++) 
        {
            PlayerPrefs.SetInt(ARTPICKUP_PLAYERPREFS_NAME + i,0);
        }
    }

    public void BackToMainMenu() 
    {
        SceneManager.LoadScene(mainMenuName);
    }

    private void CheckStatus() 
    {
        unlockedImages.Clear();
        for (int i = 0; i <= imagesToDisplay.Count; i++) 
        {
            if(PlayerPrefs.GetInt(ARTPICKUP_PLAYERPREFS_NAME + i) > 0)
            {
                unlockedImages.Add(true);
            }
            else 
            {
                unlockedImages.Add(false);
            }

        }
    }


    public void SetCurrentIDViaButton(int id) 
    {
        currentID = id;
        zoomedInView = true;
        Debug.Log("Button Pressed");
        Display();
    }

    public void SetMultview() 
    {
        zoomedInView = false;
        Display();
    }

    public void IncrementCurrentID() 
    {
        currentID++;
        if(currentID >= imagesToDisplay.Count) 
        {
            currentID = 0;
        }
        Display();
    }

    public void DecrementCurrentID() 
    {
        currentID--;
        if (currentID < 0)
        {
            currentID = imagesToDisplay.Count-1;
        }
        Display();
    }

    private void Start()
    {
        Display();
    }

    public void Display() 
    {

        
        if (zoomedInView) 
        {
            SingleView();
        }

        else 
        {
            MultipleView();
        }
    }

    private void SingleView() 
    {
        multyviewParent.SetActive(false);
        singleviewParent.SetActive(true);

        bool unlocked = unlockedImages[currentID];
        zoomedInImageDisplay.sprite = unlocked ? imagesToDisplay[currentID].image : lockedImageSprite;
        imageDescription.text = unlocked ? imagesToDisplay[currentID].description : imageDescriptionForlocked;
    }
    private void MultipleView() 
    {
        multyviewParent.SetActive(true);
        singleviewParent.SetActive(false);
        int i = imagesToDisplay.Count - displays.Count;
        CheckStatus();
        for (i = 0; i < imagesToDisplay.Count; i++) 
        {
            bool unlocked = unlockedImages[i];
            displays[i].sprite = unlocked ? imagesToDisplay[i].image : lockedImageSprite;
        }
    
    }

}
