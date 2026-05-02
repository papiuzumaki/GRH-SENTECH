namespace GRH_SENTECH.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data, string msg = "")
        {
            return new ServiceResult<T> { Success = true, Data = data, Message = msg };
        }

        public static ServiceResult<T> Fail(string msg)
        {
            return new ServiceResult<T> { Success = false, Message = msg };
        }
    }
}
