namespace Orpheus.Mpd.Commands
{
    public interface IMpdCommand <TResponse>
    {
        string Command { get; set; }
        TResponse Response { get; set; }

        TResponse Parse(MpdResponse response);
    }
}
