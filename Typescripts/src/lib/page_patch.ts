import { click, component, page, PageService, ResourceService } from "zes-unity-jslib";
import { Transit } from "zes-unity-jslib/dist/lib/metadata_page";
import { PageTitle } from "./page_title";

@component({ template: "Assets/Bundles/ui/patch.prefab" })
@page({ transit: Transit.Fade })
export class PagePatch {

    constructor(
        public pages: PageService,
        public loader: ResourceService
    ) { }


    @click("#submit")
    async onSubmit() {
        this.pages.replace(PageTitle);
    }
}