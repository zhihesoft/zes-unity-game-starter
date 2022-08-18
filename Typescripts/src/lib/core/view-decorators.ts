/* eslint-disable @typescript-eslint/no-explicit-any */
import { System, TMPro, UnityEngine } from "csharp";
import "reflect-metadata";
import { Transition } from "./view-transition";


export const META_BIND = Symbol("meta-bind");
export const META_BIND_EVENT = Symbol("meta-bind-event");
export const META_COMPONENT = Symbol("meta-component");

export interface BindPropOption {
    type: System.Type,
    prop: string,
}

export interface BindEventOption {
    type: System.Type;
    event: "onClick" | "onValueChanged";
    throttleSeconds?: number;
}

export interface BindViewOption {
    // view: constructor<T>;
    extra: unknown;            // extra data, list view 里面是child的constructor
}

export interface BindData {
    path: string;
    option?: BindPropOption | BindEventOption | BindViewOption;
}

export interface ComponentData {
    template?: string;          // 模板路径
    node?: string | symbol;     // 目标节点，新建的View将会挂载在这个node下，可以是TOKEN或者路径，如果是路径，会在Parent下寻找
    transition?: Transition;    // 转换，目前只有一个Fade
}

export function component(conf?: ComponentData): ClassDecorator {
    return Reflect.metadata(META_COMPONENT, conf);
}


export function bind(path: string, option?: BindPropOption | BindEventOption | BindViewOption): PropertyDecorator {
    return (target, key) => {
        let data: Map<string, BindData> = Reflect.getMetadata(META_BIND, target.constructor);
        if (!data) {
            data = new Map();
            Reflect.defineMetadata(META_BIND, data, target.constructor);
        }
        data.set(String(key), { path, option });
    };
}

export function text(path: string) {
    return bind(path, { type: TMPro.TMP_Text, prop: "text" });
}

export function click(path: string): PropertyDecorator;
export function click(path: string, throttleSeconds: number): PropertyDecorator;
export function click(path: string, throttleSeconds?: number): PropertyDecorator {
    throttleSeconds = throttleSeconds ?? 0;
    return bind(path, { type: UnityEngine.UI.Button, event: "onClick", throttleSeconds });
}

export function view(path: string): PropertyDecorator;
export function view(path: string, extra: unknown): PropertyDecorator;
export function view(path: string, extra?: unknown): PropertyDecorator {
    extra = extra ?? {};
    return bind(path, { extra });
}
