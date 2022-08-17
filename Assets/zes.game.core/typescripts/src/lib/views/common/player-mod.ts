import { UnityEngine } from "csharp";
import { injectable } from "tsyringe";
import { bind, component } from "../../core/view-decorators";
import { ViewRef } from "../../core/view-ref";

@component()
@injectable()
export class PlayerMod {

    readonly animShow = "show";
    readonly animHurt = "hurt";
    readonly animMove = "Move";
    readonly animSkill = "skill";

    static async load(id: string, parent: ViewRef, host?: UnityEngine.GameObject): Promise<PlayerMod> {
        // Assets/Bundles/mod-h/mod_h3700/mod_h3700.prefab
        const path = `Assets/Bundles/mod-h/mod_h${id}/mod_h${id}.prefab`;
        const ret = await parent.showChild(PlayerMod, { template: path, node: host }).then(v => v.component);
        if (!ret) {
            throw new Error(`create failed`);
        }
        return ret;
    }

    @bind("") animator!: UnityEngine.Animator;

    playAnim(name: string) {
        this.animator.Play(name);
    }

}