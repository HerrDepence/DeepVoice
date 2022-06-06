namespace DeepVoice;

public class Config
{
    public string Version { get; set; } = null!;
    public int[] VoiceRanges { get; set; } = null!;
    public int KeyMute { get; set; }
    public int KeyChange { get; set; }
    public bool EnableMegaphone { get; set; }
    public int KeyToggleMegaphone { get; set; }
    public int MegaphoneRange { get; set; }
}