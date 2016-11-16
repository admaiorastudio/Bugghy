namespace AdMaiora.Bugghy.Api
{
    using System;

    public enum IssueType
    {
        Any,
        Crash,
        Blocking,
        NonBlocking
    }

    public enum IssueStatus
    {
        Any,
        Opened,
        Evaluating,
        Working,
        Resolved,
        Rejected,
        Closed
    }
}
