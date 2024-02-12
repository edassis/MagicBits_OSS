using UnityEngine;
using UnityEngine.Events;

namespace MagicBits_OSS.Shared.Scripts
{
    public class SelectIsland : MonoBehaviour
    {
        // public GameObject StartGroup;
        // public GameObject StartedGroup;
        // public GameObject level1;
        // public GameObject level2;
        // public GameObject level3;
        public UnityEvent onPressAnyKey;

        // Start is called before the first frame update
        void Start()
        {
            // CreateObjectives();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.anyKeyDown)
            {
                // StartGroup.SetActive(false);
                // StartedGroup.SetActive(true);
                onPressAnyKey.Invoke();
            }
        }

        // Função responsável por criar os objetivos do jogo.
        // Isso é o que permite salvar as informações dos alunos, como em que nível eles estão.
        // Após cumprir o primeiro objetivo, o aluno vai liberar o level 2 e assim por diante.
        // Além disso, cada objetivo possui uma nota que será usada no final como média para o minigame todo.
        // Essa informação é salva no Moodle através do SCORM.
        // private void CreateObjectives()
        // {
        //     List<Scorm.ObjectiveData> objectives = Scorm.Moodle.Moodle.GetObjectives();

        //     if(objectives.Count == 0)
        //     {
        //         Scorm.ObjectiveData objectiveData = new Scorm.ObjectiveData("Aritmetica", 0f, 100f, 0f, 1f, Scorm.LessonStatus.Incomplete, Scorm.LessonStatus.Completed, 1f, "Nível em que testa o conhecimento sobre soma, subtração, multiplicação, divisão e resto.");
        //         Scorm.Moodle.Moodle.SetObjective(objectiveData);

        //         Scorm.ObjectiveData objectiveData2 = new Scorm.ObjectiveData("LogicaDeslocamento", 0f, 100f, 0f, 1f, Scorm.LessonStatus.Incomplete, Scorm.LessonStatus.Completed, 1f, "Nível em que testa o conhecimento sobre and, or, xor, sll, srl e sra");
        //         Scorm.Moodle.Moodle.SetObjective(objectiveData2);

        //         Scorm.ObjectiveData objectiveData3 = new Scorm.ObjectiveData("MemoriaSaltos", 0f, 100f, 0f, 1f, Scorm.LessonStatus.Incomplete, Scorm.LessonStatus.Completed, 1f, "Nível em que testa o conhecimento sobre memória, comparação, saltos condicionais e incondicionais.");
        //         Scorm.Moodle.Moodle.SetObjective(objectiveData3);
        //     }
        //     else
        //     {
        //         foreach (Scorm.ObjectiveData objective in objectives)
        //         {
        //             if(objective.Id == "Aritmetica" && objective.SuccessStatus == Scorm.LessonStatus.Completed)
        //             {
        //                 level2.SetActive(true);
        //             }
        //             else if(objective.Id == "LogicaDeslocamento" && objective.SuccessStatus == Scorm.LessonStatus.Completed)
        //             {
        //                 level3.SetActive(true);
        //             }
        //         }
        //     }
        // }
    }
}
