/* eslint-disable @typescript-eslint/no-explicit-any */

import { Subject } from "rxjs";

export interface OnInit {
    ngOnInit(): void;
}

export interface AfterViewInit {
    ngAfterViewInit(): void;
}

export interface OnDestroy {
    ngOnDestroy(): void;
}

export interface OnActiveChanged {
    ngOnActiveChanged(activeStatus: boolean): void;
}

export interface OnSelected<T = any> {
    ngOnSelected: Subject<T>;
}

export function isOnSelected(component: unknown): component is OnSelected {
    return (<OnSelected>component).ngOnSelected != undefined;
}

export function isOnInit(component: unknown): component is OnInit {
    return (<OnInit>component).ngOnInit != undefined;
}

export function isAfterViewInit(component: unknown): component is AfterViewInit {
    return (<AfterViewInit>component).ngAfterViewInit != undefined;
}

export function isOnDestroy(component: unknown): component is OnDestroy {
    return (<OnDestroy>component).ngOnDestroy != undefined;
}

export function isOnActiveChanged(component: unknown): component is OnActiveChanged {
    return (<OnActiveChanged>component).ngOnActiveChanged != undefined;
}
