import { BehaviorSubject, Subject } from "rxjs";
import { inject, injectable } from "tsyringe";
import { click, component, text } from "../../core/view-decorators";
import { OnInit, OnSelected } from "../../core/view-interfaces";
import { VIEW_DATA } from "../../core/view-ref";
import { LanguageService } from "../../services/language-service";
import { format } from "../../util";

@component()
@injectable()
export class AreaItem implements OnInit, OnSelected {

    constructor(
        private lang: LanguageService,
        @inject(VIEW_DATA) private data: number
    ) { }

    ngOnSelected = new Subject<number>();

    @text("label")
    label = new BehaviorSubject("");

    @click("")
    onClick() {
        this.ngOnSelected?.next(this.data);
    }

    ngOnInit(): void {
        // 50646 {0}区
        const fmt = this.lang.get(500646);
        const label = format(fmt, this.data * 10 + 1);
        const label2 = format(fmt, this.data * 10 + 10);
        this.label.next(`${label} - ${label2}`);
    }

}