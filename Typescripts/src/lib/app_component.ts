import { UnityEngine, Zes } from "csharp";
import "reflect-metadata";
import {
    AfterViewInit,
    assert,
    Bind,
    Component,
    FadeService,
    getLogger,
    I18nService,
    i18n_en_us, LayerService,
    OnInit,
    PageService,
    ResourceService
} from "zes-unity-jslib";
import { PageTitle } from "./page_title";

@Component()
export class AppComponent implements OnInit, AfterViewInit {

    constructor(
        // public test: TestService,
        public loader: ResourceService,
        public layers: LayerService,
        public fade: FadeService,
        public pages: PageService,
        public i18n: I18nService,
    ) {
    }

    async zesAfterViewInit() {
        assert(this.layerMain, "layer-main is null");
        await this.loader.loadBundles(["ui", "conf", "language"], () => {
            // console.log(`p: ${p}`);
        });
        // await this.i18n.load(Zes.App.config.appLanguage, Zes.App.config.languageBundlePath + "i18n-zh-cn.json");
        await this.i18n.load(Zes.App.config.appLanguage, `${Zes.App.config.languageBundlePath}/i18n-${Zes.App.config.appLanguage}.json`);
        await this.i18n.load(i18n_en_us, "Assets/Bundles/language/i18n-en-us.json");
        this.i18n.currentLanguage = Zes.App.config.appLanguage;
        this.layers.add("main", this.layerMain);
        this.pages.navigate(PageTitle);
    }

    zesOnInit(): void {
        logger.info("app component init");
        logger.debug(`layer service is null? ${this.layers == null}`);
        this.fade.setFadeImage(this.fadeImage);
    }

    @Bind("#camera")
    camera!: UnityEngine.Camera;

    @Bind("#layer-main")
    layerMain!: UnityEngine.GameObject;

    @Bind("#fade-image")
    fadeImage!: UnityEngine.UI.Image;
}

const logger = getLogger(AppComponent.name);
