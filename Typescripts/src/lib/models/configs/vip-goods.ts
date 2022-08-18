import { singleton } from "tsyringe";
import { ConfigBase } from "./config-base";
import { configuration } from "./config-decorators";

export interface VipGoods {
    id: string;
    name: string;
    info: string;
    price: number;
    exp: number;
    icon: string;
}

@singleton()
@configuration({ sheet: "vip-goods", key: "id", allowMultiKey: false })
export class VipGoodsConfig extends ConfigBase<VipGoods> {

}