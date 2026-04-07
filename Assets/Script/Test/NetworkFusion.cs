using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
public class NetworkFusion : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner runner;

    private const string ROOM_NAME = "TestRoom";
    private const int MAX_PLAYERS = 2;

    private List<SessionInfo> cachedSessions = new List<SessionInfo>();
    private bool isMatching = false;

    public async void StartGame()
    {
        if (isMatching) return;
        isMatching = true;

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        Debug.Log("🔍 Join Lobby...");

        // 🔥 vào lobby để lấy danh sách room
        await runner.JoinSessionLobby(SessionLobby.Shared);

        // 🔥 đợi 3 giây rồi mới quyết định
        StartCoroutine(DelayedMatch());
    }

    IEnumerator DelayedMatch()
    {
        Debug.Log("⏳ Đợi 3 giây tìm phòng...");
        yield return new WaitForSeconds(3f);

        // 👉 tìm room có thể join
        foreach (var session in cachedSessions)
        {
            if (session.Name == ROOM_NAME && session.PlayerCount < session.MaxPlayers)
            {
                Debug.Log("👉 Join phòng có sẵn");

                StartRunner(session.Name);
                yield break;
            }
        }

        // ❌ không có → tạo host
        Debug.Log("👉 Không có phòng → tạo host");

        StartRunner(ROOM_NAME);
    }

    async void StartRunner(string roomName)
    {
        var scene = SceneRef.FromIndex(1);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            PlayerCount = MAX_PLAYERS,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    // 🔥 lưu session list (KHÔNG xử lý tại đây)
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        cachedSessions = sessionList;
        Debug.Log($"📡 Session updated: {sessionList.Count}");
    }

    // 🔥 sau khi load scene → gửi lineup
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene Loaded → Send Lineup");

        Invoke(nameof(SendLineup), 0.5f);
    }

    void SendLineup()
    {
        var lineup = Lineup.Instance.GetNetworkLineup();

        BattleManager.Instance.RPC_SendLineup(lineup);
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player joined: {player}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player left: {player}");
    }

    // ===== giữ nguyên =====
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        request.Accept();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}