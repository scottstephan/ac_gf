using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DEBUG_debutButtonBehaviors : MonoBehaviour {

	public enum debugButtonBehaviors
    {
        showAd,
        addCredit,
        lockNonDefaultCats,
        lockAllCats,
        unlockAllCats
    }

    public debugButtonBehaviors thisButtonBehavior;

    public void OnClick()
    {
        List<string> allCatNames = u_acJsonUtility.instance.discoverCategories(true);

        switch (thisButtonBehavior)
        {
            case debugButtonBehaviors.showAd:
                break;
            case debugButtonBehaviors.addCredit:
                break;
            case debugButtonBehaviors.lockNonDefaultCats:
                break;
            case debugButtonBehaviors.lockAllCats:
                for(int i = 0; i < allCatNames.Count; i++)
                {
                    u_acJsonUtility.instance.findAndLockCategory(allCatNames[i]);
                }
                break;
            case debugButtonBehaviors.unlockAllCats:
                for (int i = 0; i < allCatNames.Count; i++)
                {
                    u_acJsonUtility.instance.findAndUnlockCategory(allCatNames[i]);
                }
                break;
        }
            
    }
}
