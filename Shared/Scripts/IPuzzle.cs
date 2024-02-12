using System.Collections;

namespace MagicBits_OSS.Shared.Scripts
{
    public interface IPuzzle
    {
        void StartInteraction();
        void CloseInteraction();
        void FinishInteraction();
        bool ValidateAnswer(string playerInput);
        IEnumerator UpdateCurrentQuestion();
        void OnAnswerConfirmed(string playerInput);
    }
}
