using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hipparu.Shared
{
    public class Answers
    {
        public List<AnswerItem> Data { get; set; }
    }
    public class AnswerItem
    {
        public int Id { get; set; }
        public string RomajiScript { get; set; }
        public string HiraganaScript { get; set; }
        public string KatakanaScript { get; set; }
    }
}
