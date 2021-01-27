using Hipparu.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hipparu.Pages
{
    public partial class Index
    {
        public enum GameModes
        {
            Hiragana,
            Katakana
        }

        List<AnswerItem> AnswerListB = new List<AnswerItem>();
        TimeSpan exerciseTimer = new TimeSpan();
        bool isTimerRunning = false;

        private AnswerItem LastDropped { get; set; }
        public IList<AnswerItem> AnswerList;

        private void ResetGame()
        {
            AnswerList.Clear();
            AnswerList = BuildAnswerList();
            foreach (IList<AnswerItem> sublist in ListOfAnswerLists)
            {
                sublist.Clear();
            }
            AnswerListB = AnswerList as List<AnswerItem>;
            exerciseTimer = new TimeSpan();
        }

        private void SuccessfulDrop()
        {
            LastDropped = null;
            if (AnswerList.Count == 0)
            {
                WinGame();
            }
        }

        private void WinGame()
        {
            // send the user to the personal bests page and send along their current time to see if they've reached a personal best
            // for now, just reset the game
            ResetGame();
        }

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

        private static IList<AnswerItem> BuildAnswerList()
        {
            string json = File.ReadAllText("./Data/CharacterList.json");
            Answers answers = JsonConvert.DeserializeObject<Answers>(json);
            return Shuffle<AnswerItem>(answers.Data);
        }

        async Task TimerTask()
        {
            isTimerRunning = true;
            while (isTimerRunning)
            {
                await Task.Delay(1000);
                exerciseTimer = exerciseTimer.Add(new TimeSpan(0, 0, 1));
                StateHasChanged();
            }
        }
        protected override async Task OnInitializedAsync()
        {
            TimerTask();
        }

        // This is horrifying but we're gonna do it anyway because Dropzone wants lists
        #region Dropzone Lists
        // Misc group
        public static List<AnswerItem> WaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> WoList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> NList = new List<AnswerItem>()
        {
        };

        // R group
        public static List<AnswerItem> RaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> RiList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> RuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> ReList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> RoList = new List<AnswerItem>()
        {
        };

        // Y group
        public static List<AnswerItem> YaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> YuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> YoList = new List<AnswerItem>()
        {
        };

        // M group
        public static List<AnswerItem> MaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> MiList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> MuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> MeList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> MoList = new List<AnswerItem>()
        {
        };

        // H group
        public static List<AnswerItem> HaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> HiList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> FuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> HeList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> HoList = new List<AnswerItem>()
        {
        };

        // N group
        public static List<AnswerItem> NaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> NiList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> NuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> NeList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> NoList = new List<AnswerItem>()
        {
        };

        // T group
        public static List<AnswerItem> TaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> ChiList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> TsuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> TeList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> ToList = new List<AnswerItem>()
        {
        };

        // S group
        public static List<AnswerItem> SaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> ShiList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> SuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> SeList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> SoList = new List<AnswerItem>()
        {
        };

        // K group
        public static List<AnswerItem> KaList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> KiList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> KuList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> KeList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> KoList = new List<AnswerItem>()
        {
        };

        // Vowel group
        public static List<AnswerItem> AList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> IList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> UList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> EList = new List<AnswerItem>()
        {
        };

        public static List<AnswerItem> OList = new List<AnswerItem>()
        {
        };
        #endregion

        // It would be nicer to add these all to the list as the lists are getting made
        // But it's not a fan for some reason, so here's some more silliness.
        public List<List<AnswerItem>> ListOfAnswerLists = new List<List<AnswerItem>>()
    {
        WaList,
        WoList,
        NList,
        RaList,
        RiList,
        RuList,
        ReList,
        RoList,
        YaList,
        YuList,
        YoList,
        MaList,
        MiList,
        MuList,
        MeList,
        MoList,
        HaList,
        HiList,
        FuList,
        HeList,
        HoList,
        NaList,
        NiList,
        NuList,
        NoList,
        TaList,
        ChiList,
        TsuList,
        TeList,
        ToList,
        SaList,
        ShiList,
        SuList,
        SeList,
        SoList,
        KaList,
        KiList,
        KuList,
        KeList,
        KoList,
        AList,
        IList,
        UList,
        EList,
        OList
    };

    }

}