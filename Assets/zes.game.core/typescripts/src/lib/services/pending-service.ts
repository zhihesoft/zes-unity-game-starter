import { UnityEngine } from "csharp";
import { container, injectable, singleton } from "tsyringe";
import { ApplicationRef } from "../core/application-ref";
import { bind, component } from "../core/view-decorators";
import { ViewRef } from "../core/view-ref";
import { LAYER_TOPMOST } from "./layer-service";

@singleton()
export class PendingService {
    private counter = 0;
    private panel?: PendingPanel;

    run<T>(promise: Promise<T>): Promise<T> {

        return this.enter().then(() => { return promise; })
            .then(r => {
                this.leave();
                return r;
            }).catch(err => {
                this.leave();
                return Promise.reject(err);
            });
    }

    private async enter() {
        this.counter++;
        if (!this.panel) {
            const app = container.resolve(ApplicationRef);
            const view = new ViewRef(PendingPanel, app.rootView);
            await view.show();
            this.panel = view.component;
        }
        this.panel?.setActive(true);
    }

    private leave() {
        this.counter = Math.max(0, this.counter - 1);
        if (this.counter <= 0) {
            this.panel?.setActive(false);
        }
    }
}

@injectable()
@component({ template: "Assets/Bundles/ui-prefabs/controls/pending-panel.prefab", node: LAYER_TOPMOST })
class PendingPanel {
    constructor(
        private view: ViewRef
    ) { }


    @bind("background") background!: UnityEngine.GameObject;

    private flag = false;

    setActive(flag: boolean) {

        if (flag) {
            if (this.flag) {
                return; // alread show
            }
            this.view.setActive(true);
            this.flag = flag;
            this.background.SetActive(false);
            setTimeout(() => {
                if (this.flag) {
                    this.background.SetActive(true);
                }
            }, 500);
        } else {
            this.flag = flag;
            this.view.setActive(false);
            this.background.SetActive(false);
        }
    }
}


