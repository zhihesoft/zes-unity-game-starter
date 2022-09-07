import { Click, Component, Page, PageService, Transit } from "zes-unity-jslib";
import { PageControls } from "./page_controls";
import { PageTest } from "./page_test";

@Component({ template: "Assets/Bundles/ui/title.prefab" })
@Page({ transit: Transit.Fade })
export class PageTitle {

    constructor(
        public pages: PageService,
    ) { }

    @Click("#btn-controls")
    onClickControls() {
        this.pages.navigate(PageControls);
    }

    @Click("#btn-pages")
    onClickPages() {
        this.pages.navigate(PageTest);
    }
}