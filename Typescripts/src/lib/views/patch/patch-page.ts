import { Liv, TMPro, UnityEngine } from "csharp";
import { BehaviorSubject } from "rxjs";
import { inject, injectable, registry } from "tsyringe";
import { ApplicationRef } from "../../core/application-ref";
import { bind, component } from "../../core/view-decorators";
import { AfterViewInit } from "../../core/view-interfaces";
import { Transition } from "../../core/view-transition";
import { getLogger } from "../../logger";
import { Platform } from "../../platforms/platform";
import { FadeService } from "../../services/fade-service";
import { LanguageService } from "../../services/language-service";
import { PageService } from "../../services/page-service";
import { ResourceService } from "../../services/resource-service";
import { waitForSeconds } from "../../util";
import { GAME_CONFIG } from "../global-values";
import { TitlePage } from "../title/title-page";
import { PatchProvider, PatchStatus } from "./patch-provider";

@registry([
    { token: PatchProvider, useClass: PatchProvider }
])
@injectable()
@component({ template: "Assets/Bundles/patch/patch-page.prefab", transition: Transition.Fade })
export class PatchPage implements AfterViewInit {
    constructor(
        private app: ApplicationRef,
        private pages: PageService,
        private fade: FadeService,
        private provider: PatchProvider,
        private res: ResourceService,
        private language: LanguageService,
        @inject(GAME_CONFIG) private gameConfig: Liv.Config,
        private platform: Platform
    ) { }

    @bind("progress", { type: UnityEngine.UI.Slider, prop: "value" })
    progress = new BehaviorSubject(0);

    @bind("info", { type: TMPro.TMP_Text, prop: "text" })
    info = new BehaviorSubject("");

    async ngAfterViewInit(): Promise<void> {
        this.check();
    }

    async check() {
        this.info.next(this.language.get("300205"));
        const status = await this.provider.check();
        if (status == PatchStatus.None) {
            this.prepare();
            return;
        } else if (status == PatchStatus.Extract) {
            await this.provider.extract();
            this.check();
            return;
        } else if (status == PatchStatus.Reinstall) {
            logger.error("need reinstall");
            return;
        } else if (status == PatchStatus.Found) {
            this.patch();
        } else {
            logger.error(`unknown patch status: ${status}`);
        }
    }

    async patch() {
        await this.provider.patch((p) => {
            this.progress.next(p);
        });
        await this.fade.out();

        // restart js env
        this.pages.clear();
        this.app.host.loader.unloadBundle("data");
        this.app.host.loader.unloadBundle("patch");
        logger.info(`restarting javascript env`);
        this.app.host.RestartJavaScriptEnviroment();
    }

    async prepare() {
        await waitForSeconds(1);
        this.progress.next(0);
        await this.res.loadBundles(this.progress.next.bind(this.progress));

        // init platform
        await this.platform.init();
        this.pages.replace(TitlePage);
    }
}

const logger = getLogger(PatchPage.name);
