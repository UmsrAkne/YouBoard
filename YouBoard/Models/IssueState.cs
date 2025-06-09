namespace YouBoard.Models
{
    public enum IssueState
    {
        #pragma warning disable SA1602

        Created = 0,
        Complete,
        Paused,
        Working,
        Obsolete,

        #pragma warning restore SA1602
    }
}