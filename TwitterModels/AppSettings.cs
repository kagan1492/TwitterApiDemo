namespace TwitterModels
{
    public class AppSettings
    {
        public string TWITTER_CONSUMER_KEY { get; set; }
        public string TWITTER_CONSUMER_SECRET { get; set; }
        public string TWITTER_ACCESS_TOKEN { get; set; } //not needed at the moment -it is required for posting tweetes / user api 
        public string TWITTER_ACCESS_TOKEN_SECRET { get; set; } //not needed at the moment -it is required for posting tweetes / user api 
    }
}
