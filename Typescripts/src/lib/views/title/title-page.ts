import { Liv, UnityEngine } from "csharp";
import { BehaviorSubject } from "rxjs";
import { inject, injectable } from "tsyringe";
import { Dialog } from "../../core/dialog-ref";
import { bind, click, component, text } from "../../core/view-decorators";
import { AfterViewInit } from "../../core/view-interfaces";
import { Transition } from "../../core/view-transition";
import { getLogger } from "../../logger";
import { ServerConfig } from "../../models/game/server-config";
import { Platform } from "../../platforms/platform";
import { HttpService } from "../../services/http-service";
import { PendingService } from "../../services/pending-service";
import { PrefsService } from "../../services/prefs-service";
import { ToastService } from "../../services/toast-service";
import { GAME_CONFIG } from "../global-values";
import { AccountLoginDialog, LoginResult } from "./dlg-account-login";
import { AccountRegisterDialog } from "./dlg-account-register";
import { EnterGameDialog } from "./dlg-enter-game";

@injectable()
@component({ template: "Assets/Bundles/ui-prefabs/title/title-page.prefab", transition: Transition.Fade })
export class TitlePage implements AfterViewInit {

    constructor(
        private dialog: Dialog,
        private platform: Platform,
        private http: HttpService,
        private toast: ToastService,
        private prefs: PrefsService,
        private pending: PendingService,
        @inject(GAME_CONFIG) private config: Liv.Config
    ) { }

    private servers: ServerConfig[] = [];

    @text("version") version = new BehaviorSubject<string>("");
    @bind("login") loginPanel?: UnityEngine.GameObject;
    @bind("login-detail") loginDetailPanel!: UnityEngine.GameObject;
    @bind("login/button-guest-login") guestLoginButton?: UnityEngine.GameObject;


    @click("login/button-guest-login", 0.5)
    private async showAccountLoginDlg() {
        this.loginPanel?.SetActive(false);
        const ref = await this.dialog.open(AccountLoginDialog, {}, this.loginDetailPanel);
        ref.afterClosed().subscribe(async (status: LoginResult) => {
            switch (status) {
                case LoginResult.cancel:
                    this.loginPanel?.SetActive(true);
                    break;
                case LoginResult.succ:
                    this.onLoginSucc();
                    break;
                case LoginResult.register:
                    this.showAccountRegisterDlg();
                    break;
            }
        });
    }


    private async showAccountRegisterDlg() {
        const ref = await this.dialog.open(AccountRegisterDialog, {}, this.loginDetailPanel);
        ref.afterClosed().subscribe(async (status: LoginResult) => {
            switch (status) {
                case LoginResult.cancel:
                    this.showAccountLoginDlg();
                    break;
                case LoginResult.succ:
                    this.onLoginSucc();
                    break;
                case LoginResult.register:
                    this.showAccountRegisterDlg();
                    break;
            }
        });
    }

    @click("login/button-login", 0.5)
    private async onClickLogin() {

        // await this.loadServerList();
        // this.loginPanel?.SetActive(false);
        // this.onLoginSucc();

        this.pending.run(this.platform.defaultAccount.login())
            .then(() => this.platform.setActiveAccount(this.platform.defaultAccount.name))
            .then(this.loadServerList.bind(this))
            .then(this.onLoginSucc.bind(this))
            .catch(err => {
                logger.error(`${err}`);
                this.toast.show(200265);
            });
    }

    ngAfterViewInit(): void {
        this.version.next(Liv.App.version);
        this.guestLoginButton?.SetActive(this.config.allowGuest);
    }

    @bind("toggle", { type: UnityEngine.UI.Toggle, event: "onValueChanged" })
    private onToggleChanged(v: boolean) {
        logger.debug(`toggle changed: ${v}`);
    }


    private async onLoginSucc() {
        logger.info(`account login succ`);
        this.prefs.setUser(this.platform.activeAccount.userid);
        this.loginPanel?.SetActive(false);
        this.dialog.open(EnterGameDialog, this.servers, this.loginDetailPanel);
    }

    private async loadServerList() {
        this.servers = await this.http.serverList();
        // this.servers = [];
        // for (let i = 0; i < 77; i++) {
        //     this.servers.push({ id: i + 1, name: `TEST${i + 1}`, url: "", state: 0 });
        // }
    }
}

const logger = getLogger(TitlePage.name);
