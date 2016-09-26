using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class categorySelectionButtonManager : MonoBehaviour
{
    public string categoryName;
    public string categoryId;
    public Text catButtonText;

    public void categorySelected()
    {
        if (appManager.curLiveGame.isMPGame)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundMP);
        else
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
    }

    public void buttonClicked()
    {
        m_categorySelectionManager.instance.loadCategoryQuestion(categoryName);
        appManager.setCurGameQuestionDetails(categoryId,categoryName, appManager.currentQuestion.questionID, appManager.currentQuestion.questionName);

        m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
    }

    public void setUpButton()
    {
        catButtonText.text = categoryName;
    }
}
