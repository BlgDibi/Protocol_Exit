using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image greenbar; 
    [SerializeField] Image redbar; 

    [Header("Player")]
    [SerializeField] HP playerHP;

    [Header("Animation")]
    [Tooltip("Red bar speed after gren bar")]
    [SerializeField] float ghostSpeed = 3f;

    float maxhp;

    void Start()
    {
        if (playerHP != null)
        {
            maxhp = playerHP.Hp;

            if (greenbar != null) greenbar.fillAmount = 1f;
            if (redbar != null) redbar.fillAmount = 1f;
        }
    }

    void Update()
    {
        if (playerHP != null && maxhp > 0)
        {
            float targetFill = playerHP.isDead ? 0f : (playerHP.Hp / maxhp);

            greenbar.fillAmount = targetFill;

            if (redbar != null)
            {
                if (redbar.fillAmount > greenbar.fillAmount)
                {
                    redbar.fillAmount = Mathf.Lerp(redbar.fillAmount, greenbar.fillAmount, Time.deltaTime * ghostSpeed);
                }
                else
                {
                    redbar.fillAmount = greenbar.fillAmount;
                }
            }
        }
    }
}