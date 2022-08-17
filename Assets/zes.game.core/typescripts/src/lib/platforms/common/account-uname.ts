import { Liv } from "csharp";
import { Md5 } from "ts-md5";
import { container } from "tsyringe";
import { getLogger } from "../../logger";
import { HttpService } from "../../services/http-service";
import { isNullOrEmpty } from "../../util";
import { Account, ReportType } from "../account";

export class AccountUName extends Account {

    public readonly keyOfUsername = "uname-account-username";
    public readonly keyOfPassword = "uname-account-password";

    get name(): string {
        return "uname";
    }

    login(): Promise<void>;
    login(username: string, password: string): Promise<void>;
    async login(username?: string, password?: string): Promise<void> {
        if (!username || !password || isNullOrEmpty(username) || isNullOrEmpty(password)) {
            return Promise.reject(`username or password cannot be null`);
        }
        password = Md5.hashStr(password).toLowerCase();
        const http = container.resolve(HttpService);
        return http.accountLogin(username, password)
            .then(ret => {
                this._token = ret.token;
                this._userid = ret.userid;
                Liv.Http.setToken(this._token);
            });
    }

    register(username: string, password: string): Promise<void> {
        if (!username || !password || isNullOrEmpty(username) || isNullOrEmpty(password)) {
            return Promise.reject(`username or password cannot be null`);
        }
        password = Md5.hashStr(password).toLowerCase();
        const http = container.resolve(HttpService);
        return http.accountRegister(username, password)
            .then(ret => {
                this._token = ret.token;
                this._userid = ret.userid;
                Liv.Http.setToken(this._token);
            });
    }

    async logout(): Promise<void> {
        this._token = "";
    }

    async report(type: ReportType): Promise<void> {
        logger.info(`report ${type}`);
        return;
    }

}

const logger = getLogger(AccountUName.name);
