using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
public class CharacterState : MonoBehaviour
{

    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;

    //UI
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    //새로 추가되는 마나 변수
    public int maxMana = 10;                    //최대 마나
    public int currentMana;                     //현재 마나
    public Slider manaBar;                  //마나바 ui
    public TextMeshProUGUI manaText;            //마나 텍스트 ui


    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateUI();
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (DamageEffectManager.instance != null)
        {
            Vector3 position = transform.position;
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0);

            DamageEffectManager.instance.ShowDamager(position, damage, false);
        }
    }

    public void Heal(int amout)
    {
        currentHealth += amout;
        if (DamageEffectManager.instance != null)
        {
            Vector3 position = transform.position;
            position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 1.5f), 0);

            DamageEffectManager.instance.ShowDamager(position, amout, false);
        }
    }

    public void UseMana(int amout)
    {
        currentMana -= amout;
        if (currentMana < 0)
        {
            currentMana = 0;
        }
        UpdateUI();
    }

    public void GainMana(int amout)
    {
        currentMana += amout;

        if(currentMana > maxMana)
        {
            currentMana = maxMana;
        } 
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }

        if(healthText!= null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
        if(manaBar!= null)
        {
            manaBar.value = (float)currentHealth / maxMana;
        }

        if(manaText!= null)
        {
            manaText.text = $"{currentMana}/{maxMana}";
        }
        
        
    }

}
