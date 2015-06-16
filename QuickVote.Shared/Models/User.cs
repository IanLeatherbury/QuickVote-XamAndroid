using System;
using Android.Graphics;
using System.Collections.Generic;

namespace QuickVoteNative.Models
{
    public class User
    {
        public string Id { get; set; }

        public string FbId { get; set; }

        public string Name { get; set; }

        public Bitmap ProfilePicture { get; set; }

        public List<string> MyPolls { get; set; }
    }
}

