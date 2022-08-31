import { UnityEngine } from "csharp";
import "reflect-metadata";
import { Subject } from "rxjs";
import { injectable } from "tsyringe";
import { AfterViewInit, LayerService, App, bind, component, getLogger, OnInit, text, ResourceService } from "zes-unity-jslib";

@component()
@injectable()
export class AppComponent implements OnInit, AfterViewInit {

    constructor(
        // public test: TestService,
        public loader: ResourceService,
        public layers: LayerService
    ) {
    }

    zesAfterViewInit(): void {
        logger.info(`camera: ${this.camera.gameObject.name}`);
        this.text.next("hello world");
    }
    zesOnInit(): void {
        logger.info("app component init");
        logger.debug(`layer service is null? ${this.layers == null}`);
    }

    @bind("#camera")
    camera!: UnityEngine.Camera;

    @text("#text")
    text: Subject<string> = new Subject();
}

const logger = getLogger(AppComponent.name);
