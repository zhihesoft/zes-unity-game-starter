import { Click, Component, PageService, Transit } from "zes-unity-jslib";

@Component({ template: "Assets/Bundles/ui/test1.prefab", transit: Transit.Fade })
export class PageTest {
    constructor(
        public pages: PageService,
    ) { }

    @Click("#button")
    onClickReturn() {
        this.pages.goBack();
    }
}