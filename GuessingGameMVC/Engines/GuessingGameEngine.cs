using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuessingGameMVC.Engines
{
    /// <summary>
    /// Guessing Game Engine
    /// </summary>
    [Serializable]
    public class GuessingGameEngine
    {
        /// <summary>
        /// Number to guess
        /// </summary>
        public int NumberToGuess { get; private set; }
        /// <summary>
        /// Total Players
        /// </summary>
        public int TotalPlayers { get; set; }
        /// <summary>
        /// Whether game is running.
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// Whether the game is being reset.
        /// </summary>
        public bool IsBeingReset { get; set; }

        /// <summary>
        /// Start New Game
        /// </summary>
        /// <param name="maxValue"></param>
        public void StartNewGame(int maxValue)
        {
            var rand = new Random();
            NumberToGuess = rand.Next(maxValue);
            IsRunning = true;
            IsBeingReset = false;
        }

        /// <summary>
        /// Guess Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GuessValue(int value)
        {
            if (value == NumberToGuess)
                return 0;
            else if (value > NumberToGuess)
                return 1;

            return -1;
        }
    }

}