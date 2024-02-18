using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTimer : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI timerText;

    private Coroutine craftingCoroutine; // Reference to the current crafting coroutine


    private void Start()
    {
        itemNameText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
    }

    public bool StartCrafting(string itemName, float craftTime)
    {
        itemNameText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);

        if (craftingCoroutine != null)
        {
            Debug.LogWarning("Crafting process is already in progress.");
            return false;
        }

        itemNameText.text = itemName;

        craftingCoroutine = StartCoroutine(WaitForCraft(craftTime));



        return true; // Indicate successful start of crafting
    }

    // Coroutine that waits for the specified craft time and updates crafting progress
    private IEnumerator WaitForCraft( float craftTime)
    {
        float timer = 0f;

        while (timer < craftTime)
        {
            timer += Time.deltaTime;

            progressBar.fillAmount = timer / craftTime;

            timerText.text = (craftTime - timer).ToString("F1"); // Format to one decimal place

            yield return null; // Wait for the next frame
        }

        ResetUI();

        yield return null;
    }

    // Resets UI after crafting completion or cancellation
    private void ResetUI()
    {
        progressBar.fillAmount = 0f;
        timerText.text = "0.0";



        itemNameText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);

        // Reset the crafting coroutine reference
        craftingCoroutine = null;
    }
}