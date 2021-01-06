namespace AvaloniaMpv.mpv
{
    public class MediaTrack
    {
        public MediaTrackType Type { get; }

        public string Language { get; }

        public int Id { get; }

        public string Title { get; }

        public MediaTrack(MediaTrackType type, string language, int id, string title)
        {
            Type = type;
            Language = language;
            Id = id;
            Title = title;
        }
    }
}