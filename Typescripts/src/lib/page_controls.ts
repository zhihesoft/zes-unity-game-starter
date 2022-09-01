import { UnityEngine } from "csharp";
import { $typeof } from "puerts";
import { BehaviorSubject } from "rxjs";
import { bind, click, component, page, PageService, Transit, waitForSeconds } from "zes-unity-jslib";

@component({ template: "Assets/Bundles/ui/controls.prefab" })
@page({ transit: Transit.Fade })
export class PageControls {
    constructor(
        public pages: PageService,
    ) { }

    @bind("#slider", { type: $typeof(UnityEngine.UI.Slider), prop: "value" })
    slider: BehaviorSubject<number> = new BehaviorSubject(0);

    @click("#btn-reset-slider", 10)
    async onClickReset() {
        this.slider.next(0);
        this.slider.next(0);
        while (this.slider.value < 1) {
            this.slider.next(this.slider.value + 0.01);
            await waitForSeconds(1 / 60);
        }
    }

    @click("#btn-return")
    onClickReturn() {
        this.pages.goBack();
    }
}