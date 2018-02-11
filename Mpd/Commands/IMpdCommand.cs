namespace Orpheus.Mpd.Commands
{
    interface IMpdCommand <TResponse>
    {
        string Command { get; set; }
        TResponse Response { get; set; }

        TResponse Parse(MpdResponse response);
    }
}
