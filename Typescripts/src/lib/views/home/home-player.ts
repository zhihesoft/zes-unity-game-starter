import { UnityEngine } from "csharp";
import { BehaviorSubject } from "rxjs";
import { injectable } from "tsyringe";
import { bind, component } from "../../core/view-decorators";
import { AfterViewInit, OnInit } from "../../core/view-interfaces";
import { ViewRef } from "../../core/view-ref";
import { getLogger } from "../../logger";
import { waitForSeconds } from "../../util";
import { PlayerMod } from "../common/player-mod";

@component()
@injectable()
export class HomePlayer implements OnInit, AfterViewInit {
    constructor(
        private view: ViewRef
    ) { }

    private mod?: PlayerMod;
    @bind("") navAgent!: UnityEngine.AI.NavMeshAgent;
    moving = new BehaviorSubject(new UnityEngine.Vector2(0, 0));
    moveSpeed = 10;

    async ngOnInit() {
        this.mod = await PlayerMod.load("3700", this.view, this.view.host?.find(""));
    }

    ngAfterViewInit(): void {
        logger.debug(`view init`);
        this.navAgent.enabled = true;
        this.move();
        this.mod?.playAnim(this.mod.animMove);
    }

    setSpeed(speed: number) {
        this.moveSpeed = speed;
        this.navAgent.speed = speed;
    }

    private async move() {
        this.mod?.playAnim(this.mod.animMove);
        const v = this.moving.value;
        const dest = UnityEngine.Vector3.op_Addition(this.navAgent.transform.position, new UnityEngine.Vector3(v.x * this.moveSpeed, 0, v.y * this.moveSpeed));
        this.navAgent.SetDestination(dest);
        if (v.x != 0 || v.y != 0) {
            this.mod?.animator.SetFloat("speed", this.moveSpeed);
        } else {
            this.mod?.animator.SetFloat("speed", 0.5);
        }
        await waitForSeconds(0);
        this.move();
    }
}

const logger = getLogger(HomePlayer.name);
