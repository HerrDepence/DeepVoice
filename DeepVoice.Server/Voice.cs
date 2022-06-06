using AltV.Net;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;

namespace DeepVoice;

public class Voice : Resource
{
    private Config _config = null!;

    private readonly IVoiceChannel[] _voiceChannels = { };
    private IVoiceChannel _megaphoneChannel;

    private int _voiceRange;

    public override void OnStart()
    {
        LoadConfig();

        foreach (var range in _config.VoiceRanges) _voiceChannels.Append(Alt.CreateVoiceChannel(_config.PartialAudio, range));

        if (_config.EnableMegaphone)
        {
            _megaphoneChannel = Alt.CreateVoiceChannel(true, _config.MegaphoneRange);
            Alt.OnClient<IPlayer, bool>("deepvoice::megaphone:toggle", MegaphoneToggle);
        }

        Alt.OnClient<IPlayer>("deepvoice::voice:mute", OnVoiceMute);
        Alt.OnClient<IPlayer, int>("deepvoice::voice:change", OnVoiceRangeChanged);

        Alt.Log("[DeepVoice] Started!");
    }

    private void LoadConfig()
    {
        var configFile = Path.Combine(Alt.Resource.Path, "config.json");
        if (!File.Exists(configFile)) throw new FileNotFoundException("Missing config.json");

        try
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
            
            if (config!.KeyChange == -1) Environment.FailFast("Missing config entry for keyChange.");
            if (config!.KeyMute == -1) Environment.FailFast("Missing config entry for keyMute.");
            if (config!.KeyToggleMegaphone == -1) Environment.FailFast("Missing config entry for keyToggleMegaphone.");
            if (config!.VoiceRanges.Length == 0) Environment.FailFast("Missing voiceRanges.");
            
            _config = config!;
            Alt.Log($"[DeepVoice] New status: enabled. Version: {_config.Version}");
        }
        catch (Exception ex)
        {
            Alt.Log("[DeepVoice] Failed loading configuration from config.json: " + ex);
            Environment.FailFast("Failed loading configuration from config.json", ex);
        }
    }

    public override void OnStop()
    {
        foreach (var voiceChannel in _voiceChannels) voiceChannel.Remove();

        Alt.Emit("deepvoice:voice:change", 0);
    }

    [ScriptEvent(ScriptEventType.PlayerBeforeConnect)]
    private void OnBeforePlayerConnect(IPlayer player, string reason)
    {
        Alt.EmitClients(new[] { player }, "deepvoice:debug");
    }

    [ScriptEvent(ScriptEventType.PlayerConnect)]
    private void OnPlayerConnect(IPlayer player)
    {
        foreach (var voiceChannel in _voiceChannels)
        {
            voiceChannel.AddPlayer(player);
            voiceChannel.MutePlayer(player);
        }
    }

    [ScriptEvent(ScriptEventType.PlayerDisconnect)]
    public void OnPlayerDisconnect(IPlayer player, string reason)
    {
        RemoveFromVoiceChannels(player);
    }

    private void OnVoiceMute(IPlayer player)
    {
        foreach (var voiceChannel in _voiceChannels) voiceChannel.MutePlayer(player);

        Alt.EmitClients(new[] { player }, "deepvoice:voice:muted");
    }

    private void OnVoiceRangeChanged(IPlayer player, int range)
    {
        var foundMatchingRange = false;
        foreach (var voiceChannel in _voiceChannels) voiceChannel.MutePlayer(player);

        foreach (var voiceChannel in _voiceChannels)
        {
            var distance = voiceChannel.MaxDistance;
            if (Math.Abs(range - distance) < 0)
            {
                voiceChannel.UnmutePlayer(player);
                foundMatchingRange = true;
                break;
            }
        }

        if (foundMatchingRange)
        {
            _voiceRange = range;
            if (Alt.Core.IsDebug) Alt.Log("[DeepVoice] Voice range changed to " + _voiceRange);
            Alt.EmitClients(new[] { player }, "deepvoice:voice:change", _voiceRange);
        }
        else
        {
            if (Alt.Core.IsDebug) Alt.Log("[DeepVoice] Please keep your voice ranges in sync.");
            Alt.EmitClients(new[] { player }, "deepvoice:voice:muted");
        }
    }

    private void RemoveFromVoiceChannels(IPlayer player)
    {
        foreach (var voiceChannel in _voiceChannels)
            if (voiceChannel.HasPlayer(player))
                voiceChannel.RemovePlayer(player);
    }
    
    private void MegaphoneToggle(IPlayer player, bool enabled)
    {
        if (enabled)
        {
            _megaphoneChannel.MutePlayer(player);
            OnVoiceRangeChanged(player, _voiceRange);
            Alt.EmitClients(new []{player}, "deepvoice:megaphone:disabled");
        }
        else
        {
            foreach (var voiceChannel in _voiceChannels) voiceChannel.MutePlayer(player);
            _megaphoneChannel.UnmutePlayer(player);
            Alt.EmitClients(new []{player}, "deepvoice:megaphone:enabled");
        }
    }
}