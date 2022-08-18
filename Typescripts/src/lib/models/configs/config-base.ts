/* eslint-disable @typescript-eslint/ban-types */
/* eslint-disable @typescript-eslint/no-explicit-any */
import "reflect-metadata";
import { ConfigurationSettings, metaConfigSettings } from "./config-decorators";

export interface Configuration {
    load(json: string): void;
    save(): string;
    getSettings(): ConfigurationSettings;
}

export class ConfigBase<T> implements Configuration {

    constructor(
        private itemCtor: { new(): T },
    ) {
        this.settings = <ConfigurationSettings>Reflect.getMetadata(metaConfigSettings, (<Object>this).constructor);
        if (!this.settings) {
            throw new Error(`cannot find config settings on this type ${Object.getPrototypeOf(this).name}`);
        }
    }

    private settings: ConfigurationSettings;

    protected items: T[] = [];

    protected idmap: Map<string, T[]> = new Map();

    getSettings(): ConfigurationSettings {
        return this.settings;
    }

    load(json: string) {
        const proto = JSON.parse(json);
        if (!Array.isArray(proto)) {
            throw new Error(`json ${json} is not an array`);
        }

        this.items = proto.map(i => {
            const ri = new this.itemCtor();
            Object.assign(ri, i);
            return ri;
        });

        const idKey = this.settings.key || "id";
        const allowMultiKey = this.settings.allowMultiKey || false;

        for (const item of this.items) {

            const keyValue = `${(<any>item)[idKey]}`.toLowerCase();

            if (this.idmap.has(keyValue) && !allowMultiKey) {
                throw new Error(`key ${keyValue} is already exist, this config doesnt allow multiKey.`);
            }

            if (!this.idmap.has(keyValue)) {
                this.idmap.set(keyValue, []);
            }

            const container = this.idmap.get(keyValue);
            if (!container) {
                throw new Error(`container cannot be null`);
            }
            container.push(item);
        }

        this.init();
    }

    save(): string {
        return JSON.stringify(this.items);
    }

    findOne(key: string): T {
        const container = this.find(key);
        return container[0];
    }

    find(key: string): T[] {
        const container = this.idmap.get(key.toLowerCase());
        if (!container) {
            throw new Error(`cannot find item of key ${key}`);
        }
        return container;
    }

    protected init() {
        // this.logger
    }
}