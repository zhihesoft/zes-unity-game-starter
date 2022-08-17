import { Liv } from "csharp";
import { container, singleton } from "tsyringe";
import { getLogger } from "../logger";
import { GAME_CONFIG } from "../views/global-values";
import { Account } from "./account";
import { AccountUName } from "./common/account-uname";

@singleton()
export class Platform {

    protected _activeAccount?: Account;
    protected _defaultAccount?: Account;
    protected accounts: Account[] = [];

    public get name(): string { throw new Error(`not implement`); }

    public async init(): Promise<void> {
        const config: Liv.Config = container.resolve<Liv.Config>(GAME_CONFIG);
        if (config.allowGuest) {
            this.addAccount(new AccountUName());
        }
        await this.onInit();
    }

    public get defaultAccount(): Account {
        if (!this._defaultAccount) {
            throw new Error(`no default account found`);
        }
        return this._defaultAccount;
    }

    public setActiveAccount(name: string) {
        logger.info(`active account set to ${name}`);
        this._activeAccount = this.accounts.find(i => i.name == name);
    }

    public get activeAccount(): Account {
        if (!this._activeAccount) {
            throw new Error(`no active account found`);
        }
        return this._activeAccount;
    }

    public getAccount(name: string): Account {
        const ret = this.accounts.find(i => i.name == name);
        if (!ret) {
            throw new Error(`cannot find account system of ${name}`);
        }
        return ret;
    }

    protected onInit(): Promise<void> { throw new Error(`not implement`); }

    protected addAccount(account: Account) {
        const one = this.accounts.find(i => i.name == account.name);
        if (one) {
            throw new Error(`account system ${account.name} already existed`);
        }
        logger.info(`add account ${account.name}`);
        this.accounts.push(account);
    }
}

const logger = getLogger(Platform.name);
