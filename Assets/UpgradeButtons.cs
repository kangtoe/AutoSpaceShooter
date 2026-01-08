using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButtons : MonoBehaviour
{
    [SerializeField] GameObject upgradeButtonUIPrefab;

    public List<UpgradeButtonUI> UpgradeButtonUIList
    {
        get {
            if (upgradeButtonUIList == null)
            {
                upgradeButtonUIList = new List<UpgradeButtonUI>();

                for (int i = 0; i < 5; i ++)
                {
                    UpgradeButtonUI ui = Instantiate(upgradeButtonUIPrefab, transform).GetComponent<UpgradeButtonUI>();
                    upgradeButtonUIList.Add(ui);
                }
            }            
            return upgradeButtonUIList;
        }
    }
    List<UpgradeButtonUI> upgradeButtonUIList;
}
