/* eslint-disable @typescript-eslint/no-explicit-any */

export enum ReportType {
    CreateRole,
    UpdateRole,
    EnterGame,
}

export abstract class Account {

    protected _token = "";
    protected _userid = "";
    get userid() { return this._userid; }
    get token() { return this._token; }

    abstract get name(): string;
    abstract login(): Promise<void>;
    abstract logout(): Promise<void>;
    abstract report(type: ReportType): Promise<void>;
}