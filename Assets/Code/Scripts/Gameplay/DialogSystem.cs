using HeroController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogSystem : MonoBehaviour
{
    [SerializeField] List<KeyCode> keysToAdvanceDialog;
    [SerializeField] List<DialogText> texts;
    [SerializeField] DialogBox boxRefrences;
    private const float BUTTON_PRESS_COOLDOWN = 0.25f;
    public bool blockMovement = false;
    bool canAdvance = true;
    private int currentDialogID = 0;
    [HideInInspector] public bool dialogCompleted = false;
    PlayerController playerRef;
    [System.Serializable]
    struct DialogText
    {
        public Sprite speaker;
        public Sprite textBackgroundImage;
        public string text;
    }

    private void Awake()
    {
        this.gameObject.SetActive(false);
        this.boxRefrences.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResetText();
        if (texts.Count > 0)
        {
            boxRefrences.SetValues(texts[currentDialogID].speaker, texts[currentDialogID].text, texts[currentDialogID].textBackgroundImage);
            playerRef = FindAnyObjectByType<PlayerController>();
            playerRef.inputBlocked = blockMovement;
        }
    }

    private void OnDisable()
    {
        if (playerRef != null)
        {
            playerRef.inputBlocked = false;
        }
    }

    public void BecomeActive()
    {
        if (playerRef == null)
        {
            playerRef = FindAnyObjectByType<PlayerController>();

        }
        boxRefrences.SetValues(texts[currentDialogID].speaker, texts[currentDialogID].text, texts[currentDialogID].textBackgroundImage);
        this.gameObject.SetActive(true);
        boxRefrences.gameObject.SetActive(true);
    }
    public void ResetText()
    {
        currentDialogID = 0;
    }
    private void Update()
    {
        playerRef.inputBlocked = blockMovement;
        if (CheckKeys())
        {
            AdvanceDialog();
        }
    }
    private void AdvanceDialog()
    {
        if (canAdvance)
        {
            StartCoroutine(InputCooldown());
            canAdvance = false;
        }
        currentDialogID++;
        if (currentDialogID >= texts.Count)
        {
            dialogCompleted = true;
            this.gameObject.SetActive(false);
            boxRefrences.gameObject.SetActive(false);
            playerRef.inputBlocked = false;
            return;
        }
        boxRefrences.SetValues(texts[currentDialogID].speaker, texts[currentDialogID].text, texts[currentDialogID].textBackgroundImage);


    }

    IEnumerator InputCooldown()
    {
        yield return new WaitForSeconds(BUTTON_PRESS_COOLDOWN);
        canAdvance = true;
    }

    private bool CheckKeys()
    {
        if (canAdvance)
        {
            foreach (var key in keysToAdvanceDialog)
            {
                if (Input.GetKey(key)) return true;
            }
            return false;
        }
        else return false;
    }
}
