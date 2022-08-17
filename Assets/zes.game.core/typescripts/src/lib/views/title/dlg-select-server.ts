import _ from "lodash";
import { inject, injectable } from "tsyringe";
import { DialogRef } from "../../core/dialog-ref";
import { ListView } from "../../core/list-view";
import { click, component, view } from "../../core/view-decorators";
import { OnInit } from "../../core/view-interfaces";
import { ViewRef, VIEW_DATA } from "../../core/view-ref";
import { getLogger } from "../../logger";
import { ServerConfig } from "../../models/game/server-config";
import { PrefsService } from "../../services/prefs-service";
import { AreaItem } from "./area-item";
import { ServerItem } from "./server-item";

@component({ template: "Assets/Bundles/ui-prefabs/title/controls/select-server-panel.prefab" })
@injectable()
export class SelectServerDialog implements OnInit {
    constructor(
        private view: ViewRef,
        private prefs: PrefsService,
        private dialogRef: DialogRef,
        @inject(VIEW_DATA) private servers: ServerConfig[]
    ) { }

    private readonly serversPerZone = 10;
    private zone = 0;

    @view("dlg-background/recommend/list/server-item-hot") recommendServer!: ServerItem;
    @view("dlg-background/recommend/list/server-item-last") lastServer!: ServerItem;
    @view("dlg-background/servers", ServerItem) nodeServers!: ListView<ServerConfig>;
    @view("dlg-background/areas/Viewport/Content", AreaItem) nodeAreas!: ListView<number>;

    @click("dlg-background/close")
    onClickClose() { this.dialogRef.close(); }
    @click("dlg-background/recommend/list/server-item-hot")
    onClickHotServer() { this.dialogRef.close(this.recommendServer.getData()); }
    @click("dlg-background/recommend/list/server-item-last")
    onClickLastServer() { this.dialogRef.close(this.lastServer.getData()); }

    async ngOnInit() {
        logger.debug(`servers: ${JSON.stringify(this.servers)}`);
        const max = _.maxBy(this.servers, (i) => i.id);
        if (!max) {
            throw new Error(`need at least on area`);
        }
        const ids: number[] = [];
        for (let i = 0; i < max.id / this.serversPerZone; i++) {
            ids.push(i);
        }
        this.nodeAreas.setData(ids);
        this.nodeServers.setData(this.getServers());
        this.nodeAreas.onItemSelected.subscribe(this.onSelectZone.bind(this));
        this.nodeServers.onItemSelected.subscribe(this.onSelectServer.bind(this));
        const recommend = _.last(this.servers);
        if (recommend) {
            this.recommendServer.setData(recommend);
        }
        const lastId = this.prefs.lastServerId;
        const last = this.servers.find(i => i.id == lastId);
        if (last) {
            this.lastServer.setData(last);
        } else {
            this.lastServer.view.gameObject?.SetActive(false);
        }
    }

    private onSelectServer(config: ServerConfig) {
        this.dialogRef.close(config);
    }

    private onSelectZone(id: number) {
        this.zone = id;
        this.nodeServers.setData(this.getServers());
    }

    private getServers(): ServerConfig[] {
        const ret: ServerConfig[] = [];
        const start = this.zone * this.serversPerZone;
        const end = start + this.serversPerZone;
        for (const item of this.servers) {
            if (item.id > start && item.id <= end) {
                ret.push(item);
            }
        }
        return ret;
    }
}

const logger = getLogger(SelectServerDialog.name);
