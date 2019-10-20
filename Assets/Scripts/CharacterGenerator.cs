using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerator : MonoBehaviour
{
    public List<MSB_Character> PlayerPrefabs;
    public int PrefabNum = 0;

    private void Awake()
    {
        InstantiateCharacter(PrefabNum);
    }

    const int WEAPON_SWORD = 0;
    const int WEAPON_SHURIKEN = 1;
    private void InstantiateCharacter(int PrefabNum)
    {
        MSB_Character newPlayer = new MSB_Character();       
        switch (PrefabNum)
        {
            case WEAPON_SWORD:
                newPlayer = (MSB_Character)Instantiate(PlayerPrefabs[WEAPON_SWORD], new Vector3(0, 0, 0), Quaternion.identity);
                break;

            case WEAPON_SHURIKEN:
                newPlayer = (MSB_Character)Instantiate(PlayerPrefabs[WEAPON_SHURIKEN], new Vector3(0, 0, 0), Quaternion.identity);
                break;

            default:
                Debug.LogWarning("recieved weapon id is not defined");
                break;
        }

        newPlayer.gameObject.AddComponent<RCReciever>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
