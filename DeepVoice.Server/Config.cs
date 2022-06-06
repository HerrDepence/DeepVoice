namespace DeepVoice;

public class Config
{
    public string Version { get; set; } = "";
    public int[] VoiceRanges { get; set; } = {};
    public int KeyMute { get; set; } = -1;
    public int KeyChange { get; set; } = -1;
    public bool EnableMegaphone { get; set; } = false;
    public int KeyToggleMegaphone { get; set; } = -1;
    public int MegaphoneRange { get; set; } = -1;
    public bool PartialAudio { get; set; } = true;
}