using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hipparu.Shared;
using Microsoft.AspNetCore.Components;
using Plk.Blazor.DragDrop;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;


namespace Hipparu.Pages
{
    public partial class Index : ComponentBase
    {
        // Game states:
        // 0 - Main Menu
        // 1 - Hiragana Mode
        // 2 - Katakana Mode
        // 3 - Mixed Mode
        private int gameState = 0;

        // Text states
        // 0 - Hiragana
        // 1 - Katakana
        private int textState = 0;

        // Styles for the game and menu divs to decide visibilty
        private string menuVisibility = "menu-visible";
        private string gameVisibility = "game-invisible";

        // A list of answers to save us loading from json every time
        // Gets assigned to in OnInitializedAsync()
        private static Answers masterAnswers;

        // List for Answer Items
        public List<AnswerItem> AnswerList = new List<AnswerItem>(){};

        // LastDropped lets us tell the player when they're going wrong
        private AnswerItem LastDropped { get; set; }

        /// <summary>
        /// Run PrepareGame() to get the board ready, then switch case to decide which kana to use
        /// </summary>
        /// <param name="mode">Int provided by button</param>
        private void SelectMode(int mode)
        {
            PrepareGame();

            switch (mode)
            {
                case 1:
                    //set to display hiragana
                    break;

                case 2:
                    //set to display katakana
                    break;

                case 3:
                    //enable mixed game switch
                    break;
            }
        }
        /// <summary>
        /// Get board prepared for a new game
        /// </summary>
        private void PrepareGame()
        {
            SwapBetweenGameAndMenu();
            AnswerList = BuildAnswerList();
            // Reset the game timer
        }
        /// <summary>
        /// Swap between menu being visible and game being hidden. This changes the strings of the class of the divs which the menu and games are located in. 
        /// </summary>
        private void SwapBetweenGameAndMenu()
        {
            if (menuVisibility == "menu-invisible")
            {
                gameState = 0;
                menuVisibility = "menu-visible";
                gameVisibility = "game-invisible";
            }
            else
            {
                //gameState handled by SelectMode
                menuVisibility = "menu-invisible";
                gameVisibility = "game-visible";
            }
        }
        /// <summary>
        /// Take a list of Answer Items (usually masterAnswers) and spit out a shuffled version of the list
        /// </summary>
        /// <typeparam name="AnswerItem"></typeparam>
        /// <param name="list">Answers, usually masterAnswers</param>
        /// <returns></returns>
        public static IList<AnswerItem> Shuffle<AnswerItem>(IList<AnswerItem> list)
        {
            Random rng = new Random();
            var source = list.ToList();
            int n = source.Count;
            var shuffled = new List<AnswerItem>(n);
            shuffled.AddRange(source);
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                AnswerItem value = shuffled[k];
                shuffled[k] = shuffled[n];
                shuffled[n] = value;
            }
            return shuffled;
        }
        /// <summary>
        /// Return a shuffled list of masterAnswers
        /// </summary>
        /// <returns></returns>
        private static List<AnswerItem> BuildAnswerList()
        {
            return (List<AnswerItem>)Shuffle(masterAnswers.Data);
        }
    }
}


