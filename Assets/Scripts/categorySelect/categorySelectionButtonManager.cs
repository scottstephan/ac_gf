﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class categorySelectionButtonManager : MonoBehaviour
{
    public string categoryName;
    public int categoryId;

    public void categorySelected()
    {
        m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundMP);
    }
}
