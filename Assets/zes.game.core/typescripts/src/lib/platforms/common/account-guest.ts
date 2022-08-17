import { Liv } from "csharp";
import { container } from "tsyringe";
import { getLogger } from "../../logger";
import { HttpService } from "../../services/http-service";
import { Account, ReportType } from "../account";

export class AccountGuest extends Account {


    get name(): string {
        return "guest";
    }

    login(): Promise<void> {
        const http = container.resolve(HttpService);
        return http.guestLogin(Liv.App.deviceId)
            .then(ret => {
                this._token = ret.token;
                this._userid = ret.userid;
                Liv.Http.setToken(this._token);
            });
    }

    logout(): Promise<void> {
        throw new Error("Method not implemented.");
    }

    async report(type: ReportType): Promise<void> {
        logger.info(`report ${type}`);
        return;
    }
}

const logger = getLogger(AccountGuest.name);
