using Microsoft.AspNetCore.Mvc.ModelBinding; 
using System.Linq;
using Westwind.Utilities;

namespace TwitterDemoAPI.Helpers
{
    public class ApiError
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public string Detail { get; set; }
        public ValidationErrorCollection errors { get; set; }

        public ApiError(string message)
        {
            this.Message = message;
            IsError = true;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            this.IsError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Any()))
                Message = "Please correct the errors and try again";
        }
    }
}
