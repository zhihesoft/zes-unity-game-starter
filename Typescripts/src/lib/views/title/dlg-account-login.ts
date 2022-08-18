import { TMPro } from "csharp";
import { injectable } from "tsyringe";
import { DialogRef } from "../../core/dialog-ref";
import { bind, click, component } from "../../core/view-decorators";
import { AfterViewInit } from "../../core/view-interfaces";
import { getLogger } from "../../logger";
import { AccountUName } from "../../platforms/common/account-uname";
import { Platform } from "../../platforms/platform";
import { PendingService } from "../../services/pending-service";
import { PrefsService } from "../../services/prefs-service";
import { ToastService } from "../../services/toast-service";

export enum LoginResult {
    succ,
    register,
    cancel,
}

@component({ template: "Assets/Bundles/ui-prefabs/title/controls/account-login-panel.prefab" })
@injectable()
export class AccountLoginDialog implements AfterViewInit {
    constructor(
        private toast: ToastService,
        private platform: Platform,
        private prefs: PrefsService,
        private dialogRef: DialogRef,
        private pending: PendingService,
    ) { }
    @bind("edit-username") username!: TMPro.TMP_InputField;
    @bind("edit-password") password!: TMPro.TMP_InputField;

    @click("buttons/back")
    onClickBack() {
        this.dialogRef.close(LoginResult.cancel);
    }

    @click("buttons/login", 0.5)
    login() {
        const account: AccountUName = <AccountUName>this.platform.getAccount("uname");
        this.pending.run(account.login(this.username.text, this.password.text))
            .then(() => {
                this.platform.setActiveAccount(account.name);
                this.prefs.setSystem(account.keyOfUsername, this.username.text);
                this.prefs.setSystem(account.keyOfPassword, this.password.text);
                this.dialogRef.close(LoginResult.succ);
            })
            .catch(err => {
                logger.error(`${err}`);
                this.toast.show(200265);
            });
    }

    @click("buttons/register", 0.5)
    async register(): Promise<void> {
        this.dialogRef.close(LoginResult.register);
    }

    ngAfterViewInit(): void {
        const account: AccountUName = <AccountUName>this.platform.getAccount("uname");
        this.username.text = this.prefs.getSystem(account.keyOfUsername, "");
        this.password.text = this.prefs.getSystem(account.keyOfPassword, "");
    }
}

const logger = getLogger(AccountLoginDialog.name);
