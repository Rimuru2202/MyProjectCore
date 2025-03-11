using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    [Tooltip("UI Text to display notifications")]
    public Text notificationText;
    public float fadeOutDuration = 1f; // Duration for fade-out

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        if(notificationText != null)
        {
            notificationText.text = "";
            notificationText.gameObject.SetActive(false);
        }
    }

    public void ShowNotification(string message, float displayDuration = 2f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowNotificationCoroutine(message, displayDuration));
    }

    IEnumerator ShowNotificationCoroutine(string message, float displayDuration)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            Color origColor = notificationText.color;
            notificationText.color = new Color(origColor.r, origColor.g, origColor.b, 1f);
            notificationText.gameObject.SetActive(true);
            yield return new WaitForSeconds(displayDuration);
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                notificationText.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
                yield return null;
            }
            notificationText.text = "";
            notificationText.gameObject.SetActive(false);
            notificationText.color = new Color(origColor.r, origColor.g, origColor.b, 1f);
        }
    }
}
