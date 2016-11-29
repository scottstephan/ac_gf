using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class m_roundAdvanceButton : MonoBehaviour {
    public enum buttonRole
    {
        advanceToNextRound,
        startGame,
        endGame
    }

    public buttonRole myButtonRole;
    public EasyTween toMid;
    public EasyTween toLeft;
    public Text buttonText;

    public void onClick()
    {
        switch (myButtonRole)
        {
            case buttonRole.advanceToNextRound:
                 m_gameManager.instance.resetRoundInfo();
                 m_gameManager.instance.moveInRoundInterface(m_gameManager.instance.roundIndex);
                 m_gameManager.instance.changePhase(m_gameManager.roundPhases.input);
                 toLeft.OpenCloseObjectAnimation();
                m_gameManager.instance.updateRoundCounter();
                break;
            case buttonRole.endGame:
                m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreComp);
                toLeft.OpenCloseObjectAnimation();
                break;
            case buttonRole.startGame:
                break;
        }
    }

    public void setTextByRole()
    {
        switch (myButtonRole)
        {
            case buttonRole.advanceToNextRound:
                buttonText.text = "Next Round";
                buttonText.fontSize = 50;
                break;
            case buttonRole.endGame:
                if (appManager.curLiveGame.isMPGame)
                    buttonText.text = "See Who Won";
                else
                    buttonText.text = "See Your Score";
                buttonText.fontSize = 50;
                break;
            case buttonRole.startGame:
                break;
        }

    }
}
