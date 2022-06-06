# DeepVoice

DeepVoice is a voice only resource for [alt:V](https://altv.mp/)

## Description

DeepVoice is a selfwritten voice resource based on the internal alt:V voice API.
This resource is serverside written in C# and clientside with Typescript.

## Features

- Configurable 3D/2D voice
- Configurable megaphone mode
- Configure as many as wished voice ranges
- Configure keys for each needed function (unless push to talk which is defined in the alt:V client)
- Transactional safe voice activities (local voice range/mute/megaphone is only applied after it is accepeted by the server)
- Build your own UI

## Events

### deepvoice:voice:changed

Server accecpted a requested voice range change.
Parameter: range (number)

### deepvoice:voice:muted

Server accecpted a requested mute.
Parameter: None

### deepvoice:megaphone:enabled

Server accecpted a requested megaphone activation.
Parameter: None

### deepvoice:megaphone:disabled

Server accecpted a requested megaphone deactivation.
Parameter: None

## Usage

- Configure the config file located in `%SERVER_ROOT%/resources/DeepVoice/config.json`.
- Add `DeepVoice` to resources in your `server.cfg`.

## Config

Config is located in `%SERVER_ROOT%/resources/DeepVoice/config.json`.
```json
{
	"version": "1.0.1",
	"voiceRanges": [1, 2, 3, 4],
	"keyMute": 109,
	"keyChange": 107,
	"enableMegaphone": true,
	"keyToggleMegaphone": 44,
	"megaphoneRange": 20,
	"partialAudio": true
}
```

## Debug

Enable the debug option in your `server.cfg`.

## Installation

```
../
resources/
├─ DeepVoice.Client/
│  ├─ *.js
├─ DeepVoice.Server/
│  ├─ *.dll
├─ config.json
├─ resource.cfg
altv-server.exe
AltV.Net.Host.dll
server.cfg
```

## Other
Get in touch with me:

- Twitter: [https://twitter.com/HerrDepence](https://twitter.com/HerrDepence)
- Twitch: [https://twitch.tv/herrdepence](https://twitch.tv/herrdepence)
- Github: [https://github.com/HerrDepence](https://github.com/HerrDepence)