import { Cinemachine, UnityEngine } from "csharp";
import { injectable } from "tsyringe";
import { bind, component } from "../../core/view-decorators";
import { OnInit } from "../../core/view-interfaces";
import { ViewRef } from "../../core/view-ref";
import { HomePlayer } from "./home-player";
import { HomeWorld } from "./home-world";

@component({ template: "Assets/Bundles/scenes/home.unity" })
@injectable()
export class HomeScene implements OnInit {

    constructor(
        private view: ViewRef
    ) { }

    private world?: HomeWorld;
    public player?: HomePlayer;

    @bind("player") navAgent!: UnityEngine.AI.NavMeshAgent;
    @bind("vcam") vcam!: Cinemachine.CinemachineVirtualCamera;


    async ngOnInit() {
        this.world = await this.view.showChild(HomeWorld).then(v => v.component);
        this.player = await this.view.attachChild(HomePlayer, this.navAgent.gameObject).then(v => v.component);
    }
}