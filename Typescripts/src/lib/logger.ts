const loggers = new Map<string, Logger>();

export function getLogger(name: string): Logger {

    let ret = loggers.get(name);
    if (!ret) {
        ret = new Logger(name);
        loggers.set(name, ret);
    }
    return ret;
}

export class Logger {

    constructor(private name: string) { }

    private getMessage(message: string | object) {
        return `[${this.name}] ${message}`;
    }

    debug(message: string | object) {
        console.log(`[DEBUG] ${this.getMessage(message)}`);
    }

    info(message: string | object) {
        console.log(`[INFO] ${this.getMessage(message)}`);
    }

    warn(message: string | object) {
        console.warn(`[WARN] ${this.getMessage(message)}`);
    }

    error(message: string | object) {
        console.error(`[ERROR] ${this.getMessage(message)}`);
    }
}