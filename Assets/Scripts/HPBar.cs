using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] Image greenbar;
    [SerializeField] Image redbar;

    [SerializeField] HP playerHP;  

    float maxhp;

    void Start()
    {
        if (playerHP != null)
        {
            maxhp = playerHP.Hp; 
            redbar.fillAmount = maxhp / 100f;
        }
    }

    void Update()
    {
        if (playerHP != null)
        {
           
            greenbar.fillAmount = playerHP.Hp / maxhp;

          

           
            if (playerHP.isDead)
            {
                greenbar.fillAmount = 0f;  
            }
        }
    }
}
