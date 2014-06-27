using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using GuessingGameMVC.Engines;
using System.Threading;
using System.Runtime.Caching;

namespace GuessingGameMVC
{
    public class GuessingGameHub : Hub
    {
        /// <summary>
        /// Dummy Lock
        /// </summary>
        private static object dummyLock = new object();
        /// <summary>
        /// Max Guess Value
        /// </summary>
        public const int MAX_GUESS_VALUE = 1000;
        /// <summary>
        /// Countdown to new game in seconds
        /// </summary>
        public const int COUNTDOWN_TO_NEW_GAME = 10;
        /// <summary>
        /// Game Engine
        /// </summary>
        public GuessingGameEngine GameEngine
        {
            get
            {
                var cache = MemoryCache.Default;
                GuessingGameEngine gameEngine = cache["GameEngine"] as GuessingGameEngine;
                if (gameEngine == null)
                    cache["GameEngine"] = gameEngine = new GuessingGameEngine();
                return gameEngine;
            }
        }

        public GuessingGameHub() { }

        public void Register(string username)
        {
            var gameEngine = GameEngine;
            Clients.Caller.Username = username;
            // Send message to everyone announcing new player
            Clients.All.sendMessage(username + " has joined");
            // Update total players 
            gameEngine.TotalPlayers++;
            Clients.All.updateTotalUsers(GameEngine.TotalPlayers);
            // If not running or being reset, start game
            if (!gameEngine.IsRunning && !gameEngine.IsBeingReset)
                StartNewGame(skipCountdown: true);
            // Enable client controls for game
            Clients.Caller.playGame();
        }

        public void Guess(int value)
        {
            bool isCorrect = false;
            var gameEngine = GameEngine;

            // Lock to prevent too many playes from guessing at the same time
            if (gameEngine.IsRunning)
            {
                int guessCheckVal = 0;

                // 1) Announce to everyone else that someone has made a guess
                Clients.AllExcept(Context.ConnectionId).sendMessage(String.Format("{0} has guessed {1}", Clients.Caller.Username, value));

                // 2) Check if it is right
                lock (dummyLock)
                {
                    guessCheckVal = gameEngine.GuessValue(value);
                }

                // 3) If correct, announce that someone guessed correctly
                if (guessCheckVal == 0)
                {
                    isCorrect = true;
                    gameEngine.IsRunning = false;
                    gameEngine.IsBeingReset = true;
                    Clients.All.sendMessage(String.Format("{0} has guessed correctly!", Clients.Caller.Username));
                }
                else if (guessCheckVal > 0)
                    Clients.Caller.sendMessage(String.Format("{0} is not correct, guess lower!", value));
                else
                Clients.Caller.sendMessage(String.Format("{0} is not correct, guess higher!", value));
            }

            // 4) If correct, start new game via countdown
            // Start new game if user guessed correctly or if game was unexpectantly stopped
            if (isCorrect || !gameEngine.IsRunning && !gameEngine.IsBeingReset)
                lock (dummyLock)
                {
                    StartNewGame();
                }
        }

        private void StartNewGame(bool skipCountdown = false)
        {
            var gameEngine = GameEngine;

            // Count down timer for players
            if (!skipCountdown)
                for (int i = 0; i < COUNTDOWN_TO_NEW_GAME; i++)
                {
                    Clients.All.sendMessage(String.Format("New game starting {0} seconds", COUNTDOWN_TO_NEW_GAME - i));
                    Thread.Sleep(1000);
                } // end for block

            Clients.All.sendMessage("Starting new game!");

            gameEngine.StartNewGame(MAX_GUESS_VALUE);
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            var gameEngine = GameEngine;

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            var gameEngine = GameEngine;
            gameEngine.TotalPlayers--;

            return base.OnDisconnected();
        }


    }
}