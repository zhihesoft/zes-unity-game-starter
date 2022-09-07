import { Click, Component, Page, PageService, ResourceService, Transit } from "zes-unity-jslib";
import { PageTitle } from "./page_title";

@Component({ template: "Assets/Bundles/ui/patch.prefab" })
@Page({ transit: Transit.Fade })
export class PagePatch {

    constructor(
        public pages: PageService,
        public loader: ResourceService
    ) { }


    @Click("#submit")
    async onSubmit() {
        this.pages.replace(PageTitle);
    }
}