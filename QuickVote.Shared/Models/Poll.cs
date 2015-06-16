using System;

namespace QuickVoteNative.Models
{
    public class Poll
    {
        public string Id { get; set; }

        public string PollName{ get; set; }

        public string UserName{ get; set; }

        public string ImageUrl{ get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public string Option4 { get; set; }

        public int Option1Count { get; set; }

        public int Option2Count { get; set; }

        public int Option3Count { get; set; }

        public int Option4Count { get; set; }

    }
}

