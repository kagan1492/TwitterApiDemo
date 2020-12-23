using System;

namespace TwitterModels
{
    public class TwitterStatModel
    {
        public DateTime LastTweetDate { get; set; }

        public int TotalNumberOfTweetsReceived { get; set; } 
         
        public string TopEmoji { get; set; }

        public string TopHastag { get; set; }

        public string TopDomain { get; set; }

        public decimal PercentOfTweetsWithEmojis  { get; set; }

        public decimal PercentOfTweetsWithUrls { get; set; }

        public decimal PercentOfTweetsWithPhotoUrls { get; set; }

        public decimal AverageTweetsPerSecond { get; set; } 

        public decimal AverageTweetsPerMinute
        {
            get
            {
                if (AverageTweetsPerSecond == 0)
                    return 0;

                return AverageTweetsPerSecond * 60;
            }
        }

        public decimal AverageTweetsPerHour
        {
            get
            {
                if (AverageTweetsPerMinute == 0)
                    return 0;

                return AverageTweetsPerMinute * 60;
            }
        }


    }
}
