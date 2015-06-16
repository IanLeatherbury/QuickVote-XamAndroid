using System;
using QuickVoteNative.Adapters;
using System.Collections.Generic;

namespace QuickVoteNative.ViewModels
{
    public class PollViewModel : Java.Lang.Object
    {
        string m_Image;
        public string Image
        {
            get { return this.m_Image; }
            set { this.m_Image = value; }
        }


        private long m_Id;
        /// <summary>
        /// Gets or sets the unique ID for the menu
        /// </summary>
        public long Id
        {
            get { return this.m_Id; }
            set { this.m_Id = value; }
        }

        public string pollId;
        public string PollId
        {
            get { return pollId; }
            set { pollId = value; }
        }

        private string m_Title = string.Empty;
        /// <summary>
        /// Gets or sets the name of the menu
        /// </summary>
        public string Title
        {
            get { return this.m_Title; }
            set { this.m_Title = value; }
        }

        public void Init(int id, string title, string image)
        {
            this.Id = id;
            this.Title = title;
            this.Image = image;
            this.Items.RemoveRange(0, this.Items.Count - 2);
        }

        string userName;
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                userName = value;
            }
        }

        string optionOne;
        public string OptionOne
        {
            get
            {
                return this.optionOne;
            }
            set
            {
                optionOne = value;
            }
        }

        string optionTwo;
        string optionThree;
        string optionFour;      

        public string OptionTwo
        {
            get
            {
                return this.optionTwo;
            }
            set
            {
                optionTwo = value;
            }
        }

        public string OptionThree
        {
            get
            {
                return this.optionThree;
            }
            set
            {
                optionThree = value;
            }
        }

        public string OptionFour
        {
            get
            {
                return this.optionFour;
            }
            set
            {
                optionFour = value;
            }
        }

        string hasVoted;
        public string HasVoted
        {
            get
            {
                return this.hasVoted;
            }
            set
            {
                hasVoted = value;
            }
        }   

        private List<PollViewModel> m_Items;
        public List<PollViewModel> Items
        {
            get { return this.m_Items; }
            set { this.m_Items = value; }
        }
    }
}

