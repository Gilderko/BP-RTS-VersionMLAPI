using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Healthbar script that displays when you hover over it with a mouse.
/// </summary>
public class HealthDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

#if !UNITY_SERVER

    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
    }

    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
    }

#endif

    private void HandleHealthUpdated(int current, int max)
    {
        RectTransform transf = healthBarImage.GetComponent<RectTransform>();
        var previousScale = transf.localScale;
        transf.localScale = new Vector3(current / (float)max, previousScale.y, previousScale.z);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        healthBarParent.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        healthBarParent.SetActive(false);
    }
}
