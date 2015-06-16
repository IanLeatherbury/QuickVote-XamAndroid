using System;
using QuickVoteNative.Models;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;
using System.Threading.Tasks;
using QuickVoteNative.ViewModels;
using QuickVoteNative.Activities;
using QuickVote.Shared;
using Android.App;

namespace QuickVoteNative.Adapters
{
    public static class Util
    {
        static List<Poll> allPolls;
        static List<MyPolls> pollsAlreadyVotedIn;

        public static MobileServiceClient Client = new MobileServiceClient(
                                                       Constants.Url, Constants.Key);

        public static async Task<List<Poll>> GetMostRecentPublicPollsNotVotedIn()
        {
            try
            {
                allPolls = await Client.GetTable<Poll>().ToListAsync();
                pollsAlreadyVotedIn = await Client.GetTable<MyPolls>().Where(n => n.UserName == MainActivity.UserNameForPoll).ToListAsync();//.ToListAsync();

                //var result = peopleList2.Where(p => !peopleList1.Any(p2 => p2.ID == p.ID));
                var pollsNotVotedIn = allPolls.Where(p => !pollsAlreadyVotedIn.Any(av => av.PollId == p.Id)).ToList();
                return pollsNotVotedIn;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Azure error pulling polls" + exc);
            }
            return new List<Poll>();
        }

        public static async Task<List<PollViewModel>> GetPolls()
        {
            var polls = await GetMostRecentPublicPollsNotVotedIn();

            List<PollViewModel> pollViewModelList = new List<PollViewModel>();


            pollViewModelList.AddRange(polls.Select(p => new PollViewModel
                    { 
                        Title = p.PollName, 
                        Image = "http://www.gravatar.com/avatar/2350a81a60c24ca4edb56404d70bb93e.png",//MainActivity.ImgUrl,
                        UserName = p.UserName,
                        OptionOne = p.Option1, 
                        OptionTwo = p.Option2, 
                        OptionThree = p.Option3, 
                        OptionFour = p.Option4,
                        PollId = p.Id
                    }).ToArray());

            return pollViewModelList;
        }
    }
}


