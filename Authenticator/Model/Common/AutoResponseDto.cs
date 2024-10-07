using System.Net;
using System.Text.Json.Serialization;

namespace Authenticator.Model.Common
{
    public class AutoResponseDto<T>
    {
        public AutoResponseDto()
        {
            Warnings = new List<string>();
            Errors = new List<string>();
            Success = true;
            StatusCode = HttpStatusCode.OK;
        }

        public bool Success { get; set; }
        public T Result { get; set; }
        public string Message { get; set; }
        public List<string> Warnings { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        public string Exception { get; set; }

        private List<string> _error;
        public List<string> Errors
        {

            get
            {
                return _error;
            }
            set
            {
                _error = value;

                if (value != null && value.Count > 0)
                    Success = false;
            }
        }

        public void AddError(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                Errors.Add(error);
                Success = false;
            }
        }

        public void AddWarning(string content)
        {
            Warnings.Add(content);
        }

        public void AddErrors(List<string> errorList)
        {
            Errors.AddRange(errorList);
            Success = false;
        }

        public void AddMessage(string content)
        {
            Message = content;
        }

        public void SetStatusCode(HttpStatusCode code)
        {
            StatusCode = code;
        }
    }
}
