import { injectable } from "tsyringe";
import { getLogger, OnInit } from "zes-unity-jslib";

@injectable()
export class AppComponent implements OnInit {
    zesOnInit(): void {
        logger.info("app component init");
    }
}

const logger = getLogger(AppComponent.name);
