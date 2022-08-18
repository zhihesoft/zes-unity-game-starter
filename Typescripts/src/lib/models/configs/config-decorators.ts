import "reflect-metadata";

export const metaConfigSettings = Symbol("configSettings");

export interface ConfigurationSettings {
    sheet: string;
    key?: string;
    allowMultiKey?: boolean;
}

export function configuration(settings: ConfigurationSettings): ClassDecorator {
    return target => {
        Reflect.defineMetadata(metaConfigSettings, settings, target);
    };
}
