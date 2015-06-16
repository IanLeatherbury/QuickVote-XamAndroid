using System;

namespace QuickVote.Shared
{
    public class PollResponse
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

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

