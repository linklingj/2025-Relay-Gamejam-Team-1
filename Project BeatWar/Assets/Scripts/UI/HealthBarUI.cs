using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    Image fill;

    void Awake()
    {
        fill = GetComponent<Image>();
    }

    void Update()
    {
        fill.fillAmount = Player.Instance.Health / (float)Player.Instance.MaxHealth;
    }
}
