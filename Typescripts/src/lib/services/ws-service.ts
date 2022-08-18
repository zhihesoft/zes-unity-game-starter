import { Liv } from "csharp";
import { Subject } from "rxjs";
import { singleton } from "tsyringe";
import { getLogger } from "../logger";
import { waitRequest } from "../util";

@singleton()
export class WebSocketService {

    private ws?: Liv.WebSock;
    public onMessage = new Subject<string>();

    public get connected(): boolean {
        return this.ws?.connected || false;
    }

    async open(url: string, token: string): Promise<unknown> {
        if (this.ws) {
            return Promise.reject(new Error(`websocket is not close`));
        }
        const ws = new Liv.WebSock();
        return waitRequest(ws.open(url, token))
            .then(() => ws.onMessage = (msg) => {
                logger.debug(`ws got ${msg}`);
                this.onMessage.next(msg);
            })
            .then(() => this.ws = ws)
            .then(() => logger.info(`websock connected`));
    }

    close() {
        this.ws?.close();
        this.ws = undefined;
    }

    send(message: string) {
        if (this.ws) {
            this.ws.send(message);
        }
    }
}

const logger = getLogger(WebSocketService.name);
