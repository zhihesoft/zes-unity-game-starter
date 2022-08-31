import { UnityEngine } from "csharp";
import { Subject } from "rxjs";
import { injectable } from "tsyringe";
import { AfterViewInit, getLogger, OnInit, bind, text } from "zes-unity-jslib";

@injectable()
export class AppComponent implements OnInit, AfterViewInit {
    zesAfterViewInit(): void {
        logger.info(`camera: ${this.camera.gameObject.name}`);
        this.text.next("hello world");
    }
    zesOnInit(): void {
        logger.info("app component init");
    }

    @bind("#camera")
    camera!: UnityEngine.Camera;

    @text("#text")
    text: Subject<string> = new Subject();
}

const logger = getLogger(AppComponent.name);
