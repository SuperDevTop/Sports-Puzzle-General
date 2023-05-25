using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    [Header("--------------------  English  --------------------")]
    [SerializeField] private GameObject settings0E;
    [SerializeField] private GameObject settings1E;
    [SerializeField] private GameObject settings2E;
    [SerializeField] private GameObject settings3E;
    [SerializeField] private GameObject settingsBtnE;

    [Header("--------------------  Spanish  --------------------")]
    [SerializeField] private GameObject settings0S;
    [SerializeField] private GameObject settings1S;
    [SerializeField] private GameObject settings2S;
    [SerializeField] private GameObject settings3S;
    [SerializeField] private GameObject settingsBtnS;

    [SerializeField] private GameObject successText;    
    [SerializeField] private GameObject notSuccessText;
    [SerializeField] private GameObject[] items;
    [SerializeField] private GameObject successLines;
    [SerializeField] private GameObject notSuccessLines;
    [SerializeField] private Sprite[] itemImages;
    [SerializeField] private Sprite[] statusSprites;
    [SerializeField] private AudioSource buttonAudio;
    [SerializeField] private AudioSource bgAudio;

    [SerializeField] private GameObject[] soundGameBtn;
    [SerializeField] private GameObject[] soundOnBtns;
    [SerializeField] private GameObject[] soundOffBtns;

    public int gameStatus;
    public int leftIndex;
    public int rightIndex;
    public int leftImageIndex;
    public int rightImageIndex;

    bool isPlayingMusic = true;
    bool isEnglish = true;
    bool eLanguage = false;
    bool eSound = false;
    //bool sLanguage = false;
    //bool sSound = false;

    #region Game Index
    enum GameStatus
    {
        Play = 0,
        Pause = 1
    }

    #endregion

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }            

        if (gameStatus == (int)GameStatus.Play)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                leftIndex = 3;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.tag == "left")
                    { 
                        leftIndex = hit.transform.gameObject.GetComponent<ItemIndex>().itemInfo;
                        leftImageIndex = hit.transform.gameObject.GetComponent<ImageIndex>().imageInfo;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.tag == "right")
                    {
                        rightIndex = hit.transform.gameObject.GetComponent<ItemIndex>().itemInfo;
                        rightImageIndex = hit.transform.gameObject.GetComponent<ImageIndex>().imageInfo;

                        if (leftIndex != 3)
                        {
                            if (leftImageIndex == rightImageIndex)
                            {
                                StartCoroutine(DelayToShowState(0));
                            }
                            else
                            {
                                StartCoroutine(DelayToShowState(1));
                            }
                        }
                    }
                }
            }
        }
    }

    #region Game Engine
    public void ShowImage()
    {
        gameStatus = (int)GameStatus.Play;
        leftIndex = 3;
        rightIndex = 3;
        
        List<int> imageIndices;
        List<int> selectedIndicesLeft;
        List<int> selectedIndicesRight;

        imageIndices = new List<int>();
        selectedIndicesLeft = new List<int>();
        selectedIndicesRight = new List<int>();

        // Add all available images
        for (int i = 0; i < 18; i++)
        {
            imageIndices.Add(i);
        }

        // Select image indices
        for(int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, 1000) % imageIndices.Count;
            selectedIndicesLeft.Add(imageIndices[index]);
            selectedIndicesRight.Add(imageIndices[index]);
            imageIndices.RemoveAt(index);                        
        }

        // Place images
        for(int i = 0; i < 3; i++)
        {
            int indexLeft = Random.Range(10, 100) % (3 - i);
            int indexRight = Random.Range(0, 1000) % (3 - i);

            items[i * 2].transform.GetChild(0).GetComponent<Image>().sprite = itemImages[selectedIndicesLeft[indexLeft] * 2];
            items[i * 2].GetComponent<ImageIndex>().imageInfo = selectedIndicesLeft[indexLeft];

            items[i * 2 + 1].transform.GetChild(0).GetComponent<Image>().sprite = itemImages[selectedIndicesRight[indexRight] * 2 + 1];
            items[i * 2 + 1].GetComponent<ImageIndex>().imageInfo = selectedIndicesRight[indexRight];

            selectedIndicesLeft.RemoveAt(indexLeft);
            selectedIndicesRight.RemoveAt(indexRight);
        }
    }

    IEnumerator DelayToShowState(int index)
    {
        if (index == 0)
        {
            successText.SetActive(true);
            successLines.transform.GetChild(leftIndex * 3 + rightIndex).gameObject.SetActive(true);
        }
        else
        {
            notSuccessText.SetActive(true);
            notSuccessLines.transform.GetChild(leftIndex * 3 + rightIndex).gameObject.SetActive(true);
        }

        gameStatus = (int)GameStatus.Pause;

        yield return new WaitForSeconds(2f);

        successText.SetActive(false);
        notSuccessText.SetActive(false);
        successLines.transform.GetChild(leftIndex * 3 + rightIndex).gameObject.SetActive(false);
        notSuccessLines.transform.GetChild(leftIndex * 3 + rightIndex).gameObject.SetActive(false);
        
        ShowImage();
    }

    #endregion

    #region UI functionalites
    public void PlayGame()
    {        
        if (isPlayingMusic)
        {
            soundGameBtn[0].SetActive(true);
            soundGameBtn[1].SetActive(false);
            bgAudio.Play();
        }
        else
        {
            soundGameBtn[1].SetActive(true);
            soundGameBtn[0].SetActive(false);
        }

        if (isEnglish)
        {
            successText.GetComponent<Image>().sprite = statusSprites[0];
            notSuccessText.GetComponent<Image>().sprite = statusSprites[1];
        }
        else
        {
            successText.GetComponent<Image>().sprite = statusSprites[2];
            notSuccessText.GetComponent<Image>().sprite = statusSprites[3];
        }
        
        ShowImage();
    }

    public void PlayBtnSound()
    {
        if (isPlayingMusic)
        {
            buttonAudio.Play();
        }                
    }

    public void GameSoundOnBtnClick()
    {
        soundGameBtn[0].SetActive(true);
        soundGameBtn[1].SetActive(false);
        isPlayingMusic = true;
        SettingsBtnClick();
        bgAudio.Play();
    }

    public void GameSoundOffBtnClick()
    {
        soundGameBtn[0].SetActive(false);
        soundGameBtn[1].SetActive(true);
        isPlayingMusic = false;
        SettingsBtnClick();
        bgAudio.Pause();
    }

    public void SoundOnBtnClick()
    {
        soundGameBtn[0].SetActive(true);
        soundGameBtn[1].SetActive(false);
        isPlayingMusic = true;
        SettingsBtnClick();
    }

    public void SoundOffBtnClick()
    {
        soundGameBtn[0].SetActive(false);
        soundGameBtn[1].SetActive(true);
        isPlayingMusic = false;
        SettingsBtnClick();
    }

    public void EnglishBtnClick()
    {
        settingsBtnE.SetActive(true);
        settingsBtnS.SetActive(false);
        isEnglish = true;        
    }

    public void SpanishBtnClick()
    {
        settingsBtnE.SetActive(false);
        settingsBtnS.SetActive(true);
        isEnglish = false;  
    }

    public void BackBtnClick()
    {
        eSound = false;
        eLanguage = false;
    }

    public void HomeBtnClick()
    {
        bgAudio.Stop();
    }

    public void SettingsBtnClick()
    {
        if (isPlayingMusic)
        {
            for (int i = 0; i < 4; i++)
            {
                soundOnBtns[i].SetActive(true);
                soundOffBtns[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                soundOnBtns[i].SetActive(false);
                soundOffBtns[i].SetActive(true);
            }
        }
    }

    // ------------------ English ----------------------------
    public void ELanguageOnClick()
    {
        if (eSound)
        {
            settings2E.SetActive(false);
            settings3E.SetActive(true);
            eLanguage = true;
        }
        else
        {
            settings0E.SetActive(false);
            settings1E.SetActive(true);
            eLanguage = true;
        }                
    }

    public void ELanguageOffClick()
    {
        if (eSound)
        {
            settings3E.SetActive(false);
            settings2E.SetActive(true);
            eLanguage = false;
        }
        else
        {
            settings1E.SetActive(false);
            settings0E.SetActive(true);
            eLanguage = false;
        }
    }

    public void ESoundOnClick()
    {
        if (eLanguage)
        {
            settings1E.SetActive(false);
            settings3E.SetActive(true);
            eSound = true;
        }
        else
        {
            settings0E.SetActive(false);
            settings2E.SetActive(true);
            eSound = true;
        }
    }

    public void ESoundOffClick()
    {
        if (eLanguage)
        {
            settings3E.SetActive(false);
            settings1E.SetActive(true);
            eSound = false;
        }
        else
        {
            settings2E.SetActive(false);
            settings0E.SetActive(true);
            eSound = false;
        }
    }

    // ------------------ Spanish ----------------------------
    public void SLanguageOnClick()
    {
        if (eSound)
        {
            settings2S.SetActive(false);
            settings3S.SetActive(true);
            eLanguage = true;
        }
        else
        {
            settings0S.SetActive(false);
            settings1S.SetActive(true);
            eLanguage = true;
        }
    }

    public void SLanguageOffClick()
    {
        if (eSound)
        {
            settings3S.SetActive(false);
            settings2S.SetActive(true);
            eLanguage = false;
        }
        else
        {
            settings1S.SetActive(false);
            settings0S.SetActive(true);
            eLanguage = false;
        }
    }

    public void SSoundOnClick()
    {
        if (eLanguage)
        {
            settings1S.SetActive(false);
            settings3S.SetActive(true);
            eSound = true;
        }
        else
        {
            settings0S.SetActive(false);
            settings2S.SetActive(true);
            eSound = true;
        }
    }

    public void SSoundOffClick()
    {
        if (eLanguage)
        {
            settings3S.SetActive(false);
            settings1S.SetActive(true);
            eSound = false;
        }
        else
        {
            settings2S.SetActive(false);
            settings0S.SetActive(true);
            eSound = false;
        }
    }
    
    #endregion
}
