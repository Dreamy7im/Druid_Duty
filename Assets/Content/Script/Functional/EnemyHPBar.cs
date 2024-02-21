using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI EnemyNameText;
    [SerializeField] private Image EnemyHPFillBar;



    public void ShowEnemyBar(string EnemyNameInfo, float EnemyHP)
    {
        EnemyNameText.text = EnemyNameInfo;
        UpdateBar(EnemyHP);
    }


    public void UpdateBar(float EnemyHP)
    {
        EnemyHPFillBar.fillAmount = EnemyHP;
    }

}
