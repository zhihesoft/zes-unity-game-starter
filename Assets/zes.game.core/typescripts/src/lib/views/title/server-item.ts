import { BehaviorSubject, Subject } from "rxjs";
import { inject, injectable } from "tsyringe";
import { click, component, text } from "../../core/view-decorators";
import { OnInit, OnSelected } from "../../core/view-interfaces";
import { ViewRef, VIEW_DATA } from "../../core/view-ref";
import { ServerConfig } from "../../models/game/server-config";
import { LanguageService } from "../../services/language-service";
import { format } from "../../util";

@component()
@injectable()
export class ServerItem implements OnInit, OnSelected {

    constructor(
        public view: ViewRef,
        private lang: LanguageService,
        @inject(VIEW_DATA) private data: ServerConfig
    ) { }


    ngOnSelected = new Subject<ServerConfig>();

    @text("label")
    label = new BehaviorSubject("");

    @click("")
    onClick() {
        this.ngOnSelected?.next(this.data);
    }

    ngOnInit(): void {
        this.setData(this.data);
    }

    setData(server: ServerConfig) {
        this.data = server;
        // 50646 {0}区
        if (this.data) {
            const fmt = this.lang.get(500646);
            const label = format(fmt, this.data.id);
            this.label.next(`${label} ${this.data.name}`);
        }
    }

    getData() {
        return this.data;
    }
}