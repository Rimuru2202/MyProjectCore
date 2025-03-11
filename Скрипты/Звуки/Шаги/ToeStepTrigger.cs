using UnityEngine;

public class ToeStepTrigger : MonoBehaviour
{
    private StepSoundController stepSoundController;

    private void Start()
    {
        // Ищем компонент StepSoundController в родительских объектах
        stepSoundController = GetComponentInParent<StepSoundController>();
        if (stepSoundController == null)
        {
            Debug.LogError("ToeStepTrigger: StepSoundController не найден в родителе объекта " + gameObject.name);
        }
    }

    // Вызывается, когда этот триггер сталкивается с другим коллайдером
    private void OnTriggerEnter(Collider other)
    {
        // Если столкнулись с землей (тег "Ground")
        if (other.CompareTag("Ground"))
        {
            stepSoundController.PlayStepSound();
        }
    }
}
