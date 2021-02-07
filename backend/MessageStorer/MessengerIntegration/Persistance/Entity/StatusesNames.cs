namespace MessengerIntegration.Persistance.Entity
{
    public partial class Statuses
    {
        public static readonly string Created = "Created";
        public static readonly string Queued = "Queued";
        public static readonly string ErrorInvalidFile = "Error_InvalidFile";
        public static readonly string ErrorNoMessages = "Error_NoMessages";     
        public static readonly string Processing = "Processing";
        public static readonly string Completed = "Completed";
        public static readonly string ErrorUnknownError = "Error_UnknownError";
    }
}
