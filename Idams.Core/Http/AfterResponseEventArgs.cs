namespace Idams.Core.Http;

public class AfterResponseEventArgs : EventArgs
{
    public HttpResponseMessage Response { get; internal set; }
}