
using Nakama.TinyJson;
using System.Collections.Generic;
using UnityEngine;

public static class MatchDataJson
{
    /// <summary>
    /// Creates a network message containing velocity and position.
    /// </summary>
    /// <param name="velocity">The velocity to send.</param>
    /// <param name="position">The position to send.</param>
    /// <returns>A JSONified string containing velocity and position data.</returns>
    public static string VelocityAndPosition(Vector2 velocity, Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "velocity.x", velocity.x.ToString() },
            { "velocity.y", velocity.y.ToString() },
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message containing player input.
    /// </summary>
    /// <param name="horizontalInput">The current horizontal input.</param>
    /// <param name="jump">The jump input.</param>
    /// <param name="jumpHeld">The jump held input.</param>
    /// <param name="attack">The attack input.</param>
    /// <returns>A JSONified string containing player input.</returns>
    public static string Input(float horizontalInput, bool jump, bool jumpHeld, bool attack)
    {
        var values = new Dictionary<string, string>
        {
            { "horizontalInput", horizontalInput.ToString() },
            { "jump", jump.ToString() },
            { "jumpHeld", jumpHeld.ToString() },
            { "attack", attack.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message specifying that the player died and the position when they died.
    /// </summary>
    /// <param name="position">The position on death.</param>
    /// <returns>A JSONified string containing the player's position on death.</returns>
    public static string Died(Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message specifying that the player respawned and at what spawn point.
    /// </summary>
    /// <param name="spawnIndex">The spawn point.</param>
    /// <returns>A JSONified string containing the player's respawn point.</returns>
    public static string Respawned(int spawnIndex)
    {
        var values = new Dictionary<string, string>
        {
            { "spawnIndex", spawnIndex.ToString() },
        };

        return values.ToJson();
    }

    /// <summary>
    /// Creates a network message indicating a new round should begin and who won the previous round.
    /// </summary>
    /// <param name="winnerPlayerName">The winning player's name.</param>
    /// <returns>A JSONified string containing the winning players name.</returns>
    public static string StartNewRound(string winnerPlayerName)
    {
        var values = new Dictionary<string, string>
        {
            { "winningPlayerName", winnerPlayerName }
        };

        return values.ToJson();
    }
}
