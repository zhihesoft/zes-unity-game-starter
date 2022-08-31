import { UnityEngine } from "csharp";
import { injectable } from "tsyringe";
import { AfterViewInit, getLogger, OnInit, bind } from "zes-unity-jslib";

@injectable()
export class AppComponent implements OnInit, AfterViewInit {
    zesAfterViewInit(): void {
        logger.info(`camera: ${this.camera.gameObject.name}`);
    }
    zesOnInit(): void {
        logger.info("app component init");
    }

    @bind("Main Camera")
    camera!: UnityEngine.Camera;
}

const logger = getLogger(AppComponent.name);
