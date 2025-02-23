using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class instructions : MonoBehaviour
{
    public int currentPage = 0;
    public List<GameObject> pages;

    public Button nextButton;
    public List<Transform> locations;

    public Camera mainCamera;

    public GameObject grid;
    public GameObject titleScreen;

    public void Start()
    {
        grid.SetActive(false);
        currentPage = 0;
        UpdatePage(currentPage);
    }
    public void NextPage()
    {
        if (currentPage == pages.Count - 1)
        {
            Debug.Log("Return to title screen");
            gameObject.SetActive(false);
            titleScreen.SetActive(true);
            grid.SetActive(false);
        }
        if (currentPage < pages.Count - 1)
        {
            currentPage++;
            UpdatePage(currentPage - 1);
        }



    }
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage(currentPage + 1);
        }

    }
    public void UpdatePage(int last)
    {
        pages[last].SetActive(false);
        pages[currentPage].SetActive(true);
        mainCamera.transform.position = locations[currentPage].position;
        if (currentPage == pages.Count - 1)
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Return";
        }
        else
        {
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
        if (currentPage <= 5 || currentPage > 9)
        {
            grid.SetActive(false);
        }
        else
        {
            grid.SetActive(true);
        }
    }
}