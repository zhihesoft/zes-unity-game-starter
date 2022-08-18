import { Liv, UnityEngine } from "csharp";
import { container, inject, Lifecycle, registry, singleton } from "tsyringe";
import { bind } from "../core/view-decorators";
import { AfterViewInit } from "../core/view-interfaces";
import { ViewRef } from "../core/view-ref";
import { getLogger } from "../logger";
import { PlatformCommon, TOKEN_PLATFORM_DEV } from "../platforms/common/platform-common";
import { Platform } from "../platforms/platform";
import { FadeService } from "../services/fade-service";
import { LanguageService } from "../services/language-service";
import { LAYER_DIALOG, LAYER_MAIN, LAYER_TOPMOST } from "../services/layer-service";
import { PageService } from "../services/page-service";
import { ResourceService } from "../services/resource-service";
import { BUILD_CONFIG } from "./global-values";
import { PatchPage } from "./patch/patch-page";

@registry([
    { token: TOKEN_PLATFORM_DEV, useClass: PlatformCommon },
])
@singleton()
export class MainPage implements AfterViewInit {

    constructor(
        private view: ViewRef,
        private fade: FadeService,
        private res: ResourceService,
        private pages: PageService,
        private lang: LanguageService,
        @inject(BUILD_CONFIG) private buildConfig: Liv.BuildConfig,
    ) { }

    @bind("layers/fade/background")
    fadeLayer!: UnityEngine.UI.Image;
    @bind("layers/topmost")
    topLayer!: UnityEngine.GameObject;
    @bind("layers/dialog")
    dialogLayer!: UnityEngine.GameObject;
    @bind("layers/main")
    mainLayer!: UnityEngine.GameObject;

    async ngAfterViewInit() {

        if (!this.view.host) {
            throw new Error(`no host found`);
        }

        UnityEngine.GameObject.DontDestroyOnLoad(this.view.host.find(""));
        this.fade.init(this.fadeLayer);

        container.register(LAYER_TOPMOST, { useValue: this.topLayer });
        container.register(LAYER_DIALOG, { useValue: this.dialogLayer });
        container.register(LAYER_MAIN, { useValue: this.mainLayer });

        this.fadeLayer.color = new UnityEngine.Color(1, 1, 1, 1);

        this.process();
    }

    async process() {

        await this.res.loadBundle("patch");
        await this.lang.init();

        const platform = `PLATFORM-${this.buildConfig.platform}`.toUpperCase();
        container.register(Platform, { useToken: platform }, { lifecycle: Lifecycle.Singleton }); // redirect to spec platform

        logger.info("patch bundle loaded");
        this.pages.show(PatchPage);
    }
}

const logger = getLogger(MainPage.name);
