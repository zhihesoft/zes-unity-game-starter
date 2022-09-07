import { Click, Component } from "zes-unity-jslib";
import { DialogRef } from "zes-unity-jslib/dist/lib/dialog_ref";

@Component({template: "Assets/Bundles/ui/dialog_test1.prefab"})
export class DialogTest1 {
    constructor(
        private dlg: DialogRef
    ) { }

    @Click("#close")
    onClickClose() {
        this.dlg.close("hello world");
    }
}