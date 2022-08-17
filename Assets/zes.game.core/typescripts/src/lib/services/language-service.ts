import { UnityEngine } from "csharp";
import { $typeof } from "puerts";
import { container, singleton } from "tsyringe";
import { ApplicationRef } from "../core/application-ref";
import { getLogger } from "../logger";
import { ResourceService } from "./resource-service";

const languagePrefix = "language-"

const languageDefine = [
    "zh-cn",
    "en-us",
    "en-ww",
    "ja-jp",
    "ko-kr",
    "zh-hk",
    "ru",
];

export enum Language {
    zh_cn,  // 0
    en_us,  // 1
    en_ww,  // 2
    ja_jp,  // 3
    ko_kr,  // 4
    zh_hk,  // 5
    ru,     // 6
}

@singleton()
export class LanguageService {

    // public readonly i18n_zh_cn = "zh-cn";
    // public readonly i18n_en_us = "en-us";
    // public readonly i18n_en_ww = "en-ww";
    // public readonly i18n_ja_jp = "ja-jp";
    // public readonly i18n_ko_kr = "ko-kr";
    // public readonly i18n_zh_hk = "zh-hk";
    // public readonly i18n_ru = "ru";

    constructor(
        private res: ResourceService
    ) { }

    private currentLang: Language = Language.zh_cn;
    private items: LanguageItems = {};

    async init() {
        const app = container.resolve(ApplicationRef);
        this.currentLang = app.host.config.language;
        await this.res.loadBundle("data");
        const textPath = `Assets/Bundles/data/${languagePrefix}${languageDefine[this.currentLang]}.json`;
        logger.info(`load language from ${textPath}`);
        this.res.loadAsset<UnityEngine.TextAsset>(textPath, $typeof(UnityEngine.TextAsset))
            .then(txt => this.items = JSON.parse(txt.text))
            .catch(logger.error.bind(logger));
    }

    get(id: number): string;
    get(id: string): string;
    get(id: string | number): string {
        let lid = "";
        if (typeof id == "number") {
            lid = `i18n_${id}`;
        } else {
            if (!id.startsWith("i18n_")) {
                lid = `i18n_${id}`;
            } else {
                lid = id;
            }
        }

        const value = this.items[lid];
        if (!value) {
            logger.error(`cannot find i18n string of (${id}), lang is ${this.currentLang}`);
            return lid;
        }
        return value;

    }
}

export interface LanguageItems {
    [id: string]: string;
}

const logger = getLogger(LanguageService.name);
