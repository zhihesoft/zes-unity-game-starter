/* eslint-disable @typescript-eslint/no-explicit-any */
import { UnityEngine } from "csharp";
import { BehaviorSubject, Subject } from "rxjs";
import { inject, injectable } from "tsyringe";
import { constructor } from "tsyringe/dist/typings/types";
import { bind, component } from "./view-decorators";
import { isOnSelected, OnInit } from "./view-interfaces";
import { ViewRef, VIEW_DATA } from "./view-ref";

@component()
@injectable()
export class ListView<D> implements OnInit {
    constructor(
        private view: ViewRef,
        @inject(VIEW_DATA) private itemClass: constructor<any>
    ) {
    }

    @bind("template") template!: UnityEngine.GameObject;
    onItemSelected = new Subject<D>(); //  item click event

    private data = new BehaviorSubject<D[]>([]);
    private pool: UnityEngine.GameObject[] = [];

    ngOnInit(): void {
        if (!this.itemClass) {
            throw new Error(`no item class found, you should pass it as view data`);
        }
        // fist child is template
        this.template.SetActive(false);
        this.data.subscribe(this.onItemsChanged.bind(this));
        // logger.debug(`listview data subscribed`);
    }

    setData(items: D[]) {
        // logger.debug(`list view set data: ${JSON.stringify(items)}`);
        this.data.next(items);
    }

    private onItemsChanged(items: D[]) {
        this.view.destroyChildren(false);

        const newCount = items.length - this.pool.length;
        for (let i = 0; i < newCount; i++) {
            const go = UnityEngine.GameObject.Instantiate(this.template, this.template.transform.parent);
            // logger.debug(`create go at ${i}`);
            this.pool.push(<UnityEngine.GameObject>go);
        }

        for (let i = 0; i < items.length; i++) {
            const item = items[i];
            // logger.debug(`attach go at ${i}`);
            this.pool[i].SetActive(true);
            this.view.attachChild(this.itemClass, this.pool[i], item).then(v => {
                if (isOnSelected(v.component)) {
                    v.component.ngOnSelected.subscribe(this.onItemSelected);
                }
            });
        }

        for (let i = items.length; i < this.pool.length; i++) {
            this.pool[i].SetActive(false);
        }
    }
}

// const logger = getLogger(ListView.name);
