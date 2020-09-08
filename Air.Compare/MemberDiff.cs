namespace Air.Compare
{
    public class MemberDiff
    {
        public string LeftMember { get;}
        public object LeftValue { get; }
        public string RightMember { get; }
        public object RightValue { get; }

        public string Details { get; }

        public MemberDiff(
            string leftMember, 
            object leftValue,
            string rightMember,
            object rightValue,
            string details)
        {
            LeftMember = leftMember;
            LeftValue = leftValue;
            RightMember = rightMember;
            RightValue = rightValue;
            Details = details;
        }
    }
}
