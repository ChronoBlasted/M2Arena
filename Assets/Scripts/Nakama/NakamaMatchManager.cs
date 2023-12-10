using Nakama;
using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEditor.Experimental.GraphView.GraphView;

public class NakamaMatchManager : MonoBehaviour
{
    ISocket _socket;
    IMatch _match;

    IUserPresence _localUser;
    IDictionary<string, GameObject> players;
    GameObject _localPlayer;

    [SerializeField] List<Transform> _spawnPoints;

    string _currentMatchmakingTicket;

    public async Task Init()
    {
        var mainThread = UnityMainThreadDispatcher.Instance();

        _socket = NakamaManager.Instance.Socket;

        _socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnReceivedMatchmakerMatched(m));
        _socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
        _socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));
    }

    #region Event

    async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        _localUser = matched.Self.Presence;

        var match = await _socket.JoinMatchAsync(matched);

        GameManager.Instance.UpdateStateToGame();

        /*        foreach (var user in match.Presences)
                {
                    SpawnPlayer(match.Id, user);
                }*/

        _match = match;
    }

    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        /*            foreach (var user in matchPresenceEvent.Joins)
                    {
                        SpawnPlayer(matchPresenceEvent.MatchId, user);
                    }*/

        foreach (var user in matchPresenceEvent.Leaves)
        {
            if (players.ContainsKey(user.SessionId))
            {
                Destroy(players[user.SessionId]);
                players.Remove(user.SessionId);
            }
        }
    }

    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        var userSessionId = matchState.UserPresence.SessionId;

        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case OpCodes.Died:
                var playerToDestroy = players[userSessionId];
                Destroy(playerToDestroy, 0.5f);
                players.Remove(userSessionId);

                if (players.Count == 1 && players.First().Key == _localUser.SessionId)
                {
                    //AnnounceWinnerAndStartNewRound();
                }
                break;
            case OpCodes.Respawned:
                //SpawnPlayer(currentMatch.Id, matchState.UserPresence, int.Parse(state["spawnIndex"]));
                break;
            case OpCodes.NewRound:
                await AnnounceWinnerAndRespawn(state["winningPlayerName"]);
                break;
            default:
                break;
        }
    }

    #endregion

    public async Task FindCasualMatch()
    {
        var minPlayers = 2;
        var maxPlayers = 8;
        var query = "+mode:casual";
        var stringProperties = new Dictionary<string, string> { { "mode", "casual" } };
        //var numericProperties = new Dictionary<string, double> { { "skill", 125 } }; 
        var matchmakerTicket = await _socket.AddMatchmakerAsync(query, minPlayers, maxPlayers, stringProperties);

        _currentMatchmakingTicket = matchmakerTicket.Ticket;
    }

    public void CancelMatchmaking()
    {
        _socket.RemoveMatchmakerAsync(_currentMatchmakingTicket);
    }

    public async Task QuitMatch()
    {
        await _socket.LeaveMatchAsync(_match);

        _match = null;
        _localUser = null;

        foreach (var player in players.Values)
        {
            Destroy(player);
        }

        players.Clear();

        GameManager.Instance.UpdateStateToMenu();
    }

    private async Task AnnounceWinnerAndRespawn(string winningPlayerName)
    {
        Debug.Log(string.Format("{0} won this round!", winningPlayerName));

        await Task.Delay(2000);


        players.Remove(_localUser.SessionId);
        Destroy(_localPlayer);

        var spawnIndex = Random.Range(0, _spawnPoints.Count);
        //SpawnPlayer(currentMatch.Id, localUser, spawnIndex);

        SendMatchState(OpCodes.Respawned, MatchDataJson.Respawned(spawnIndex));
    }

    public async Task SendMatchStateAsync(long opCode, string state)
    {
        await _socket.SendMatchStateAsync(_match.Id, opCode, state);
    }

    public void SendMatchState(long opCode, string state)
    {
        _socket.SendMatchStateAsync(_match.Id, opCode, state);
    }
}
