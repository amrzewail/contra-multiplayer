using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkManager
{
    public struct CreateCharacterMessage : NetworkMessage
    {
        public Color color;
    }

    public enum Color
    {
        Blue,
        Red,
        Yellow,
        Green
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCharacterMessage>(CreateCharacterCallback);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();


        SceneManager.sceneLoaded += SceneLoadedCallback;

        SceneManager.LoadScene(1);

    }

    private void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= SceneLoadedCallback;

        // you can send the message here, or wherever else you want
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            color = (Color)Random.Range(0, 4)
        };

        NetworkClient.Send(characterMessage);
    }

    void CreateCharacterCallback(NetworkConnection conn, CreateCharacterMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject playerObj = Instantiate(playerPrefab);
        playerObj.transform.position = LevelController.instance.GetSpawnLocation().position;
        playerObj.GetComponent<PlayerIdentity>().SetColor(new UnityEngine.Color(
                                                        Random.Range(0f, 1f),
                                                        Random.Range(0f, 1f),
                                                        Random.Range(0f, 1f)
                                                    ));
        playerObj.GetComponent<PlayerIdentity>().SetName($"Player {NetworkServer.connections.Count}");
        //switch (message.color)
        //{
        //    case Color.Blue:
        //        break;
        //    case Color.Green:
        //        playerObj.GetComponentInChildren<SpriteRenderer>().color = UnityEngine.Color.green;
        //        break;
        //    case Color.Yellow:
        //        playerObj.GetComponentInChildren<SpriteRenderer>().color = UnityEngine.Color.yellow;
        //        break;
        //    case Color.Red:
        //        playerObj.GetComponentInChildren<SpriteRenderer>().color = UnityEngine.Color.red;
        //        break;
        //}

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, playerObj);
    }
}
