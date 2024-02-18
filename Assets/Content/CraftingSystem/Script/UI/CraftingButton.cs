using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour
{
    [SerializeField] private Button _Button;
    [SerializeField] private Image ObjectImage;

    private UnityAction CraftEvent; // Akcja do wykonania po klikniêciu przycisku

    public void SetUpButton(UnityAction craftEvent, Sprite craftObjectImage, float objectRotation)
    {
        CraftEvent = craftEvent; // Przypisanie akcji do CraftEvent
        _Button.onClick.AddListener(OnClick); // Dodanie metody OnClick do zdarzenia onClick przycisku
        ObjectImage.sprite = craftObjectImage;
        ObjectImage.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, objectRotation);
    }

    private void OnClick()
    {
        CraftEvent?.Invoke(); // Wywo³anie akcji po klikniêciu przycisku
        gameObject.SetActive(false);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Tutaj mo¿esz umieœciæ kod, który ma byæ wykonany po najechaniu na przycisk
        Debug.Log("Mouse entered the button!");
    }
}