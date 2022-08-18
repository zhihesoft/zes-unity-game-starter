import { BehaviorSubject } from "rxjs";
import { inject, injectable } from "tsyringe";
import { Dialog } from "../../core/dialog-ref";
import { click, component, text } from "../../core/view-decorators";
import { OnInit } from "../../core/view-interfaces";
import { VIEW_DATA } from "../../core/view-ref";
import { getLogger } from "../../logger";
import { ServerConfig } from "../../models/game/server-config";
import { Platform } from "../../platforms/platform";
import { HttpService } from "../../services/http-service";
import { LanguageService } from "../../services/language-service";
import { PageService } from "../../services/page-service";
import { PendingService } from "../../services/pending-service";
import { PrefsService } from "../../services/prefs-service";
import { ToastService } from "../../services/toast-service";
import { WebSocketService } from "../../services/ws-service";
import { format } from "../../util";
import { HomePage } from "../home/home-page";
import { SelectServerDialog } from "./dlg-select-server";

@component({ template: "Assets/Bundles/ui-prefabs/title/controls/enter-game.prefab" })
@injectable()
export class EnterGameDialog implements OnInit {
    constructor(
        private lang: LanguageService,
        private pages: PageService,
        private dialog: Dialog,
        private platform: Platform,
        private toast: ToastService,
        private pending: PendingService,
        private http: HttpService,
        private ws: WebSocketService,
        private prefs: PrefsService,
        @inject(VIEW_DATA) private servers: ServerConfig[]
    ) { }

    private currentServer?: ServerConfig;

    ngOnInit(): void {
        const lastId = this.prefs.lastServerId;
        const lastServer = this.servers.find(i => i.id == lastId) || this.servers[this.servers.length - 1];
        this.setServerConfig(lastServer);
    }

    setServerConfig(server: ServerConfig) {
        if (server) {
            const fmt = this.lang.get(500646);
            const label = format(fmt, server.id);
            this.serverName.next(`${label} ${server.name}`);
            this.currentServer = server;
        }
    }

    @text("button-server/server-name")
    serverName = new BehaviorSubject("");


    @click("button-server")
    async onClickSelectServer() {
        logger.debug(`open select server dialog with servers: ${JSON.stringify(this.servers)}`);
        const ref = await this.dialog.open(SelectServerDialog, this.servers);
        ref.afterClosed().subscribe(result => {
            this.setServerConfig(result);
        })
    }

    @click("button-enter")
    async onClickEnter() {
        logger.debug(`try to enter game ${this.currentServer?.id}`);
        if (!this.currentServer) {
            throw new Error(`current server is null`);
        }

        return this.pending.run(
            this.http.enterWorld(this.currentServer.id)
                .then(data => this.ws.open(data.url, data.token))
                .then(() => logger.debug(`websocket state is ${this.ws.connected}`))
                .then(() => this.prefs.lastServerId = this.currentServer?.id || 0)
                .then(() => this.pages.replace(HomePage))

        ).catch(err => {
            this.ws.close();
            logger.error(`${err}`);
            this.toast.show(200265);
        });

        // return this.pending.run(this.ws.open(this.currentServer.url, this.platform.activeAccount.token))
        //     .then(() => logger.debug(`websocket state is ${this.ws.connected}`))
        //     .then(() => this.prefs.lastServerId = this.currentServer?.id || 0)
        //     .then(() => this.pages.replace(HomePage))
        //     .catch(err => {
        //         this.ws.close();
        //         logger.error(`${err}`);
        //         this.toast.show(200265);
        //     });
    }
}

const logger = getLogger(EnterGameDialog.name);
