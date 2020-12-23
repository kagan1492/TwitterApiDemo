using System.Collections.Generic;
using System.Threading.Tasks; 
using TwitterModels;

namespace TwitterServices.Interfaces
{
    public interface ITwitterService
    {
        TwitterStatModel GetSampledStreamStats();
        Task<IEnumerable<string>> ProcessTweets();
    }
}
