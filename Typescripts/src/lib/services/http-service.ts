/* eslint-disable @typescript-eslint/no-explicit-any */
import { Liv } from "csharp";
import { inject, singleton } from "tsyringe";
import { getLogger } from "../logger";
import { ServerConfig } from "../models/game/server-config";
import { LoginResp } from "../models/protocols/http/login-resp";
import { RequestError, waitRequest } from "../util";
import { GAME_CONFIG } from "../views/global-values";

@singleton()
export class HttpService {

    constructor(
        @inject(GAME_CONFIG) private config: Liv.Config
    ) { }

    private readonly http = Liv.Http;

    clearToken() {
        this.http.reset();
        logger.info(`token cleared`);
    }

    setToken(token: string) {
        this.http.setToken(token);
    }

    async guestLogin(token: string): Promise<LoginResp> {
        const url = "user/guest-login";
        return this.post(url, { token }).then(json => JSON.parse(json));
    }

    async accountLogin(username: string, password: string): Promise<LoginResp> {
        const url = "user/account-login";
        return this.post(url, { username, password }).then(json => JSON.parse(json));
    }

    async accountRegister(username: string, password: string): Promise<LoginResp> {
        const url = "user/account-register";
        return this.post(url, { username, password }).then(json => JSON.parse(json));
    }

    async serverList(): Promise<ServerConfig[]> {
        const url = "server/list";
        return this.post(url).then(json => JSON.parse(json));
    }

    async enterWorld(id: number): Promise<{ token: string, url: string }> {
        const url = "server/enter";
        return this.post(url, { serverId: id })
            .then(ret => {
                const json = JSON.parse(ret);
                logger.info(`token is ${ret}`);
                return json;
            });
    }

    private getUrl(url: string): string {
        if (this.config.loginServer.endsWith("/")) {
            return this.config.loginServer + url;
        } else {
            return this.config.loginServer + "/" + url;
        }
    }

    private post<T = any>(url: string, args?: T): Promise<string> {
        url = this.getUrl(url);
        const json = args ? JSON.stringify(args) : "";
        return waitRequest(this.http.post(url, json)).catch(err => {
            if (err instanceof RequestError) {
                return Promise.reject(err);
            } else {
                return Promise.reject(new RequestError(500, `${err}`));
            }
        });
    }

}

const logger = getLogger(HttpService.name);

