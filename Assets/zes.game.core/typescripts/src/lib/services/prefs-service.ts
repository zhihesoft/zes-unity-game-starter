import { UnityEngine } from "csharp";
import { singleton } from "tsyringe";
import { getLogger } from "../logger";
import { isNullOrEmpty } from "../util";

import PlayerPrefs = UnityEngine.PlayerPrefs;


@singleton()
export class PrefsService {

    readonly systemUser = "system";

    private user = "";

    setUser(user: string) {
        this.user = user;
        logger.info(`prefs set to user (${user})`);
    }

    private readonly key_of_last_server_id = "last-server-id";
    get lastServerId() { return this.get(this.key_of_last_server_id, 0); }
    set lastServerId(value: number) { this.set(this.key_of_last_server_id, value); }

    get(key: string, defaultValue: string): string;
    get(key: string, defaultValue: number): number;
    get(key: string, defaultValue: boolean): boolean;
    get(key: string, defaultValue: string | number | boolean): string | number | boolean {
        return this.getOfUser(this.user, key, defaultValue);
    }

    getSystem(key: string, defaultValue: string): string;
    getSystem(key: string, defaultValue: number): number;
    getSystem(key: string, defaultValue: boolean): boolean;
    getSystem(key: string, defaultValue: string | number | boolean): string | number | boolean {
        return this.getOfUser(this.systemUser, key, defaultValue);
    }


    set(key: string, value: string | number | boolean) {
        return this.setOfUser(this.user, key, value);
    }

    setSystem(key: string, value: string | number | boolean) {
        return this.setOfUser(this.systemUser, key, value);
    }

    private getOfUser(user: string, key: string, defaultValue: string | number | boolean): string | number | boolean {
        key = this.getKey(user, key);
        if (typeof defaultValue == "string") {
            return PlayerPrefs.GetString(key, defaultValue);
        } else if (typeof defaultValue == "boolean") {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
        } else {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }

    private setOfUser(user: string, key: string, value: string | number | boolean) {
        key = this.getKey(user, key);
        if (typeof value == "string") {
            PlayerPrefs.SetString(key, value);
        } else if (typeof value == "boolean") {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        } else {
            PlayerPrefs.SetInt(key, value);
        }
    }

    private getKey(user: string, key: string): string {
        if (isNullOrEmpty(user)) {
            user = this.systemUser;
        }
        return `${user}/${key}`;
    }
}

const logger = getLogger(PrefsService.name);
