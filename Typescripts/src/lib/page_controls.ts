import { UnityEngine } from "csharp";
import { $typeof } from "puerts";
import { BehaviorSubject } from "rxjs";
import { bind, click, component, EaseType, page, PageService, Transit, tween } from "zes-unity-jslib";

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
        await tween(0).to(1, 5).setEase(EaseType.Smooth).onUpdate(value => {
            this.slider.next(value);
        }).run();
    }

    @click("#btn-return")
    onClickReturn() {
        this.pages.goBack();
    }
}