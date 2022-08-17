import { singleton } from "tsyringe";
import { getLogger } from "../../logger";
import { Platform } from "../platform";
import { AccountGuest } from "./account-guest";

export const TOKEN_PLATFORM_DEV = "PLATFORM-COMMON";

@singleton()
export class PlatformCommon extends Platform {

    override get name(): string { return "common"; }

    override async onInit(): Promise<void> {
        logger.info("Platform dev inited");
        this._defaultAccount = new AccountGuest();
        this.addAccount(this._defaultAccount);
        return;
    }
}

const logger = getLogger(PlatformCommon.name);
