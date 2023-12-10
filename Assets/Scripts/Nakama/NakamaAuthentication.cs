using Nakama;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NakamaAuthentication : MonoBehaviour
{
    [SerializeField] string _scheme = "http", _host = "127.0.0.1", serverkey = "defaultkey";
    [SerializeField] int _port = 7350;

    IClient _client;
    ISession _session;
    ISocket _socket;

    public void Init()
    {
        _client = new Client(_scheme, _host, _port, serverkey);

        AuthenticateWithDevice();
    }

    async void AuthenticateWithDevice()
    {
        var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

        if (deviceId == SystemInfo.unsupportedIdentifier)
        {
            deviceId = System.Guid.NewGuid().ToString();
        }

        PlayerPrefs.SetString("deviceId", deviceId);

        var vars = new Dictionary<string, string>();
        vars["DeviceOS"] = SystemInfo.operatingSystem;
        vars["DeviceModel"] = SystemInfo.deviceModel;
        vars["GameVersion"] = Application.version;

        try
        {
            _session = await _client.AuthenticateDeviceAsync(deviceId, null, true, vars);

            _socket = _client.NewSocket();

            bool appearOnline = true;
            int connectionTimeout = 30;

            await _socket.ConnectAsync(_session, appearOnline, connectionTimeout);

            NakamaManager.Instance.AuthUser(_client, _session, _socket);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error authenticating with Device ID: {0}", ex.Message);
        }
    }
}
