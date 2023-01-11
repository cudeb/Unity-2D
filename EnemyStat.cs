using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStat : MonoBehaviour
{
    public int hp;
    public int currentHp;
    public int atk;
    public int def;
    public int exp;

    public GameObject healthBarBackground;
    public Image healthBarFilled;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = hp;
        healthBarFilled.fillAmount = 1f;
    }

    public int Hit(int _playerAtk)
    {
        int playerAtk = _playerAtk;
        int dmg;
        if(def >= playerAtk)
        {
            dmg = 1;
        }
        else
        {
            dmg = playerAtk - def;
        }

        currentHp -= dmg;

        if(currentHp <= 0)
        {
            Destroy(this.gameObject);
            PlayerStat.instance.currentExp += exp;
        }

        healthBarFilled.fillAmount = (float)currentHp / hp;
        healthBarBackground.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(WaitCoroutine());
        return dmg;
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(3f);
        healthBarBackground.SetActive(false);
    }
}
