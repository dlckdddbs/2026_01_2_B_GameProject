using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class CharacterState : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;

    //UI
    public Slider healthBar;
    public TextMeshProUGUI healthText;


    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Heal(int amout)
    {
        currentHealth += amout;
    }

}
