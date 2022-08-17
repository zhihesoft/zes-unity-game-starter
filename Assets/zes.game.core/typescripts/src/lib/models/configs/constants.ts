import { ConfigBase } from "./config-base";
import { configuration } from "./config-decorators";

@configuration({ sheet: "constants", key: "id", allowMultiKey: false })
export class ConstantsConfig extends ConfigBase<{ id: string, value: unknown }> {

    get gameName(): string {
        return <string>this.findOne("name").value;
    }

    get startVipLevel(): number {
        return <number>this.findOne("startVipLevel").value || 0;
    }

    get testBool(): boolean {
        return <boolean>this.findOne("testBool").value || false;
    }
}