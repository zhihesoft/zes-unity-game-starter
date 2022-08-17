import { TMPro } from "csharp";
import { BehaviorSubject } from "rxjs";
import { injectable } from "tsyringe";
import { bind, component } from "../../core/view-decorators";
import { AfterViewInit, OnDestroy } from "../../core/view-interfaces";
import { LanguageService } from "../../services/language-service";
import { LAYER_DIALOG } from "../../services/layer-service";
import { randomInt, waitForSeconds } from "../../util";

@component({ template: "Assets/Bundles/ui-prefabs/controls/loading-panel.prefab", node: LAYER_DIALOG })
@injectable()
export class LoadingPanel implements AfterViewInit, OnDestroy {

    constructor(
        private lang: LanguageService
    ) { }

    private disposed = false;

    ngOnDestroy(): void {
        this.disposed = true;
    }

    @bind("hint", { type: TMPro.TMP_Text, prop: "text" })
    info = new BehaviorSubject("");

    ngAfterViewInit(): void {
        this.refreshHintInfo();
    }

    async refreshHintInfo() {
        const str = this.lang.get(randomInt(300001, 300020));
        this.info.next(str);
        await waitForSeconds(2);
        if (!this.disposed) {
            this.refreshHintInfo();
        }
    }
}