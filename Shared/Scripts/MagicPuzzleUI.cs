using MagicBits.Shared.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public abstract class MagicPuzzleUI : MonoBehaviour
    {
        public GameObject panel;
        public TextMeshProUGUI questionTMP;
        public ParticleSystem wrongAnswerVFX;
        public AudioSource wrongAnswerSFX;
        public ParticleSystem correctAnswerVFX;
        public AudioSource correctAnswerSFX;
    
        public UnityEvent<string> onAnswerConfirmed;

        protected Animator animator;
    
        protected virtual void Awake()
        {
            animator = panel.GetComponent<Animator>();
        }

        public virtual void ActivatePanel(bool value)
        {
            if (value)
            {
                SetUpPanel();
                gameObject.SetActive(true);
            }
            else
            {
                // Starts fadeout animation.
                animator.SetTrigger("out");
                var fadeEvents =  animator.gameObject.GetComponent<FadeEvents>();
                Utilities.WaitEventTask(fadeEvents.onFadeOutCompleted, () =>
                {
                    InputClear();
                    gameObject.SetActive(false);
                });
            }
        }

        public virtual void DisplayQuestion(string text)
        {
            questionTMP.text = text;
        }

        public abstract void InputClear();

        public abstract void OnConfirmButtonPressed();

        protected abstract void SetUpPanel();
    
        public virtual void AnswerAccepted()
        {
            // Por algum motivo, esse VFX só toca se der Stop antes.\
            correctAnswerVFX.Stop();
            correctAnswerVFX.Play();
            correctAnswerSFX.Play();
        }

        public virtual void AnswerDenied()
        {
            wrongAnswerVFX.Stop();
            wrongAnswerVFX.Play();
            wrongAnswerSFX.Play();
        }
    }
}
