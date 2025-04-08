namespace TestProjectAnnur.Data.DTOs
{
    public class ResponseDTO
    {
    }

    public class SaveResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public SaveResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
