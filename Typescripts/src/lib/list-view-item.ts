import { BehaviorSubject } from "rxjs";
import { inject } from "tsyringe";
import { AfterViewInit, Component, Text, VIEW_DATA } from "zes-unity-jslib";

@Component()
export class ListViewItem implements AfterViewInit {
    constructor(
        @inject(VIEW_DATA) private data: string,
    ) { }

    zesAfterViewInit(): void {
        this.label.next(this.data);
    }

    @Text("label") label = new BehaviorSubject("");
}