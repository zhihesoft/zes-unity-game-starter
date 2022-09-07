import { UnityEngine } from "csharp";
import { BehaviorSubject } from "rxjs";
import { Click, Component, EaseType, ListView, Page, PageService, Prop, Transit, tween } from "zes-unity-jslib";
import { ListViewTest } from "./list-view";
import { ListViewItem } from "./list-view-item";

@Component({ template: "Assets/Bundles/ui/page_controls.prefab" })
@Page({ transit: Transit.Fade })
export class PageControls {
    constructor(
        public pages: PageService,
    ) { }

    private listitems: string[] = [];

    @Prop("#slider", UnityEngine.UI.Slider)
    slider: BehaviorSubject<number> = new BehaviorSubject(0);

    @ListView("#list-view", { itemClass: ListViewItem }) list!: ListViewTest;

    @Click("#btn-reset-slider", 10)
    async onClickReset() {
        this.slider.next(0);
        await tween(0).to(1, 5).setEase(EaseType.Smooth).onUpdate(value => {
            this.slider.next(value);
        }).run();
    }

    @Click("#btn-return")
    onClickReturn() {
        this.pages.goBack();
    }

    @Click("#list-add")
    onClickListAdd() {
        this.listitems.push(`items: ${this.listitems.length}`);
        this.list.setData(this.listitems);
    }
    @Click("#list-reset")
    onClickListReset() {
        this.listitems = [];
        this.list.setData(this.listitems);
    }
}