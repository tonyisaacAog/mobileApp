namespace CompanyApi.DTOs.ResponseDtos
{
    public class ValidationResults
    {
        public string PropertyName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public object? AttemptedValue { get; set; }
        public string ErrorCode { get; set; } = string.Empty;

        public ValidationResults() { }

        public ValidationResults(string propertyName, string errorMessage, object? attemptedValue = null, string errorCode = "")
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            AttemptedValue = attemptedValue;
            ErrorCode = errorCode;
        }
    }
}
