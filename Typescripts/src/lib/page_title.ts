import { Click, Component, Page, PageService, Transit } from "zes-unity-jslib";
import { PageControls } from "./page_controls";
import { PagePatch } from "./page_patch";
import { PageTest } from "./page_test";

@Component({ template: "Assets/Bundles/ui/page_title.prefab" })
@Page({ transit: Transit.Fade })
export class PageTitle {

    constructor(
        public pages: PageService,
    ) { }

    @Click("#btn-patch")
    onClickPatch() {
        this.pages.navigate(PagePatch);
    }

    @Click("#btn-controls")
    onClickControls() {
        this.pages.navigate(PageControls);
    }

    @Click("#btn-pages")
    onClickPages() {
        this.pages.navigate(PageTest);
    }
}