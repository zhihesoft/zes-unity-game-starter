import { click, component, page, PageService, Transit } from "zes-unity-jslib";
import { PageTest } from "./page_test";

@component({ template: "Assets/Bundles/ui/title.prefab" })
@page({ transit: Transit.Fade })
export class PageTitle {

    constructor(
        public pages: PageService,
    ) { }

    @click("#btn-pages")
    onClickPages() {
        this.pages.navigate(PageTest);
    }
}