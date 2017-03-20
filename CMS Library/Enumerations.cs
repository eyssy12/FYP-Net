namespace CMS.Library
{
    public class Enumerations
    {
        public enum ProcessingStatus : int
        {
            Stopped = 0,
            Started = 1,
            Disposed = 2,
            Stopping = 3,
            Starting = 4,
            Unknown = int.MaxValue
        }
    }
}