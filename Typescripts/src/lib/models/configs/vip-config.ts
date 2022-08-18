import { ConfigBase } from "./config-base";
import { configuration } from "./config-decorators";

export class VipConfigData {
    level = "";
    name = "";
    info = "";
    price = 0;
    exp = 0;
    icon = "";
}

@configuration({ sheet: "vip-config", key: "level", allowMultiKey: false })
export class VipConfigConfig extends ConfigBase<VipConfigData> {
    constructor() {
        super(VipConfigData);
    }
}
