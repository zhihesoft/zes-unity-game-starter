import { BehaviorSubject } from "rxjs";
import { inject } from "tsyringe";
import { AfterViewInit, component, text } from "zes-unity-jslib";
import { VIEW_DATA } from "zes-unity-jslib/dist/lib/view_ref";

@component()
export class ListViewItem implements AfterViewInit {
    constructor(
        @inject(VIEW_DATA) private data: string,
    ) { }
    
    zesAfterViewInit(): void {
        this.label.next(this.data);
    }

    @text("label") label = new BehaviorSubject("");
}