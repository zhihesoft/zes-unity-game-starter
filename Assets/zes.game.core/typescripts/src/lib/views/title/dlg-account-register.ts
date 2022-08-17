import { TMPro } from "csharp";
import { injectable } from "tsyringe";
import { DialogRef } from "../../core/dialog-ref";
import { bind, click, component } from "../../core/view-decorators";
import { getLogger } from "../../logger";
import { AccountUName } from "../../platforms/common/account-uname";
import { Platform } from "../../platforms/platform";
import { PendingService } from "../../services/pending-service";
import { PrefsService } from "../../services/prefs-service";
import { ToastService } from "../../services/toast-service";
import { isNullOrEmpty } from "../../util";
import { LoginResult } from "./dlg-account-login";

@component({ template: "Assets/Bundles/ui-prefabs/title/controls/account-register-panel.prefab" })
@injectable()
export class AccountRegisterDialog {
    constructor(
        private dialogRef: DialogRef,
        private platform: Platform,
        private prefs: PrefsService,
        private toast: ToastService,
        private pending: PendingService,
    ) { }

    @bind("edit-username") username!: TMPro.TMP_InputField;
    @bind("edit-password") password!: TMPro.TMP_InputField;
    @bind("edit-password2") password2!: TMPro.TMP_InputField;

    @click("buttons/register", 0.5)
    onClickRegister() {
        if (isNullOrEmpty(this.username.text) || isNullOrEmpty(this.password.text)) {
            return this.toast.show(200266);
        }
        if (this.password.text != this.password2.text) {
            return this.toast.show(200267);
        }

        const account: AccountUName = <AccountUName>this.platform.getAccount("uname");
        this.pending.run(account.register(this.username.text, this.password.text))
            .then(() => {
                this.platform.setActiveAccount(account.name);
                this.prefs.setSystem(account.keyOfUsername, this.username.text);
                this.prefs.setSystem(account.keyOfPassword, this.password.text);
                this.dialogRef.close(LoginResult.succ);
            })
            .catch(ex => {
                logger.error(`${ex}`);
                this.toast.show(200268);
            });
    }

    @click("buttons/back")
    async onClickBack() {
        this.dialogRef.close(LoginResult.cancel);
    }
}

const logger = getLogger(AccountRegisterDialog.name);
