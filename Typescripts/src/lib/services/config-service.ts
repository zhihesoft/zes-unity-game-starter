import { UnityEngine } from "csharp";
import { $typeof } from "puerts";
import { container, InjectionToken, singleton } from "tsyringe";
import { getLogger } from "../logger";
import { Configuration } from "../models/configs/config-base";
import { VipConfigConfig } from "../models/configs/vip-config";
import { VipGoodsConfig } from "../models/configs/vip-goods";
import { ResourceService } from "./resource-service";

const tokens: InjectionToken[] = [
    VipConfigConfig,
    VipGoodsConfig,
];

@singleton()
export class ConfigService {

    constructor(
        private res: ResourceService
    ) { }

    private map: Map<InjectionToken, unknown> = new Map();

    get<T>(token: InjectionToken<T>): T | undefined {
        return <T>this.map.get(token);
    }

    async init() {
        await this.res.loadBundle("data");
        for (const token of tokens) {
            const config = <Configuration>container.resolve(token);
            const settings = config.getSettings();
            try {
                const json = await this.res.loadAsset<UnityEngine.TextAsset>(`Assets/Bundles/data/${settings.sheet}.json`, $typeof(UnityEngine.TextAsset));
                config.load(json.text);
                logger.info(`configuration ${settings.sheet} loaded`);
            } catch (err) {
                logger.error(`${err}`);
            }
        }
    }
}

const logger = getLogger(ConfigService.name);
