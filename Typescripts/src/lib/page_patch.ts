import { UnityEngine } from "csharp";
import { BehaviorSubject } from "rxjs";
import { Click, Component, getPatcher, Page, PageService, PatchProvider, PatchStatus, Prop, ResourceService, Text, Transit } from "zes-unity-jslib";

@Component({ template: "Assets/Bundles/ui/page_patch.prefab" })
@Page({ transit: Transit.Fade })
export class PagePatch {

    constructor(
        public pages: PageService,
        public loader: ResourceService,
    ) {
        this.patcher = getPatcher();
    }

    private readonly patcher: PatchProvider;

    @Text("#messages")
    messages = new BehaviorSubject("");

    @Prop("#progress", UnityEngine.UI.Slider)
    progress = new BehaviorSubject(0);

    @Click("#back")
    async onBack() {
        this.pages.goBack();
    }

    @Click("#btn-check", 2)
    async onCheck() {
        const ret = await this.patcher.check();
        if (ret == PatchStatus.Extract) {
            this.messages.next(`Extracting`);
            await this.patcher.extract();
            this.messages.next(`Extract DONE`);
        } else if (ret == PatchStatus.Found) {
            await this.patcher.patch(p => {
                const percent = Math.floor(p * 100);
                this.messages.next(`Patching ${percent}%`);
                this.progress.next(p);
            });
            this.messages.next(`Patch DONE`);
        } else if (ret == PatchStatus.Reinstall) {
            this.messages.next(`Need reinstall`);
        } else {
            this.messages.next(`no patch`);
        }
    }
}