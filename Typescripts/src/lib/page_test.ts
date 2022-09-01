import { click, component, page, PageService, Transit } from "zes-unity-jslib";

@component({ template: "Assets/Bundles/ui/test1.prefab" })
@page({ transit: Transit.Fade })
export class PageTest {
    constructor(
        public pages: PageService,
    ) { }

    @click("#button")
    onClickReturn() {
        this.pages.goBack();
    }
}