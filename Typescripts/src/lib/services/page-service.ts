/* eslint-disable @typescript-eslint/no-explicit-any */
import { UnityEngine } from "csharp";
import { container, singleton } from "tsyringe";
import { constructor } from "tsyringe/dist/typings/types";
import { ApplicationRef } from "../core/application-ref";
import { ViewRef } from "../core/view-ref";
import { getLogger } from "../logger";
import { FadeService } from "./fade-service";
import { LAYER_MAIN } from "./layer-service";

@singleton()
export class PageService {
    constructor(
        private fade: FadeService,
        private app: ApplicationRef
    ) { }

    private views: ViewRef[] = [];
    private currentView?: ViewRef;

    clear() {
        for (const item of this.views) {
            item.destroy();
        }
        this.views = [];
        this.currentView = undefined;
    }

    async replace<T = any>(ctor: constructor<T>): Promise<void>;
    async replace<T = any, D = any>(ctor: constructor<T>, data: D): Promise<void>;
    async replace<T = any, D = any>(ctor: constructor<T>, data?: D): Promise<void> {
        logger.info(`replace views with ${ctor.name} `);
        const go = this.getDefaultLayer();
        await this.fade.out();
        this.clear();

        const view = new ViewRef(ctor, this.app.rootView);
        await view.show({ node: go, data });
        this.views.push(view);
        this.currentView = view;
        await this.fade.in();
    }

    async show<T = any>(ctor: constructor<T>): Promise<void>;
    async show<T = any, D = any>(ctor: constructor<T>, data: D): Promise<void>;
    async show<T = any, D = any>(ctor: constructor<T>, data?: D): Promise<void> {
        logger.info(`showing view of ${ctor.name} `);
        const go = this.getDefaultLayer();
        let view: ViewRef | undefined;
        const idx = this.views.findIndex(i => i.componentClass == ctor);
        if (idx >= 0) {
            view = this.views[idx];
            this.views.splice(idx, 1);
            if (view?.componentMeta?.transition) {
                await this.fade.out();
            }
            view?.setActive(true);
            this.views.push(view);
            logger.info(`view is in the stack, just make it active`);
        } else {
            view = new ViewRef(ctor, this.app.rootView);
            if (view?.componentMeta?.transition) {
                await this.fade.out();
            }
            await view.show({ node: go, data })
            this.views.push(view);
            logger.info(`view is created and put to view stack`);
        }

        if (this.currentView != view) {
            this.currentView?.setActive(false);
            this.currentView = view;
        }

        if (view?.componentMeta?.transition) {
            await this.fade.in();
        }
    }

    private getDefaultLayer(): UnityEngine.GameObject {
        const layer = container.resolve<UnityEngine.GameObject>(LAYER_MAIN);
        return layer;
    }
}

const logger = getLogger(PageService.name);
