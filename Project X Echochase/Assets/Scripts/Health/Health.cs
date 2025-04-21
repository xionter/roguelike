using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    public int StartingHealth{get;private set;}
    public int CurrentHealth{get;private set;}

    public void SetStartingHealth(int startingHealth) 
    { 
        this.StartingHealth = startingHealth; 
        CurrentHealth = startingHealth; 
    }
}
