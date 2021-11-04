using System.Net.Http;

namespace ChatDesign.Helpers
{
    public class Singleton
    {
        private static Singleton instance_;

        public static Singleton Instance()
        {
            if (instance_ == null)
            {
                instance_ = new Singleton();
            }
            return instance_;
        }

        public HttpClient APIClient { get; set; }
        public string EnvironmentPath { get; set; }
    }
}
