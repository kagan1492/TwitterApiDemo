using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterDemoAPI.Helpers;
using TwitterModels;
using TwitterServices.Interfaces;

namespace TwitterDemoAPI.Controllers
{
    /// <summary>
    /// Twitter API Demo by Kagan Yilmaz
    /// </summary> 
    [ApiController]
    [Route("[controller]")] 
    [Produces("application/json")]
    [TypeFilter(typeof(ApiExceptionFilter))] 
    public class TwitterController : ControllerBase
    {  
        private readonly ILogger<TwitterController> _logger;
        private readonly ITwitterService _twitterService;
         
        public TwitterController(ILogger<TwitterController> logger, ITwitterService twitterService)
        {
            //_logger = logger; //not needed at the moment - can be used incase we need to log some specific params etc
            _twitterService = twitterService;
        }

        /// <summary> 
        /// /// Gets Twitter statistics
        /// /// </summary>  
        /// /// <returns>TwitterStatModel</returns> 
        /// /// <remarks> 
        /// /// Please run ProcessTweets prior to running this method in order to build the data
        /// /// </remarks>  
        [HttpGet("api/stats")]
        public TwitterStatModel Stats()
        { 
            return _twitterService.GetSampledStreamStats(); 
        }

        /// <summary>
        /// Populates Twitter tweets data in cache 
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/process")]
        public async Task<IEnumerable<string>> Process()
        {
            return await _twitterService.ProcessTweets();
        }
    }
}
