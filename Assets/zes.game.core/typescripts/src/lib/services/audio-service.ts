import { singleton } from "tsyringe";
import { getLogger } from "../logger";

@singleton()
export class AudioService {

    playBGM(path: string) {
        logger.info(`play bgm ${path}`);
    }

    playSE(path: string) {
        logger.info(`play se ${path}`);
    }
}

const logger = getLogger(AudioService.name);