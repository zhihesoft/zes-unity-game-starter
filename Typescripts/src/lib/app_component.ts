import { UnityEngine } from "csharp";
import "reflect-metadata";
import { AfterViewInit, assert, bind, component, getLogger, LayerService, OnInit, PageService, ResourceService } from "zes-unity-jslib";
import { FadeService } from "zes-unity-jslib/dist/lib/services/fade_service";
import { PagePatch } from "./page_patch";

@component()
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
        await this.loader.loadBundles(["ui", "conf"], (p) => {
            console.log(`p: ${p}`);
        });
        this.layers.add("main", this.layerMain);
        this.pages.navigate(PagePatch);
    }
    
    zesOnInit(): void {
        logger.info("app component init");
        logger.debug(`layer service is null? ${this.layers == null}`);
        this.fade.setFadeImage(this.fadeImage);
    }

    @bind("#camera")
    camera!: UnityEngine.Camera;

    @bind("#layer-main")
    layerMain!: UnityEngine.GameObject;

    @bind("#fade-image")
    fadeImage!: UnityEngine.UI.Image;
}

const logger = getLogger(AppComponent.name);
