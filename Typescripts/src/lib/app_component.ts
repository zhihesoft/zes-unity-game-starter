import { UnityEngine } from "csharp";
import "reflect-metadata";
import { AfterViewInit, assert, Bind, Component, FadeService, getLogger, LayerService, OnInit, PageService, ResourceService } from "zes-unity-jslib";
import { PageTitle } from "./page_title";

@Component()
export class AppComponent implements OnInit, AfterViewInit {

    constructor(
        // public test: TestService,
        public loader: ResourceService,
        public layers: LayerService,
        public fade: FadeService,
        public pages: PageService,
    ) {
    }

    async zesAfterViewInit() {
        assert(this.layerMain, "layer-main is null");
        await this.loader.loadBundles(["ui", "conf"], () => {
            // console.log(`p: ${p}`);
        });
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
