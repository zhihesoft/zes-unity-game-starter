namespace Zes
{
    public class HttpResult
    {
        public HttpResult(int error, string message)
        {
            this.error = error;
            this.message = message;
        }

        public HttpResult(string data)
        {
            this.data = data;
        }

        public int error;
        public string message;
        public string data;
    }
}
