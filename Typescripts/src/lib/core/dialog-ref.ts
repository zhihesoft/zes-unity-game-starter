/* eslint-disable @typescript-eslint/no-explicit-any */
import { UnityEngine } from "csharp";
import "reflect-metadata";
import { Observable, Subject } from "rxjs";
import { inject, injectable } from "tsyringe";
import { constructor } from "tsyringe/dist/typings/types";
import { LAYER_DIALOG } from "../services/layer-service";
import { ViewRef } from "./view-ref";
import GameObject = UnityEngine.GameObject;

@injectable()
export class Dialog {

    constructor(
        private view: ViewRef,
        @inject(LAYER_DIALOG) private layer: UnityEngine.GameObject,
    ) { }

    open<T>(ctor: constructor<T>): Promise<DialogRef>;
    open<T, D = any>(ctor: constructor<T>, data: D): Promise<DialogRef>;
    open<T, D = any>(ctor: constructor<T>, data: D, parentGo: GameObject): Promise<DialogRef>;
    async open<T, D = any>(ctor: constructor<T>, data?: D, parentGO?: GameObject): Promise<DialogRef> {
        const dlgView = new ViewRef(ctor, this.view);
        const dlgRef = new DialogRef(dlgView);
        parentGO = parentGO || this.layer;
        dlgView.container.register(DialogRef, { useValue: dlgRef });
        await dlgView.show({ node: parentGO, data });
        return dlgRef;
    }
}

export class DialogRef<R = any> {
    constructor(
        private view: ViewRef
    ) { }

    private closeNotify: Subject<R | undefined> = new Subject();

    afterClosed(): Observable<R | undefined> {
        return this.closeNotify;
    }

    close(): void;
    close(result: R): void;
    close(result?: R): void {
        this.view.destroy();
        this.closeNotify.next(result);
    }

}
