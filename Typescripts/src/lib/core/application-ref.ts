import { Liv, UnityEngine } from "csharp";
import { singleton } from "tsyringe";
import { constructor } from "tsyringe/dist/typings/types";
import { getLogger } from "../logger";
import { ViewRef } from "./view-ref";

@singleton()
export class ApplicationRef {

    bootstrap<T>(token: constructor<T>) {
        logger.info(`application init ...`);

        const go = UnityEngine.GameObject.Find("root");
        if (!go) {
            const err = new Error(`cannot find an game object of name root`);
            logger.error(err.message);
            throw err;
        }

        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const emptyParent = <any>undefined;
        this.rootView = new ViewRef(token, emptyParent);
        this.rootView.attach(go);
    }

    rootView!: ViewRef;
    host = Liv.App;
    http = Liv.Http;
}

const logger = getLogger(ApplicationRef.name);