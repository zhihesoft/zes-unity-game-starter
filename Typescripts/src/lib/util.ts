/* eslint-disable @typescript-eslint/no-explicit-any */

import { UnityEngine } from "csharp";
import _ from "lodash";

export class RequestError {
    constructor(
        public code: number,
        public message: string
    ) { }

    toString(): string { return `[${this.code}] ${this.message}`; }
}

export function isGameObject(obj: unknown): obj is UnityEngine.GameObject {
    return (<UnityEngine.GameObject>obj).activeSelf != undefined;
}

export function isNullOrEmpty(value: string | undefined | null) {
    return (!value || value.length <= 0);
}

export async function waitForSeconds(seconds: number) {
    return new Promise(resolve => setTimeout(resolve, seconds * 1000));
}

export function waitUntil(condition: () => boolean): Promise<void> {
    return new Promise(resolve => {
        const fun = () => {
            if (condition()) {
                resolve();
            } else {
                setTimeout(fun, 0);
            }
        };
        fun();
    });
}

/**
 * get random int from [min, max]
 * @param min min value
 * @param max max value
 */
export function randomInt(min: number, max: number): number {
    return _.random(min, max);
}


export function empty() {
    // empty function; make compiler happy.
}

export function format(fmt: string, ...args: any) {
    // const args = Array.prototype.slice.call(arguments, 1);
    return fmt.replace(/{(\d+)}/g, (match, number) => {
        return typeof args[number] != 'undefined'
            ? args[number]
            : match
            ;
    });
}