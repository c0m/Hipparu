using System;
using System.Linq;

namespace Hipparu.Data
{
    public class PersonalBestTable
    {
        private TimeSpan[] personalBests;

        private void Initialize()
        {
            personalBests = Enumerable.Repeat(TimeSpan.Zero, 5).ToArray();
        }

        private void NewScore(TimeSpan newScore)
        {
            for (int i = 0; i < personalBests.Length; i++)
            {
                if (newScore < personalBests[i] || personalBests[i] == TimeSpan.Zero)
                {
                    personalBests[i] = newScore;
                    return;
                }
            }
        }

        private TimeSpan[] GetPersonalBests()
        {
            return personalBests;
        }
    }
}
