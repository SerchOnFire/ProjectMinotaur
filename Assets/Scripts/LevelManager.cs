using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    CharacterManager playerManager;
    CharacterManager minotaurManager;
    PlatformManager firstpPlatformManager;

    void Start ()
    {
        LoadLevel();
        StartFirstPlatform();
        playerManager = StartCharacter(playerManager, "Player");
        minotaurManager = StartCharacter(minotaurManager, "Minotaur");
    }

    void StartFirstPlatform()
    {
        firstpPlatformManager = GameObject.Find("PlatformSEStart").GetComponent<PlatformManager>();
    }

    CharacterManager StartCharacter(CharacterManager manager, string name)
    {
        manager = GameObject.Find(name).GetComponent<CharacterManager>();
        manager.moveForward.SetDirection(firstpPlatformManager.findPath.direction);
        manager.moveForward.move = true;
        return manager;
    }

    void LoadLevel()
    {

    }
}
