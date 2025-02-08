using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wordapp
{
    public class GameHub : Hub
    {
        public async Task BroadcastGameState(object gameState)
        {
            await Clients.Others.SendAsync("ReceiveGameState", gameState);
        }

        public async Task BroadcastUserInput(int index, string input)
        {
            await Clients.Others.SendAsync("ReceiveUserInput", index, input);
        }

        public async Task BroadcastAnimation(int index, string animationType)
        {
            await Clients.Others.SendAsync("ReceiveAnimation", index, animationType);
        }

        public async Task SubmitWord(int playerIndex, string word)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(word))
                    throw new ArgumentException("Word cannot be empty.");

                await Clients.Others.SendAsync("PlayerSubmittedWord", playerIndex, word);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public async Task JoinLobby(string lobbyId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
                Console.WriteLine($"Client {Context.ConnectionId} joined lobby {lobbyId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining lobby: {ex.Message}");
                throw;
            }
        }

        public async Task NotifyPlayerJoined(string lobbyId, Player player)
        {
            try
            {
                Console.WriteLine($"Broadcasting player joined to lobby {lobbyId}: {player.Username}");
                await Clients.Group(lobbyId).SendAsync("PlayerJoined", player);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error notifying player joined: {ex.Message}");
                throw;
            }
        }

        public async Task StartGame(string roomId)
        {
            await Clients.Group(roomId).SendAsync("RedirectToGame");
        }

        public async Task BroadcastGameStart()
        {
            await Clients.All.SendAsync("ReceiveGameStart");
        }

        public async Task BroadcastDifficulty(string roomId, string difficulty)
        {
            await Clients.Group(roomId).SendAsync("ReceiveDifficultyUpdate", difficulty);
        }

        public async Task BroadcastTimerSync(double remainingTime)
        {
            await Clients.Others.SendAsync("ReceiveTimerSync", remainingTime);
        }

        public async Task BroadcastTimerStart(double initialTime)
        {
            await Clients.Others.SendAsync("ReceiveTimerStart", initialTime);
        }
        
    }
}