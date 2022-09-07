import { Click, Component, PageService, Transit, ViewRef } from "zes-unity-jslib";
import { DialogTest1 } from "./dialog_test1";
import { PageControls } from "./page_controls";
import { PagePatch } from "./page_patch";
import { PageTest } from "./page_test";

@Component({ template: "Assets/Bundles/ui/page_title.prefab", transit: Transit.Fade })
export class PageTitle {

    constructor(
        public pages: PageService,
        public view: ViewRef,
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

    @Click("#btn-dialog")
    async onClickDialog() {
        const dlg = await this.view.dialog(DialogTest1);
        dlg.afterClosed().subscribe(p => {
            console.log(p);
        });
    }
}