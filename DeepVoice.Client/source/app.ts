// AltV
import * as alt from "alt-client";
import {IConfig} from "./IConfig";

const config: IConfig = require("./../config.json");

export class Voice {

    private debug: boolean = false;

    private voiceRangeIndex: number = 0;
    private configVoiceRanges: number[] = config.voiceRanges;
    private readonly configMegaphoneRange: number = config.megaphoneRange;
    private megaphoneEnabled: boolean = true;

    private readonly configKeyMute: number = config.keyMute;
    private readonly configKeyChange: number = config.keyChange;
    private readonly configKeyToggleMegaphone: number = config.keyToggleMegaphone;

    private muted: boolean = true;

    constructor() {
        alt.onServer("deepvoice:debug", () => this.debug = true);
        alt.onServer("deepvoice:voice:muted", this.serverAcceptedMute.bind(this))
        alt.onServer("deepvoice:voice:changed", this.serverAcceptedRangeChange.bind(this));
        
        alt.onServer("deepvoice:megaphone:disabled", () => this.megaphoneEnabled = false);
        alt.onServer("deepvoice:megaphone:enabled", () => this.megaphoneEnabled = true);
        
        alt.on("keydown", this.processKeyDownEvent.bind(this));
        alt.on("keypress", this.processKeyPressEvent.bind(this));

        if (this.debug) alt.log("[DeepVoice] Started!")
    }
    
    private processKeyDownEvent(key: number) {
        if (key === this.configKeyChange) { // Num+
            this.voiceRangeChanged();
        } else if (key === this.configKeyMute) { // Num-
            this.mutePlayer();
        }
    }
    
    private processKeyPressEvent(key: number) {
        if (key == this.configKeyToggleMegaphone && this.configMegaphoneRange) {
            alt.emitServer("deepvoice::megaphone:toggle", alt.LocalPlayer, this.megaphoneEnabled);
        }
    }

    private voiceRangeChanged() {
        let voiceRangeIndex = this.voiceRangeIndex;
        voiceRangeIndex++;
        if (voiceRangeIndex >= this.configVoiceRanges.length) {
            voiceRangeIndex = 0;
        }

        alt.emitServer("deepvoice::voice:change", this.configVoiceRanges[voiceRangeIndex])
    }
    
    private serverAcceptedRangeChange(range: number) {
        const newIndex = this.configVoiceRanges.indexOf(range);
        if (newIndex == -1) {
            this.voiceRangeIndex = 0;
            this.mutePlayer();
            throw new Error("[DeepVoice] Please keep your voice ranges in sync.");
        } else {
            this.voiceRangeIndex = newIndex;
            alt.emit("deepvoice::voice:changed");
            if (this.debug) alt.log("Voice range changed to " + this.configVoiceRanges[this.voiceRangeIndex])
        }
    }

    private mutePlayer(): void {
        alt.emitServer("deepvoice::voice:mute");
    }
    
    private serverAcceptedMute(): void {
        this.muted = !this.muted;
        alt.emit("deepvoice:voice:muted");

        if (this.debug) {
            if (this.muted) {
                alt.log("[DeepVoice] Muted");
            } else {
                alt.log("[DeepVoice] Unmuted");
            }
        }
    }
}

export const AppInstance = new Voice();
