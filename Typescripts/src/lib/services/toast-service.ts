import { TMPro, UnityEngine } from "csharp";
import { BehaviorSubject } from "rxjs";
import { inject, injectable, singleton } from "tsyringe";
import { ApplicationRef } from "../core/application-ref";
import { bind, component } from "../core/view-decorators";
import { AfterViewInit } from "../core/view-interfaces";
import { ViewRef, VIEW_DATA } from "../core/view-ref";
import { waitForSeconds } from "../util";
import { LanguageService } from "./language-service";
import { LAYER_TOPMOST } from "./layer-service";

@singleton()
export class ToastService {

    constructor(
        private app: ApplicationRef,
        private language: LanguageService,
        @inject(LAYER_TOPMOST) private layer: UnityEngine.GameObject,
    ) { }

    show(id: number): void;
    show(raw: string): void
    show(message: string | number) {
        if (typeof message == "number") {
            message = this.language.get(`${message}`);
        }
        const v = new ViewRef(ToastItem, this.app.rootView);
        v.show({ data: message });
    }
}

@component({ template: "Assets/Bundles/ui-prefabs/controls/toast.prefab", node: LAYER_TOPMOST })
@injectable()
class ToastItem implements AfterViewInit {
    constructor(
        private view: ViewRef,
        @inject(VIEW_DATA) private message: string
    ) { }

    @bind("background/text", { type: TMPro.TMP_Text, prop: "text" })
    text = new BehaviorSubject("");

    async ngAfterViewInit() {
        this.text.next(this.message);
        await waitForSeconds(2);
        this.view.destroy();
    }


}