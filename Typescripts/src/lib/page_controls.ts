import { click, component, page, PageService, Transit } from "zes-unity-jslib";

@component({ template: "Assets/Bundles/ui/controls.prefab" })
@page({ transit: Transit.Fade })
export class PageControls {
    constructor(
        public pages: PageService,
    ) { }

    @click("#btn-return")
    onClickReturn() {
        this.pages.goBack();
    }
}