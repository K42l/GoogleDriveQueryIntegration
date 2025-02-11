using Google.Drive.Query.Integration.Util;

namespace Google.Drive.Query.Integration.Query
{
    public abstract class TermsBase
    {
        public AndOrEnum AndOr { get; set; }
        public bool EncapsulateWithNext { get; set; } = false;
        public DateTime CreatedTime { get; private set; } = DateTime.Now;
        public bool NegateSearchQuery { get; set; } = false;

        public enum AndOrEnum
        {
            [StringValue("and")]
            And,

            [StringValue("or")]
            Or
        }
    }
}
