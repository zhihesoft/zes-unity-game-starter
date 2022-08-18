/* eslint-disable @typescript-eslint/no-explicit-any */
import { UnityEngine } from "csharp";
import { $typeof } from "puerts";
import "reflect-metadata";
import { Observable, Subject, throttleTime } from "rxjs";
import { container, DependencyContainer } from "tsyringe";
import { constructor } from "tsyringe/dist/typings/types";
import { getLogger } from "../logger";
import { ResourceService } from "../services/resource-service";
import { empty, isGameObject } from "../util";
import { BindData, BindEventOption, BindPropOption, BindViewOption, ComponentData, META_BIND, META_COMPONENT } from "./view-decorators";
import { isViewHost, ViewHost } from "./view-host";
import { isAfterViewInit, isOnActiveChanged, isOnDestroy, isOnInit } from "./view-interfaces";
import { ViewOption } from "./view-option";

import GameObject = UnityEngine.GameObject;
import Transform = UnityEngine.Transform;

export const VIEW_DATA = Symbol("VIEW_DATA");

export class ViewRef<T = any> {

    constructor(
        public componentClass: constructor<T>,
        public parent: ViewRef
    ) {
        this.componentMeta = Reflect.getMetadata(META_COMPONENT, componentClass);
        if (parent) {
            this.container = parent.container.createChildContainer();
        } else {
            this.container = container;
        }
    }

    public readonly componentMeta?: ComponentData;
    public readonly container: DependencyContainer;
    public host?: ViewHost;
    public gameObject?: UnityEngine.GameObject;
    public component?: T;
    private disposed = false;
    private children: ViewRef[] = [];

    public get isDisposed() { return this.disposed; }

    async showChild<T>(cls: constructor<T>): Promise<ViewRef<T>>;
    async showChild<T>(cls: constructor<T>, option: ViewOption): Promise<ViewRef<T>>;
    async showChild<T>(cls: constructor<T>, option?: ViewOption): Promise<ViewRef<T>> {
        const view = new ViewRef(cls, this);
        await view.show(option || {});
        return view;
    }

    async attachChild<T>(cls: constructor<T>, host: GameObject): Promise<ViewRef<T>>;
    async attachChild<T>(cls: constructor<T>, host: GameObject, data: any): Promise<ViewRef<T>>;
    async attachChild<T>(cls: constructor<T>, host: GameObject, data?: any): Promise<ViewRef<T>> {
        const v = new ViewRef(cls, this);
        await v.attach(host, data);
        return v;
    }

    async attach(host: GameObject | ViewHost): Promise<void>;
    async attach(host: GameObject | ViewHost, data: any): Promise<void>;
    async attach(host: GameObject | ViewHost, data?: any): Promise<void> {
        if (this.parent) {
            this.parent.children.push(this);
        }
        this.container.register(ViewRef, { useValue: this });
        this.container.register(VIEW_DATA, { useValue: data });
        if (isViewHost(host)) {
            this.host = host;
        } else {
            this.host = ViewHost.create(host);
        }
        this.component = this.container?.resolve(this.componentClass);
        if (!this.host.isSceneHost) {
            this.gameObject = this.host.find("");
        }
        await this.bind();
        if (isOnInit(this.component)) {
            await this.component.ngOnInit();
        }
        if (isAfterViewInit(this.component)) {
            this.component.ngAfterViewInit();
        }
        if (isOnActiveChanged(this.component)) {
            this.component.ngOnActiveChanged(true);
        }
    }

    async show(): Promise<void>;
    async show(option: ViewOption): Promise<void>
    async show(option?: ViewOption): Promise<void> {
        const template = option?.template || this.componentMeta?.template;
        if (!template) {
            throw new Error(`show viewref need a template`);
        }

        const isSceneView = template.endsWith(".unity");
        const node = option?.node || this.componentMeta?.node;
        if (!node && !isSceneView) {
            throw new Error(`gameobject view should have a host node...`);
        }

        const res = container.resolve(ResourceService);
        if (isSceneView) {
            const scene = await res.loadAdditiveScene(template, empty);
            this.host = ViewHost.create(scene);
        } else {
            let go: GameObject;
            if (isGameObject(node)) {
                go = node;
            } else if (typeof node === "string") {
                const o1 = this.parent.host?.find(node);
                if (!o1) {
                    throw new Error(`cannot find (${node}) in parent`);
                }
                go = o1;
            } else if (typeof node === "symbol") {
                go = container?.resolve(node);
            } else {
                throw new Error(`unknown node type (${node})`);
            }
            const prefab = await res.loadPrefab(template);
            const newgo = <GameObject>GameObject.Instantiate(prefab, go.transform);
            this.host = ViewHost.create(newgo);
        }
        await this.attach(this.host, option?.data);

    }

    destroyChildren(cleanup = true) {
        for (const child of this.children) {
            child.destroy(cleanup);
        }
        this.children = [];
    }

    destroy(cleanup = true) {
        if (!this.disposed) {
            for (const child of this.children) {
                child.destroy();
            }
            const idx = this.parent.children.findIndex(s => s == this);
            if (idx >= 0) {
                this.parent.children.splice(idx, 1);
                if (isOnDestroy(this.component)) {
                    this.component.ngOnDestroy();
                }
            }
            this.host?.destroy(cleanup);
            this.disposed = true;
        }
    }

    setActive(status: boolean) {
        this.host?.setActive(status);
        if (isOnActiveChanged(this.component)) {
            this.component.ngOnActiveChanged(status);
        }
    }

    private async bind() {
        if (!this.host) {
            throw new Error(`host in viewref cannot be null`);
        }
        const bindMap: Map<string, BindData> = Reflect.getMetadata(META_BIND, this.componentClass);

        if (!bindMap) {
            return;
        }

        const ps: Promise<unknown>[] = [];

        for (const pair of bindMap) {
            const { key, data } = { key: pair[0], data: pair[1] };
            const data_go = this.host.find(data.path);
            if (!data_go) {
                logger.error(`cannot find (${data.path}) on gameobject (${this.componentClass.name}.${String(key)})`);
                continue;
            }

            if (data.option && !this.isViewOption(data.option)) {
                const comp = data_go.GetComponent($typeof(<any>data.option.type));
                if (!comp) {
                    logger.error(`cannot find component on gameobject [${data.path}] (${this.componentClass.name}.${String(key)})`);
                    continue;
                }
                if (this.isPropOption(data.option)) {
                    this.bindProp(comp, key, data.option);
                } else if (this.isEventOption(data.option)) {
                    this.bindEvent(comp, key, data.option);
                } else {
                    throw new Error(`unknown bind option: ${JSON.stringify(data.option)}`);
                }
            } else {
                const type = Reflect.getMetadata('design:type', <any>this.component, key);
                if (data.option) {
//                    const promise = this.attachChild(data.option.view, data_go, data.option.extra).then(v => (<any>this.component)[key] = v.component);
                    const promise = this.attachChild(type, data_go, data.option.extra).then(v => (<any>this.component)[key] = v.component);
                    ps.push(promise);
                } else if (type == GameObject) {
                    (<any>this.component)[key] = data_go;
                } else if (type == Transform) {
                    (<any>this.component)[key] = data_go.transform;
                } else {
                    (<any>this.component)[key] = data_go.GetComponent($typeof(type));
                }
            }
        }
        if (ps.length > 0) {
            await Promise.all(ps);
        }
    }

    private isPropOption(opt: BindPropOption | BindEventOption | BindViewOption): opt is BindPropOption {
        return ((<BindPropOption>opt).prop != undefined);
    }

    private isEventOption(opt: BindPropOption | BindEventOption | BindViewOption): opt is BindEventOption {
        return ((<BindEventOption>opt).event != undefined);
    }

    private isViewOption(opt: BindPropOption | BindEventOption | BindViewOption): opt is BindViewOption {
        return ((<BindViewOption>opt).extra != undefined);
    }

    private bindProp(comp: UnityEngine.Component, key: string, opt: BindPropOption) {
        const propkey = opt.prop;
        if (!propkey) {
            logger.error(`component property key is empty. (${this.componentClass.name}.${key})`);
            return;
        }

        // 值绑定皆为subject
        const subject = (<any>this.component)[key];
        if (!subject.subscribe) {
            logger.error(`${this.componentClass.name}.${key} is not a subject object`);
        } else {
            subject.subscribe({
                next: (v: any) => (<any>comp)[propkey] = v
            });
        }
    }

    private bindEvent(comp: UnityEngine.Component, key: string, opt: BindEventOption) {
        const propkey = opt.event;
        if (!propkey) {
            logger.error(`component event key is empty. (${this.componentClass.name}.${key})`);
            return;
        }

        const handle = (<any>comp)[propkey];
        if (!handle) {
            logger.error(`event ${propkey} is not existed on (${opt.type}`);
            return;
        }

        const evt: UnityEngine.Events.UnityEvent = handle;
        const subject = (<any>this.component)[key];

        evt.AddListener(this.eventCallback(subject, opt));

        // if (!subject.subscribe) {
        //     // 不是subject的，就认为是个Callback Function
        //     evt.AddListener(subject.bind(this.component));
        // } else {
        //     evt.AddListener(subject.next.bind(subject));
        // }
    }

    private eventCallback(target: any, opt: BindEventOption): (args?: any) => void {
        const subject = new Subject();
        let observable: Observable<unknown> = subject;
        if (opt.throttleSeconds) {
            observable = subject.pipe(throttleTime(opt.throttleSeconds * 1000));
        }
        if (this.isSubject(target)) {
            observable.subscribe(v => target.next(v));
        } else {
            observable.subscribe(v => target.bind(this.component)(v));
        }
        return (args?: any) => subject.next(args);
    }

    private isSubject<T = any>(target: any): target is Subject<T> {
        if (target.subscribe) {
            return true;
        }
        return false;
    }
}

const logger = getLogger(ViewRef.name);