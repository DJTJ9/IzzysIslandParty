
public interface IQuickTimeEvent
{
    public bool QTERunning { get; }
    public bool QTEFinishedSuccessfully { get; }

    public void StartQTE();
}

