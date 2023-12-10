using BaseTemplate.Behaviours;
using Nakama;
using System.Net.Sockets;
using UnityEngine;

public class NakamaManager : MonoSingleton<NakamaManager>
{
    IClient _client;
    ISession _session;
    ISocket _socket;

    [SerializeField] NakamaAuthentication _nakamaAuthentication;
    [SerializeField] NakamaUserAccount _nakamaUserAccount;
    [SerializeField] NakamaMatchManager _nakamaMatchManager;

    public IClient Client { get => _client; }
    public ISession Session { get => _session; }
    public ISocket Socket { get => _socket; }

    public NakamaUserAccount NakamaUserAccount { get => _nakamaUserAccount; }

    public void Init()
    {
        _nakamaAuthentication.Init();
    }

    public async void AuthUser(IClient client, ISession session, ISocket socket)
    {
        _client = client;
        _session = session;
        _socket = socket;

        await _nakamaUserAccount.Init(_client, _session);

        await _nakamaMatchManager.Init();

        GameManager.Instance.UpdateStateToMenu();
    }
}
